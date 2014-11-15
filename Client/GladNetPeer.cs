using Common.Exceptions;
using GladNet.Common;
using GladNet.Server.Logging;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace GladNet.Client
{
	public sealed class GladNetPeer : MessageReciever, ILoggable
	{
		#region Package Action Queue
		private object networkIncomingEnqueueSyncObj;
		//TODO: Explore the GC pressure that a queue of Actions, with lambdas creating them, causes.
		private Queue<Action> networkPackageQueue;
		#endregion

		#region Lidgren Network Objects
		private NetClient internalLidgrenClient;
		private NetConnection internalNetConnection;
		#endregion

		public Logger ClassLogger { get; private set; }

		private Thread networkThread;

		private IListener RecieverListener;

		private volatile bool _isConnected;

		public bool isConnected
		{
			get { return _isConnected; }
		}

#if UNITYDEBUG || UNITYRELEASE
		public GladNetPeer(IListener listener, Logger logger = null)
		{
			RecieverListener = listener;
			//Call the interface method to register the packets.
			RegisterProtobufPackets(Packet.Register);

			//This registers the default serializer
			this.SerializerRegister.Register(Serializer<ProtobufNetSerializer>.Instance, Serializer<ProtobufNetSerializer>.Instance.SerializerUniqueKey);

			ClassLogger = logger == null ? new UnityLogger(Logger.LogType.Debug) : logger;
			networkPackageQueue = new Queue<Action>(20);
			networkIncomingEnqueueSyncObj = new object();
			internalLidgrenClient = null;
			_isConnected = false;
		}
#else
		public GladNetPeer(IListener listener, Logger logger)
		{
			RecieverListener = listener;
			//Call the interface method to register the packets.
			RegisterProtobufPackets(Packet.Register);

			ClassLogger = logger;

			//This registers the default serializer
			this.SerializerRegister.Register(Serializer<ProtobufNetSerializer>.Instance, Serializer<ProtobufNetSerializer>.Instance.SerializerUniqueKey);

			networkPackageQueue = new Queue<Action>(20);
			networkIncomingEnqueueSyncObj = new object();
			internalLidgrenClient = null;
			_isConnected = false;
		}
#endif

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool Poll()
		{
			lock (this.networkIncomingEnqueueSyncObj)
			{
				while (networkPackageQueue.Count != 0)
				{
					//Invokes the underlying Action delegate contained in the queue
					//We don't test nullness for preformance and because it should NEVER be null
					//A crash is likely to happen elsewhere on the logical chain leading to this being null.
					networkPackageQueue.Dequeue()();
				}
			}

			if (_isConnected == false)
			{
				RecieverListener.OnStatusChange(StatusChange.Disconnected);
				return false;
			}

			return true;
		}


		public bool Connect(string ip, int port, string hailMessage, string appName)
		{
			if (isConnected)
				this.Disconnect();

			NetPeerConfiguration config = new NetPeerConfiguration(appName);
			config.AcceptIncomingConnections = false;

			if (ip == null || appName == null || ip.Length == 0)
			{
				ClassLogger.LogError("Connection to remote host must have a valid appname and IP address.");
#if UNITYDEBUG || UNITYRELEASE
				return false;
#else
				throw new NullReferenceException("Connection to remote host must have a valid appname and IP address.");
#endif
			}	

			//This should reduce GC which is always terrible for Unity.
			config.UseMessageRecycling = true;

			internalLidgrenClient = new NetClient(config);
			internalLidgrenClient.Start();

			NetOutgoingMessage msg = GenerateClientHail(hailMessage);

			internalNetConnection = internalLidgrenClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port), msg);
			_isConnected = true;

			return true;
		}

		public void StartListener()
		{
			if(networkThread != null)
			{
				ClassLogger.LogError("Attempted to start listener while listener is spinning.");
#if !UNITYDEBUG && !UNITYRELEASE
				//TODO: Better exception throwing
				throw new Exception("Attempted to start listener while listener is spinning.");
#endif
			}

			//If we hit this point we need to make a network thread to poll the lidgren client and process incoming client data.
			networkThread = new Thread(new ThreadStart(NetworkListenerThreadMethod));
			networkThread.Start();
		}

		private void NetworkListenerThreadMethod()
		{
#if UNITYDEBUG || DEBUG
			ClassLogger.LogDebug("Started network thread.");
#endif


			if(internalLidgrenClient == null || internalLidgrenClient == null)
			{
				ClassLogger.LogError("Cannot start listening before connecting.");

#if !UNITYDEBUG && !UNITYRELEASE
				ClassLogger.LogError("Cannot start listening before connecting.");
				throw new NullReferenceException("Cannot start listening before connecting. Internally a client object is null.");
#endif
			}

			NetIncomingMessage msg;

			try
			{
				while (_isConnected)
				{
					msg = internalLidgrenClient.WaitMessage(10);

					if (msg != null)
					{
						try
						{
							ServiceLidgrenMessage(msg);

							//Recycling the message reduces GC which can be make or break for Unity.
							this.internalLidgrenClient.Recycle(msg);
						}
						//We can catch nullreferences here without affecting exceptions external to the library
						//This is because we have a Queue<Action> and thus we can't possibly catch the exception coming from the main thread via the lambda generated Action instance.
						catch (NullReferenceException e)
						{
							ClassLogger.LogError("Error occurred during polling: " + e.Message);
							//If we hit this point it generally means that the object has gone out of scope for some reason without disconnecting (usually in Unity going from playmode
							//to the editor so at this point we should just catch it and let the application and thread die.
							_isConnected = false;
						}
					}
				}
			}
			catch(Exception e)
			{
				ClassLogger.LogError(e.Message + e.Data);
				throw;
			}

			networkThread = null;
		}

		private void ServiceLidgrenMessage(NetIncomingMessage msg)
		{
			if (msg == null)
				return;

			switch(msg.MessageType)
			{
				case NetIncomingMessageType.StatusChanged:
					try
					{
						HandleStatusChange((NetConnectionStatus)msg.ReadByte());
					}
					catch(NetException e)
					{
#if UNITYDEBUG
						ClassLogger.LogError("Malformed packet recieved. Packet indicated that it was a status change but had no info.");
#else
						//TODO: What shall we do when the packet is malformed here?
#endif
					}
					catch(LoggableException e)
					{
						//Checking this because it can cause some nasty GC to make these string adds.
						if (ClassLogger.isStateEnabled(Logger.LogType.Debug))
							ClassLogger.LogDebug(e.Message + " Inner: " + e.InnerException != null ? e.InnerException.Message : "");
					}
					break;

				case NetIncomingMessageType.ExternalHighlevelMessage:
					try
					{
						HandleExternalHighLevelMessage(msg);
					}
					catch(LoggableException e)
					{
						//Checking this because it can cause some nasty GC to make these string adds.
						if (ClassLogger.isStateEnabled(Logger.LogType.Debug))
							ClassLogger.LogDebug(e.Message + " Inner: " + e.InnerException != null ? e.InnerException.Message : "");
					}
					break;
			}
		}

		private void HandleExternalHighLevelMessage(NetIncomingMessage msg)
		{
			//TODO: Investigate GC pressure of this byte[] generation. It could be hairy in Unity.
			LidgrenTransferPacket transferPacket = GenerateTransferPacket(msg.ReadBytes(msg.LengthBytes - msg.PositionInBytes));

			if(transferPacket == null)
			{
				ClassLogger.LogDebug("Recieved a null transfer packet for highlevel message response. This may be the unlikely scenario of package corruption.");
				return;
			}

			try
			{
				switch (transferPacket.OperationType)
				{
					case Packet.OperationType.Event:
						EventPackage ePackage = this.Converter.BuildIncomingNetPackage<EventPackage>(transferPacket, SerializerRegister[transferPacket.SerializerKey]);

						if (ePackage != null)
							this.RecieverListener.RecievePackage(ePackage);
#if DEBUG || UNITYDEBUG
						else
							ClassLogger.LogError("Failed to create EventPackage.");
#endif
						break;

					case Packet.OperationType.Response:
						ResponsePackage rPackage = this.Converter.BuildIncomingNetPackage<ResponsePackage>(transferPacket, SerializerRegister[transferPacket.SerializerKey]);

						if (rPackage != null)
							this.RecieverListener.RecievePackage(rPackage);
#if DEBUG || UNITYDEBUG
						else
							ClassLogger.LogError("Failed to create ResponsePackage.");
#endif
						break;

					default:
						//Malicious party could send a fake package. Don't crash the app just for that.
						//Plus it could just be data corruption. Nothing can be done.
#if DEBUG || UNITYDEBUG
						ClassLogger.LogError("Recieved an unhandlable OperationType for a packet. Type: " + transferPacket.OperationType.ToString());
#endif
						break;
				}
			}
			catch(NullReferenceException e)
			{
			//Malicious party could send a fake package. Don't crash the app just for that.
			//Plus it could just be data corruption. Nothing can be done.
#if DEBUG || UNITYDEBUG
				ClassLogger.LogError("Recieved a packet from server with SerializationKey: {0} which has not registered serializer.", transferPacket.SerializerKey);
#else
				ClassLogger.LogDebug("Recieved a packet from server with SerializationKey: {0} which has not registered serializer.", transferPacket.SerializerKey);
#endif
			}
		}

		private LidgrenTransferPacket GenerateTransferPacket(byte[] bytes)
		{
			try
			{
				return Serializer<ProtobufNetSerializer>.Instance.Deserialize<LidgrenTransferPacket>(bytes);
			}
			catch (LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? e.InnerException.Message : "");
				return null;
			}
		}

		private void HandleStatusChange(NetConnectionStatus status)
		{
			switch(status)
			{
				case NetConnectionStatus.Connected:
					QueueStatusChange(StatusChange.Connected);
					break;
				case NetConnectionStatus.Disconnected:
					//We need to let the main thread modify the value of _isConnected so that the final status change message will be polled.
					networkPackageQueue.Enqueue(() => { _isConnected = false; });
					//QueueStatusChange(StatusChange.Disconnected);
					break;
				case NetConnectionStatus.InitiatedConnect:
					QueueStatusChange(StatusChange.Connecting);
					break;
			}
		}

		private void QueueStatusChange(StatusChange change)
		{
			lock (networkIncomingEnqueueSyncObj)
			{
				networkPackageQueue.Enqueue(() => this.RecieverListener.OnStatusChange(change));
			}
		}

		private NetOutgoingMessage GenerateClientHail(string hail)
		{
			if (internalLidgrenClient != null)
			{
				NetOutgoingMessage msg = internalLidgrenClient.CreateMessage();

				//This indicates we're a pure client connection. It is a reserved connection type value.
				msg.Write(hail);
				msg.Write((byte)0);

				return msg;
			}
			else
			{
#if UNITYDEBUG || UNITYRELEASE
				ClassLogger.LogError("Internal lidgren client is null. Do not invoke HailMessageGeneration via reflection.");
				return null;
#else
				throw new NullReferenceException("internalLidgrenClient is null for some reason.");
#endif
			}
		}

		public void Disconnect()
		{
			_isConnected = false;


			//Lock around the Lidgren disconnection too so we can send a disconnected status change manually instead of letting lidgren do it.
			lock (networkIncomingEnqueueSyncObj)
			{
				if (internalLidgrenClient != null)
					//Should be thread safe and fine to do.
					internalLidgrenClient.Disconnect("Disconnecting");
				networkPackageQueue.Clear();
			}	
		}

