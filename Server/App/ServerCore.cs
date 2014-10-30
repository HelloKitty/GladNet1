using Common.Exceptions;
using Common.Packet.Handlers;
using Common.Packet.Serializers;
using GladNet.Common;
using GladNet.Server.Connections;
using GladNet.Server.Logging;
using Lidgren.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
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
	public abstract class ServerCore<LoggerType> : ILoggable<LoggerType>, IListener where LoggerType : Logger
	{
		/// <summary>
		/// Forces the application's core to provide implementation for a logger class to log information to.
		/// </summary>
		public LoggerType ClassLogger { get; private set; }

		internal IList<NetClient> PeerListeners { get; private set; }

		private bool isReady = false;

		//These are seperated for effiency. They were originally a 2D dictionary

		private Dictionary<long, Peer> _InConnections;
		public IReadOnlyDictionary<long, Peer> InConnections 
		{
			get { return _InConnections; }
		}

		private Dictionary<NetPeer, ServerPeer> _OutConnections;
		public IEnumerable<ServerPeer> OutConnections 
		{ 
			get { return _OutConnections.Values; }
		}

		public bool isListening { get; private set; }

		private readonly NetServer lidgrenServerObj;

		public readonly string ExpectedClientHailMessage;

		private readonly SerializationManager SerializerRegister;

		private readonly LidgrenMessageConverter HighlevelMessageConverter;

		//TODO: Refactor
		public ServerCore(LoggerType loggerInstance, string appName, int port, string hailMessage)
		{
			//Create the registers for serializers and the messagehandlers for a given serializer too.
			SerializerRegister = new SerializationManager();
			//Register profobuf-net as it's used internally
			//Create the message converter that will hold references to 
			HighlevelMessageConverter = new LidgrenMessageConverter();

			//Register the default serializer
			this.RegisterSerializer<ProtobufNetSerializer>();


			ClassLogger = loggerInstance;
			_InConnections = new Dictionary<long, Peer>();
			_OutConnections = new Dictionary<NetPeer, ServerPeer>();

			PeerListeners = new List<NetClient>();

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
		protected abstract Peer OnAttemptedConnection(ConnectionRequest request);

		/// <summary>
		/// Called internally when this application recieves a success response to its connection attempt to another server. Servers connectiong to this application are passed into OnAttemptedConnection instead
		/// This internally creates a client object on the server that acts as a client to that server on a seperate port while other servers who choose instead to connect to us are represented by a seperate server object
		/// internally that listens on a different port.
		/// </summary>
		/// <param name="response">Response from the server that connection was attempted from.</param>
		/// <returns>A ServerPeer base instance for the other server to be managed as or null if for some reason the accepted connection is rejected.</returns>
		protected abstract ServerPeer OnConnectionSuccess(ConnectionResponse response);

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

			//If this is successful we should create a Lidgren message converter based on this serializer
			if (this.SerializerRegister.Register(Serializer<T>.Instance, Serializer<T>.Instance.SerializerUniqueKey))
			{
				return this.HighlevelMessageConverter.Register(new HigherLevelPacketHandler<T>(), Serializer<T>.Instance.SerializerUniqueKey);
			}
			else
				return false;
		}

		/// <summary>
		/// Attempts to connect to another server application from this server core/application.
		/// </summary>
		/// <param name="endPoint"></param>
		/// <param name="applicationName"></param>
		/// <param name="hailMessage"></param>
		/// <returns>Indicates whether the endPoint was valid but not if the connection was successful.</returns>
		public bool ConnectToServer(IPEndPoint endPoint, string applicationName, string hailMessage)
		{
			//This currently all happens on the main thread so don't worry about reconnection attempts occuring at the same time an add/remove might be
			//occurring on the PeerLisenters list.

			if (endPoint == null || applicationName == null || applicationName.Length == 0)
				return false;
			else
			{
				NetPeerConfiguration config = new NetPeerConfiguration(applicationName);

				NetClient serverToServerClient = new NetClient(config);

				serverToServerClient.Connect(endPoint, serverToServerClient.CreateMessage(hailMessage));

				//The NetPeer should be added to a collection so we can service messages from the connected servers.
				PeerListeners.Add(serverToServerClient);
				
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

				//Add in all the custom packets the server uses.
				RegisterPackets(Packet.Register);

				lidgrenServerObj.Start();
				OnStartup();
				//Sets the server to a ready state to begin polling and accepting connections.
				isReady = true;
			}
			catch(Exception e)
			{
				this.ClassLogger.LogError("Exception: " + e.Message + "\n\n" + e.Source + "\n\n" + e.Data + e.StackTrace + "\n\n\n" +
					e.InnerException.Message + e.InnerException.Source + e.InnerException.StackTrace);
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

		public int ListenerCount
		{
			get { return PeerListeners.Count; }
		}

		public void DisconnectListeners(string reason = "Unknown")
		{
			foreach(NetPeer p in PeerListeners)
			{
				p.Shutdown("Server called to disconnect all listeners - Reason: " + reason);
			}

			PeerListeners.Clear();
		}

		//Hide this interface member
		/// <summary>
		/// This method should be called and serviced on the main thread or for Unity3D in a coroutine.
		/// </summary>
		public void Poll()//float inMillisecondsWait = 0.0f)
		{
			while(isReady)
			{
				//Console.WriteLine("Polling");

				if(isListening && PeerListeners.Count != 0)
					//Iterates through the listeners and polls them for packets.
					ListenerMessagePoll();

				//if (lidgrenServerObj.ConnectionsCount > 0)
				MessagePoll(lidgrenServerObj);
			}
			this.InternalOnShutdown();
		}

		private void MessagePoll(NetPeer peer)
		{
			NetIncomingMessage msg = peer.ReadMessage();
			HigherLevelPacket hlmsg = peer.ReadHighlevelMessage();

			//TODO: Refactor
			if (msg != null)
			{
				switch (msg.MessageType)
				{
					case NetIncomingMessageType.ConnectionApproval:
						if (msg.SenderConnection.RemoteHailMessage != null &&
							ExpectedClientHailMessage == msg.SenderConnection.RemoteHailMessage.ReadString())
						{
							msg.SenderConnection.Approve();
							RegisterApprovedConnection(msg.SenderConnection);
						}
						else
							msg.SenderConnection.Disconnect("Hail me.");
						break;
				}

				peer.Recycle(msg);
			}

			if (hlmsg != null)
			{
				ProcessHigherLevelPacket(hlmsg, InConnections[hlmsg.LowLevelMessage.SenderConnection.RemoteUniqueIdentifier]);
				hlmsg.Recycle();
			}
		}

		private void ProcessHigherLevelPacket(HigherLevelPacket packet, Peer passTo)
		{
			if (passTo == null)
				return;

			try
			{
				//TODO: Refactor
				switch ((Packet.OperationType)packet.PacketType)
				{
					case Packet.OperationType.Event:
						EventPackage ePackage = GeneratePackage<EventPackage>(packet, HighlevelMessageConverter.Get(packet.SerializationMethod));
						if(ePackage != null)
							passTo.PackageRecieve(ePackage);
						break;
					case Packet.OperationType.Request:
						RequestPackage rqPackage = GeneratePackage<RequestPackage>(packet, HighlevelMessageConverter.Get(packet.SerializationMethod));
						if (rqPackage != null)
							passTo.PackageRecieve(rqPackage);
						break;
					case Packet.OperationType.Response:
						ResponsePackage rPackage = GeneratePackage<ResponsePackage>(packet, HighlevelMessageConverter.Get(packet.SerializationMethod));
						if (rPackage != null)
							passTo.PackageRecieve(rPackage);
						break;
				}
			}
			catch (NullReferenceException e)
			{
				this.ClassLogger.LogError(typeof(Peer).FullName + " ID: " + packet.LowLevelMessage.SenderConnection.RemoteUniqueIdentifier + " sent packet that we failed to deserialize with method key " +
					packet.SerializationMethod + ".");
				throw new SerializationException(typeof(EventPackage), null, e, "Failed to deserialize for eventpackage, likely due to serializer key: " +
					packet.SerializationMethod + " being unreigstered.");
			}
			
		}

		private T GeneratePackage<T>(HigherLevelPacket packet, IHandler<HigherLevelPacket, T> handler) 
			where T : INetworkPackage
		{
			return handler.Handle(packet);
		}

		private void RegisterApprovedConnection(NetConnection netConnection)
		{
			this.OnAttemptedConnection(new ConnectionRequest(netConnection.RemoteEndPoint, netConnection.RemoteUniqueIdentifier, netConnection));
		}

		private void ListenerMessagePoll()
		{
			for(int i = 0; i < PeerListeners.Count; i++)
			{
				NetIncomingMessage msg = PeerListeners[i].ReadMessage();
				HigherLevelPacket hlmsg = PeerListeners[i].ReadHighlevelMessage();

				if (msg != null)
				{
					switch (msg.MessageType)
					{
						//We only need to handle ConnectionApproval to vefify the connection's acceptance on the remote endpoint.
						case NetIncomingMessageType.ConnectionApproval:
							//TODO: Call OnConnectionSuccess and create the peer and add it to PeerDictionary or how ever it's going to be handled

							break;
						case NetIncomingMessageType.StatusChanged:
							switch ((NetConnectionStatus)msg.ReadByte())
							{
								//The only case that needs to be handled.
								case NetConnectionStatus.Disconnected:
									PeerListeners.RemoveAt(i);
									break;
							}
							break;
						case NetIncomingMessageType.Data:
							//This should be hit if we're sending a custom data package through Lidgren under the hood
							//but at the same time this is not a highlevel package.
							break;
					}
					PeerListeners[i].Recycle(msg);
				}
				//For GC reduction we can recycle the method. This is handled by Lidgren

				if(hlmsg != null)
				{
					ProcessHigherLevelPacket(hlmsg, _OutConnections[PeerListeners[i]]);
					hlmsg.Recycle();
				}
			}
		}
		#endregion
	}
}
