using Common.Exceptions;
using Common.Packet.Handlers;
using Common.Packet.Serializers;
using GladNet.Common;
using GladNet.Server.Connections;
using GladNet.Server.Connections.Readers;
using GladNet.Server.Logging;
using Lidgren.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet.Server
{

	internal class CoreAttribute : Attribute
	{
		//Empty
		//This is just used to easily find the class that implements this base class in loaded assemblies.
	}

	[CoreAttribute]
	/// <summary>
	/// This class represents the core/central point exposed to the user of the library. A class should implement this core class otherwise nothing can actually occur on the server.
	/// </summary>
	public abstract class ServerCore<LoggerType> : ILoggable<LoggerType> where LoggerType : Logger
	{
		/// <summary>
		/// Forces the application's core to provide implementation for a logger class to log information to.
		/// </summary>
		public LoggerType ClassLogger { get; private set; }

		private IList<ConnectionResponse> UnhandledServerConnections;

		private bool isReady = false;

		public ConnectionCollection<ClientPeer, NetConnection> Clients;
		public ConnectionCollection<ServerPeer, NetConnection> ServerConnections;

		public bool isListening { get; private set; }

		private readonly NetServer lidgrenServerObj;

		public readonly string ExpectedClientHailMessage;

		private readonly SerializationManager SerializerRegister;

		private readonly PacketConverter Converter;

		//private readonly LidgrenMessageConverter HighlevelMessageConverter;
		
		/// <summary>
		/// Indicates the type of server. This is to distinguish between incoming and outgoing connections in server to server scenarios.
		/// Without this being considered during setup you won't be able to differentiate between a connection request from a client, or another subserver, from a server.
		/// </summary>
		protected abstract byte ServerTypeUniqueByte { get; }

		//TODO: Refactor
		public ServerCore(LoggerType loggerInstance, string appName, int port, string hailMessage)
		{
			//Create the registers for serializers and the messagehandlers for a given serializer too.
			SerializerRegister = new SerializationManager();
			//Register profobuf-net as it's used internally
			//Create the message converter that will hold references to 
			//HighlevelMessageConverter = new LidgrenMessageConverter();
			Converter = new PacketConverter();
			UnhandledServerConnections = new List<ConnectionResponse>();

			//Register the default serializer
			this.RegisterSerializer<ProtobufNetSerializer>();

			ClassLogger = loggerInstance;
			Clients = new ConnectionCollection<ClientPeer, NetConnection>();
			ServerConnections = new ConnectionCollection<ServerPeer, NetConnection>();

			//Set the server status as not listening
			isListening = false;

			//Construction of the Lidgren server listener
			NetPeerConfiguration config = new NetPeerConfiguration(appName);
			config.Port = port;

			//This message type must be set to true so we can manage connections with hailmessages and so that we can
			//reject the majority of non-malicious malformed connection attempts
			config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
			config.SetMessageTypeEnabled(NetIncomingMessageType.HighlevelDataPackage, true);

			//Reduces GC
			config.UseMessageRecycling = true;

			lidgrenServerObj = new NetServer(config);

			ExpectedClientHailMessage = hailMessage;
		}

		/// <summary>
		/// Called internally when a client attempts to connect to this server passing in details about the connection.
		/// </summary>
		/// <param name="request">A package of details about the client attempting to connect to the server.</param>
		/// <returns></returns>
		protected abstract ClientPeer OnAttemptedConnection(ConnectionRequest request);

		/// <summary>
		/// Called internally when this application recieves a success response to its connection attempt to another server. Servers connectiong to this application are passed into OnAttemptedConnection instead
		/// This internally creates a client object on the server that acts as a client to that server on a seperate port while other servers who choose instead to connect to us are represented by a seperate server object
		/// internally that listens on a different port.
		/// </summary>
		/// <param name="response">Response from the server that connection was attempted from.</param>
		/// <returns>A ServerPeer base instance for the other server to be managed as or null if for some reason the accepted connection is rejected.</returns>
		protected abstract ServerPeer OnConnectionSuccess(ConnectionResponse response);

		//protected virtual void OnConnectionFailed(ConnectionResponse response, byte connectionType);

		/// <summary>
		/// Called internally when the application starts up. The implementing class should register all the custom Protobuf-net (The default serializer for the Packet base class)
		/// packet classes so that the serializer is aware of them for both sending and recieving the packages. All internal packages that aren't written by the implementing library are handled
		/// internally.
		/// </summary>
		/// <param name="registerAsDefaultFunc">The defauly packet registeration function.</param>
		protected abstract void RegisterPackets(Func<Type, bool> registerAsDefaultFunc);

		/// <summary>
		/// Provides a method for users to register their own serializer with the networking. This will create a handler to handle packet serialized with the serializer
		/// as long as the reciever also register the serializer too.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected bool RegisterSerializer<T>() where T : SerializerBase
		{
			if (SerializerRegister.HasKey(Serializer<T>.Instance.SerializerUniqueKey))
				throw new Exception("Failed to register Serializer of Type: " + Serializer<T>.Instance.GetType().FullName + " due to a already inuse serializer key.");

			return this.SerializerRegister.Register(Serializer<T>.Instance, Serializer<T>.Instance.SerializerUniqueKey);
		}

		/// <summary>
		/// Attempts to connect to another server application from this server core/application.
		/// </summary>
		/// <param name="endPoint"></param>
		/// <param name="applicationName"></param>
		/// <param name="hailMessage"></param>
		/// <returns>Indicates whether a connection has been successfully attempted (doesn't indicate if it was established).</returns>
		public bool ConnectToServer(IPEndPoint endPoint, string hailMessage)
		{
			if (endPoint == null)
				return false;
			else
			{
				var msg = lidgrenServerObj.CreateMessage();
				msg.Write(hailMessage);
				msg.Write(ServerTypeUniqueByte);

#if DEBUGBUILD
				AsyncConsoleLogger.Instance.LogDebug("Attempting to connect to another server.");
#endif
				NetConnection possibleConnection = lidgrenServerObj.Connect(endPoint, msg);

				ConnectionResponse response = new ConnectionResponse(endPoint, possibleConnection.RemoteUniqueIdentifier,
					possibleConnection, hailMessage);

				this.UnhandledServerConnections.Add(response);
				return true;
			}
		}

		/// <summary>
		/// Is called if the connection attempt resulted in a failure the remote connection point was unreachable or did not respond.
		/// </summary>
		/// <param name="response">Response information that may have been gathered upon the failure.</param>
		protected virtual void OnConnectionFailure(ConnectionResponse response)
		{
#if DEBUGBUILD
			//TODO: Add information to the debug logger string for the response.
			ClassLogger.LogDebug("Failed to connect to a server.");
#endif
			//Just fail silently if not overidden.
		}

		/// <summary>
		/// Internally called by the app when it's loaded. Cannot be overidden and cannot be remove.
		/// </summary>
		internal void InternalOnStartup()
		{
			try
			{
				//Internally we should register Protobuf-net packets here.
				
				Packet.Register(typeof(EmptyPacket), true);

				//Add in all the custom packets the server uses.
				RegisterPackets(Packet.Register);
				Packet.LockInProtobufnet();

				lidgrenServerObj.Start();
				OnStartup();
				//Sets the server to a ready state to begin polling and accepting connections.
				isReady = true;
			}
			catch(Exception e)
			{
				this.ClassLogger.LogError("Exception: " + e.Message);/* + "\n\n" + e.Source + "\n\n" + e.Data + e.StackTrace + "\n\n\n" +
					e.InnerException.Message + e.InnerException.Source + e.InnerException.StackTrace);*/
			}
		}

		/// <summary>
		/// Internally called by the app when a shutdown request has been recieved.
		/// </summary>
		internal void InternalOnShutdown()
		{
			isReady = false;
#if DEBUGBUILD
			ClassLogger.LogDebug("Server shutdown requested.");
#endif
			OnShutdown();
		}

		protected abstract void OnStartup();
		protected abstract void OnShutdown();

		#region IListener methods
		public void Start()
		{
			isListening = true;
		}

		/// <summary>
		/// This method should be called and serviced on the main thread or for Unity3D in a coroutine.
		/// </summary>
		public void Poll()
		{
			while(isReady)
			{
				MessagePoll(lidgrenServerObj);
			}

			this.InternalOnShutdown();
		}

		private void MessagePoll(NetPeer peer)
		{
			NetIncomingMessage msg = peer.ReadMessage();
			HighLevelMessage hlmsg = peer.ReadHighlevelMessage();

			//TODO: Refactor
			if (msg != null && msg.SenderConnection != null)
			{
#if DEBUGBUILD
				ClassLogger.LogDebug("Recieved a message from client ID: " + msg.SenderConnection.RemoteUniqueIdentifier);
#endif
				switch (msg.MessageType)
				{
					case NetIncomingMessageType.ConnectionApproval:
						ClassLogger.LogDebug("Hit connection approval");
						if (TryReadHailMessage(msg, ExpectedClientHailMessage))
						{
							ClassLogger.LogDebug("About to approve.");
							try
							{
								msg.SenderConnection.Approve();
								this.RegisterApprovedConnection(msg.SenderConnection, msg.SenderConnection.RemoteHailMessage.ReadByte());
							}
							catch (NetException e)
							{
#if DEBUGBUILD
								ClassLogger.LogError("Failed to read connection type byte from hail message packet. Exception: " + e.Message);
#endif
							}
						}
						break;
					case NetIncomingMessageType.StatusChanged:
						//Malicious user could try to send a fake StatusChange without the extra byte so try to catch NetException.
						try
						{
							if (Clients.HasKey(msg.SenderConnection.RemoteUniqueIdentifier))
								this.ReadStatusChange((NetConnectionStatus)msg.ReadByte(), Clients[msg.SenderConnection.RemoteUniqueIdentifier]);
							else if (ServerConnections.HasKey(msg.SenderConnection.RemoteUniqueIdentifier))
								this.ReadStatusChange((NetConnectionStatus)msg.ReadByte(), ServerConnections[msg.SenderConnection.RemoteUniqueIdentifier]);

							//If this point is reached it indicates that the status change is not from a registered connection and
							//this could indicate potentially a subserver connecting or maybe a client message before hail approval.
							ReadStatusChange((NetConnectionStatus)msg.ReadByte(), msg.SenderConnection);
						}
						catch(NetException e)
						{
#if DEBUGBUILD
							ClassLogger.LogError("NetConnection ID: " + msg.SenderConnection.RemoteUniqueIdentifier + " sent a potentially malicious StatusChange update.");
#endif
						}
						break;
				}

				peer.Recycle(msg);
			}

			if (hlmsg != null && hlmsg.OriginalLidgrenMessage.SenderConnection != null)
			{

#if DEBUGBUILD
				ClassLogger.LogDebug("Recieved a high level message from client ID: " + hlmsg.OriginalLidgrenMessage.SenderConnection.RemoteUniqueIdentifier);
#endif
				try
				{
					if (Clients.HasKey(hlmsg.OriginalLidgrenMessage.SenderConnection.RemoteUniqueIdentifier))
					{
						ClassLogger.LogDebug("About to call Processing");
						try
						{
							LidgrenTransferPacket transferPacket = Serializer<ProtobufNetSerializer>.Instance.Deserialize<LidgrenTransferPacket>(hlmsg.PacketBytes);

							if(Clients.HasKey(hlmsg.OriginalLidgrenMessage.SenderConnection.RemoteUniqueIdentifier))
								ProcessHigherLevelPacket(transferPacket, Clients[hlmsg.OriginalLidgrenMessage.SenderConnection.RemoteUniqueIdentifier]);
							//If it's not a client's high level message it might be a server's. We shouldn't disconnect a connection if it's neither
							//Because a maliciously crafted package could be made to impersonate a current client (although examination of Lidgren
							//internals would be required to know for sure.
							else if(ServerConnections.HasKey(hlmsg.OriginalLidgrenMessage.SenderConnection.RemoteUniqueIdentifier))
							{
								ProcessHigherLevelPacket(transferPacket, ServerConnections[hlmsg.OriginalLidgrenMessage.SenderConnection.RemoteUniqueIdentifier]);
							}
						}
						catch (Exception e)
						{
							//TODO: Implement better exception handling
							throw new LoggableException("Failed to forward HighLevelMessage to a peer.", e, Logger.LogType.Error);
						}
					}
				}
				catch(LoggableException e)
				{
					ClassLogger.LogError(e.Message);
				}

				hlmsg.OriginalLidgrenMessage.SenderConnection.Peer.Recycle(hlmsg.OriginalLidgrenMessage);
			}
		}

		private void ReadStatusChange(NetConnectionStatus netConnectionStatus, NetConnection netConnection)
		{
#if DEBUGBUILD
			this.ClassLogger.LogDebug("Entered into un-collected connection status handler.");
#endif


			ConnectionResponse cr = this.UnhandledServerConnections
				.FirstOrDefault(x => x.InternalNetConnection.RemoteUniqueIdentifier == netConnection.RemoteUniqueIdentifier);
			switch(netConnectionStatus)
			{
				case NetConnectionStatus.Disconnected:
					//If we get a disconnection and it's about an unhandled server that has yet to fully connect we should remove it.
					if(cr == null)
						return;

					UnhandledServerConnections.Remove(cr);
					this.OnConnectionFailure(cr);
					break;
				case NetConnectionStatus.Connected:
					if(cr == null)
						return;

					UnhandledServerConnections.Remove(cr);
					ServerPeer newPeer = this.OnConnectionSuccess(cr);

					//If this is not null it means the application created a new serverpeer and it needs to be added
					//To the collection to allow for message forwarding and such.
					if (newPeer != null)
						ServerConnections.Register(new ConnectionPair<NetConnection, ServerPeer>(netConnection, newPeer),
							netConnection.RemoteUniqueIdentifier);
					break;
			}
		}

		private void ReadStatusChange<PeerType>(NetConnectionStatus status, ConnectionPair<NetConnection, PeerType> peerPair)
			where PeerType : Peer
		{
			switch(status)
			{
				case NetConnectionStatus.Disconnected:
					peerPair.HighlevelPeer.InternalOnDisconnection();
					break;
			}
		}

		private bool TryReadHailMessage(NetIncomingMessage msg, string expected)
		{
			try
			{
				return msg.SenderConnection.RemoteHailMessage != null &&
					expected == msg.SenderConnection.RemoteHailMessage.ReadString();
			}
			catch (NetException e)
			{
				//This exception will occur when we're reading from the buffer but we can't get the expected byte or the hail message
#if DEBUGBUILD
				this.ClassLogger.LogError("Failed to read hail message. Exception: " + e.Message);
#endif
				return false;
			}
		}

		private void ProcessHigherLevelPacket(LidgrenTransferPacket packet, Peer passTo)
		{
			if (passTo == null)
				return;

#if DEBUGBUILD
			ClassLogger.LogDebug("Handling higherlevel packet. OperationType: " + ((Packet.OperationType)packet.OperationType).ToString() + " Serialization ID: " 
				+ packet.SerializerKey);
#endif

			try
			{
				if (SerializerRegister.HasKey(packet.SerializerKey))
					//TODO: Refactor
					switch ((Packet.OperationType)packet.OperationType)
					{
						case Packet.OperationType.Event:
							EventPackage ePackage = GeneratePackage<EventPackage>(packet, packet.SerializerKey);
							if (ePackage != null)
								passTo.PackageRecieve(ePackage);
							break;
						case Packet.OperationType.Request:
							ClassLogger.LogDebug("Hit request");
							RequestPackage rqPackage = GeneratePackage<RequestPackage>(packet, packet.SerializerKey);
							if (rqPackage != null)
							{
								ClassLogger.LogDebug("About to call peer method");
								passTo.PackageRecieve(rqPackage);
							}
							break;
						case Packet.OperationType.Response:
							ResponsePackage rPackage = GeneratePackage<ResponsePackage>(packet, packet.SerializerKey);
							if (rPackage != null)
								passTo.PackageRecieve(rPackage);
							break;
					}
				else
					ClassLogger.LogError("Recieved a packet that cannot be handled due to not having a serializer registered with byte code: " + packet.SerializerKey);
			}
			catch (NullReferenceException e)
			{
				this.ClassLogger.LogError(typeof(Peer).FullName + " ID: " + passTo.InternalNetConnection.RemoteUniqueIdentifier + " sent packet that we failed to deserialize with method key " +
					packet.SerializerKey + ".");
#if DEBUGBUILD
				throw new SerializationException(typeof(EventPackage), null, e, "Failed to deserialize for eventpackage, likely due to serializer key: " +
					packet.SerializerKey + " being unreigstered.");
#endif
			}
			catch(LoggableException e)
			{
				this.ClassLogger.LogError(e.Message);
			}
		}

		private T GeneratePackage<T>(LidgrenTransferPacket packet, byte serializerKey)
			where T : NetworkPackage, new()
		{
			return Converter.BuildIncomingNetPackage<T>(packet, SerializerRegister.GetValue(serializerKey));
		}

		private void RegisterApprovedConnection(NetConnection netConnection, byte connectionType)
		{
			ClientPeer cp = OnAttemptedConnection(new ConnectionRequest(netConnection.RemoteEndPoint, netConnection.RemoteUniqueIdentifier, 
				netConnection, connectionType));
			if (cp != null)
			{
				//A connecting subserver should be treated as a client connection. ServerPeers are peers that should be created
				//on the connected end, which can recieve events and responses, to its requests. So all approved connections should be added to the client
				//registry.
				ClassLogger.LogDebug("Connectiontype ID: " + connectionType);
#if DEBUGBUILD
				ClassLogger.LogDebug("Adding new client to ConnectionCollection. ID: " + cp.UniqueConnectionId);
#endif
				Clients.Register(new ConnectionPair<NetConnection, ClientPeer>(netConnection, cp), netConnection.RemoteUniqueIdentifier);
			}
			else
				ClassLogger.LogError("cp is null");
		}

		/*private void ProcessServerToServerResponse(NetConnection connection)
		{
			ConnectionResponse cr = UnhandledServerConnections.FirstOrDefault
				(x => x.InternalNetConnection.RemoteUniqueIdentifier == connection.RemoteUniqueIdentifier);

			if (cr == null)
			{
				ClassLogger.LogError("Connected but failed to find ConnectionResponse. ID: "
					+ connection.RemoteUniqueIdentifier);
				return;
			}
		}*/
		#endregion
	}
}
