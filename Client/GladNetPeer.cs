﻿#region copyright
/// GladNet Copyright (C) 2014 X 
/// X
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
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
	public sealed class GladNetPeer : Peer, ILoggable
	{
		#region Package Action Queue
		private readonly object networkIncomingEnqueueSyncObj;
		//TODO: Explore the GC pressure that a queue of Actions, with lambdas creating them, causes.
		private readonly Queue<Action> networkPackageQueue;
		#endregion

		#region Lidgren Network Objects
		private NetClient internalLidgrenClient;
		#endregion

		public ILogger ClassLogger { get; private set; }

		private Thread networkThread;

		private IListener RecieverListener;

		private volatile bool _isConnected;

		private readonly ClientPacketHandler NetworkMessageHandler;

		private double timeNowByPoll;
		private double timeNowByPollComparer;

		public override bool isConnected
		{
			get
			{
				return _isConnected;
			}
		}

#if UNITYDEBUG || UNITYRELEASE
		public GladNetPeer(IListener listener, ILogger logger = null)
			: base(null)
		{
			timeNowByPoll = 0;
			timeNowByPollComparer = 0;

			ClassLogger = (logger == null ? new UnityLogger(LogType.Debug) : logger);

			NetworkMessageHandler = new ClientPacketHandler(ClassLogger);

			RecieverListener = listener;
			//Call the interface method to register the packets.
			RegisterProtobufPackets(Packet.Register);

			//This registers the default serializer
			NetworkMessageHandler.Register<GladNetProtobufNetSerializer>();


			networkPackageQueue = new Queue<Action>(20);
			networkIncomingEnqueueSyncObj = new object();
			internalLidgrenClient = null;
			_isConnected = false;
		}
#else
		public GladNetPeer(IListener listener, ILogger logger) 
			: base(null)
		{
			timeNowByPoll = 0;
			timeNowByPollComparer = 0;

			ClassLogger = logger;
			NetworkMessageHandler = new ClientPacketHandler(logger);

			RecieverListener = listener;
			//Call the interface method to register the packets.
			RegisterProtobufPackets(Packet.Register);

			//This registers the default serializer
			NetworkMessageHandler.Register<GladNetProtobufNetSerializer>();

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
			//We should do this as soon as Poll is called since the network thread will stop if we hold off too long
			//Sets the NetTime for the network thread to read. If it's much less then NetTime.Now then the network thread shuts down
			//This will happen if we don't poll often.
			Interlocked.Exchange(ref timeNowByPoll, NetTime.Now);

			Action action;
			int count = networkPackageQueue.Count;

			//We're the only ones touching the networkPackageQueue other than the producer so it shouldn't ever be
			//consumed while we dequeue it.
			for (int i = 0; i < count; i++)
			{
				lock (this.networkIncomingEnqueueSyncObj)
				{
					//For less locking, and for not holding the lock ALL THE WAY DOWN THE STACK like before we invoke it outside the lock down below.
					action = networkPackageQueue.Dequeue();
				}

				//We don't test nullness for preformance and because it should NEVER be null
				action();

				//For the time being we do not want to catch exceptions generated from the delegate.
				/*try
				{
					action();
				}
				catch(NullReferenceException e)
				{
					this.ClassLogger.LogError("Error in poll. Action from Queue was null: " + e.Message);
				}*/
			}


			if (_isConnected == false)
			{
				if (RecieverListener != null)
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

			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

			NetConnection connection = internalLidgrenClient.Connect(endPoint, msg);

			this.SetConnectionDetails(connection, endPoint, connection.RemoteUniqueIdentifier);

			_isConnected = true;

			Interlocked.Exchange(ref timeNowByPoll, NetTime.Now < 3 ? 0 : NetTime.Now);
			Interlocked.Exchange(ref timeNowByPollComparer, NetTime.Now < 3 ? 0 : NetTime.Now);

			return true;
		}

		public Packet.SendResult SendRequest(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel = 0, bool encrypt = false)
		{
			return this.SendRequest(packet, packetCode, deliveryMethod, channel, encrypt ? (byte)1 : (byte)0);
		}

		public Packet.SendResult SendRequest(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel, byte encrypt)
		{
			try
			{
				return this.SendMessage(Packet.OperationType.Request, packet, packetCode, deliveryMethod, channel, encrypt);
			}
			catch (LoggableException e)
			{
				QueueStatusChange(StatusChange.NetworkSendError);
				throw;
			}
		}

		public void StartListener()
		{
			if (networkThread != null)
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


			if (internalLidgrenClient == null || internalLidgrenClient == null)
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
					msg = internalLidgrenClient.WaitMessage(20);

					//TODO: May not be thread safe due to NetTime.Now
					//This checks to see if Poll was called in the last 3 seconds. If it was not it stops the network thread basically.
					if (Interlocked.Exchange(ref timeNowByPollComparer, timeNowByPoll) < NetTime.Now - 3)
					{
#if UNITYDEBUG || DEBUG
						ClassLogger.LogDebug("Stopping network thread.");
#endif
						_isConnected = false;
						//Should be thread safe and fine to do.
						this.Disconnect();
					}

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
							ClassLogger.LogError("Error occurred during polling: " + e.Message + " StackTrace: " + e.StackTrace + " Source: " + e.Source);
							//If we hit this point it generally means that the object has gone out of scope for some reason without disconnecting (usually in Unity going from playmode
							//to the editor so at this point we should just catch it and let the application and thread die.
							_isConnected = false;
						}
					}
				}
			}
			catch (LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? "Inner: " + e.InnerException : "");
			}
			catch (Exception e)
			{
				ClassLogger.LogError(e.Message + e.Data);
				throw;
			}

			Disconnect();

			networkThread = null;
		}

		#region Encryption Methods
		/// <summary>
		/// Attempts to initialize the default encryption method between the server and client
		/// for secure communications.
		/// </summary>
		/// <returns>Indicates if the client requested to establish via a networked message. Does not indicate success.</returns>
		public bool InitializeEncryption()
		{
			return this.Register(new DiffieHellmanAESEncryptor());
		}

		//This is not needed by the client
		/// <summary>
		/// Registers an encryption object. This will request that the encryption be established by sending a message to the server.
		/// Do not try to register an encryption object if you are not connected.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="encryptorInstance"></param>
		/// <returns></returns>
		public bool Register<T>(T encryptorInstance) where T : EncryptionBase
		{
			if (encryptorInstance == null)
			{
				ClassLogger.LogError("Cannot register a null encryption object.");
				return false;
			}

			if (this.EncryptionRegister.HasKey(encryptorInstance.EncryptionTypeByte))
			{
				ClassLogger.LogError("Tried to register an already known encryption object.");
				return false;
			}

			if (!isConnected || RecieverListener == null)
			{
				ClassLogger.LogError("Cannot register encryption objects when not connected.");
				return false;
			}

			//Set the callback for when the server acknowledges our encryption request.
			encryptorInstance.OnEstablished += () =>
			{
				lock (networkIncomingEnqueueSyncObj)
				{
					this.networkPackageQueue.Enqueue(() => { RecieverListener.OnStatusChange(StatusChange.EncryptionEstablished); });
				}
			};

			this.EncryptionRegister.Register(encryptorInstance, encryptorInstance.EncryptionTypeByte);

			PacketBase packet = new EncryptionRequest(encryptorInstance.EncryptionTypeByte, encryptorInstance.NetworkInitRequiredData());

			return this.SendMessage(Packet.OperationType.Request, packet, (byte)InternalPacketCode.EncryptionRequest,
				Packet.DeliveryMethod.ReliableUnordered, 0, 0, true) != Packet.SendResult.FailedNotConnected;
		}
		#endregion

		/// <summary>
		/// Registers a serializer, a custom serializer, with the network message handler. This will allow for you to
		/// register serializers other than Protobuf-net
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool Register<T>() where T : SerializerBase
		{
			return this.NetworkMessageHandler.Register<T>();
		}

		private void ServiceLidgrenMessage(NetIncomingMessage msg)
		{
			if (msg == null)
				return;

			switch (msg.MessageType)
			{
				case NetIncomingMessageType.StatusChanged:
					try
					{
						HandleStatusChange((NetConnectionStatus)msg.ReadByte());
					}
					catch (NetException e)
					{
#if UNITYDEBUG
						ClassLogger.LogError("Malformed packet recieved. Packet indicated that it was a status change but had no info.");
#else
						//TODO: What shall we do when the packet is malformed here?
#endif
					}
					catch (LoggableException e)
					{
						//Checking this because it can cause some nasty GC to make these string adds.
						if (ClassLogger.isStateEnabled(LogType.Debug))
							ClassLogger.LogDebug(e.Message + " Inner: " + e.InnerException != null ? e.InnerException.Message : "");
					}
					break;
				//We can take advantage of using the same logic for both cases.
				//All that changes is we till the handler it is an internal message for a Data message type.
				case NetIncomingMessageType.ExternalHighlevelMessage:
				case NetIncomingMessageType.Data:
					try
					{
						this.NetworkMessageHandler.DispatchMessage(this, msg, msg.MessageType == NetIncomingMessageType.Data);
					}
					catch (LoggableException e)
					{
						//Checking this because it can cause some nasty GC to make these string adds.
						if (ClassLogger.isStateEnabled(LogType.Debug))
							ClassLogger.LogDebug(e.Message + " Inner: " + e.InnerException != null ? e.InnerException.Message : "");
					}
					break;
			}
		}

		private void HandleStatusChange(NetConnectionStatus status)
		{
			switch (status)
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
				if (RecieverListener != null)
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

		public override void Disconnect()
		{
			_isConnected = false;


			//Lock around the Lidgren disconnection too so we can send a disconnected status change manually instead of letting lidgren do it.
			lock (networkIncomingEnqueueSyncObj)
			{
				this.OnDisconnection();
				networkPackageQueue.Clear();
			}
		}

		private void RegisterProtobufPackets(Func<Type, bool> registerAsDefaultFunc)
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
				Packet.Register(typeof(EncryptionRequest), true);

				Packet.SetupProtoRuntimePacketInheritance();
				this.RecieverListener.RegisterProtobufPackets(registerAsDefaultFunc);
				Packet.LockInProtobufnet();
			}
			catch (LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? e.InnerException.Message : "");
				throw;
			}
		}

		public bool RegisterProtobufPacket(Type t)
		{
			return Packet.Register(t, false);
		}

		#region Peer Implementation
		public override void PackageRecieve(RequestPackage package, MessageInfo info)
		{
			throw new LoggableException("Recieved a request package but a client cannot handle such a package.", null, LogType.Error);
		}

		public override void PackageRecieve(ResponsePackage package, MessageInfo info)
		{
			lock (networkIncomingEnqueueSyncObj)
			{
				if (RecieverListener != null)
					networkPackageQueue.Enqueue(() => { RecieverListener.RecievePackage(package, info); });
			}
		}

		public override void PackageRecieve(EventPackage package, MessageInfo info)
		{
			lock (networkIncomingEnqueueSyncObj)
			{
				if (RecieverListener != null)
					networkPackageQueue.Enqueue(() => { RecieverListener.RecievePackage(package, info); });
			}
		}

		protected override void OnDisconnection()
		{

			if (InternalNetConnection != null)
			{
				InternalNetConnection.Disconnect("disconnect");
			}

			if (internalLidgrenClient != null)
			{
				internalLidgrenClient.Disconnect("disconnect.");
			}


			//We need to clear the register so on reconnects we don't try to re-add or re-use old stale encryption objects.
			this.EncryptionRegister.Clear();
		}

		#endregion
	}
}