//If we're not in unity there is no reason for a deconstructor
#if UNITYDEBUG || UNITYRELEASE
		//A deconstructor in C#? I too like to live dangerously...
		//Why is this here? This exists to stop the network thread from spinnning
		//in cases where the user leaves playmode in the Unity3D editor before disconnecting.
		~GladNetPeer()
		{

			_isConnected = false;
			if (networkThread != null)
				try
				{
					networkThread.Abort();
				}
				catch(Exception e)
				{
					//Catch anything and everything because we're in the editor and all bets are off
				}

		}
#endif

		protected override void RegisterProtobufPackets(Func<Type, bool> registerAsDefaultFunc)
		{
			if (RecieverListener == null)
			{
				ClassLogger.LogError("The IListener instance passed in on connection is a null reference.");
				return;
			}

			try
			{
				//Registering the empty packet
				Packet.Register(typeof(EmptyPacket), true);

				this.RecieverListener.RegisterProtobufPackets(registerAsDefaultFunc);
				Packet.SetupProtoRuntimePacketInheritance();
				Packet.LockInProtobufnet();
			}
			catch(LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? e.InnerException.Message : "");
			}
		}
		
		//TODO: Implementation encryption functionality
		public Packet.SendResult SendRequest(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
		{
			try
			{
				LidgrenTransferPacket transferPacket = 
					new LidgrenTransferPacket(Packet.OperationType.Request, encrypt, packet.SerializerKey, packetCode, packet.Serialize());

				byte[] bytes = Serializer<ProtobufNetSerializer>.Instance.Serialize(transferPacket);

				NetOutgoingMessage msg = this.internalLidgrenClient.CreateMessage(bytes.Length + 1);

				msg.Write((byte)255);
				msg.Write(bytes);

				return (Packet.SendResult)this.internalLidgrenClient.SendMessage(msg, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel);
			}
			catch(Exception e)
			{
				ClassLogger.LogError("SendRequest failed to execute. This could be an issue with serialization or may be the result of a null packet being sent." +
					e.Message + (e.InnerException == null ? "" : e.InnerException.Message));
				//It is ok to crash the application since this should only ever be the result of poor compile-time logic. No malicious party could cause this.
				throw;
			}
		}
	}
}
