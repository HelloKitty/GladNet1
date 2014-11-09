using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GladNet.Client
{
	public class GladNetPeer
	{
		#region Package Action Queue
		private object networkIncomingEnqueueSyncObj;
		private Queue<Action> networkPackageQueue;
		#endregion

		#region Lidgren Network Objects
		internal NetClient internalLidgrenClient;
		internal NetConnection internalNetConnection;
		#endregion

		private Thread networkThread;

		private IListener RecieverListener;

		private volatile bool _isConnected;
		public bool isConnected
		{
			get { return _isConnected; }
		}

		public GladNetPeer()
		{
			networkPackageQueue = new Queue<Action>(20);
			networkIncomingEnqueueSyncObj = new object();
			internalLidgrenClient = null;
			_isConnected = false;
		}

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

			return isConnected;
		}


		public bool Connect(string ip, int port, string hailMessage, string appName, IListener listener)
		{
			if (isConnected)
				this.Disconnect();

			NetPeerConfiguration config = new NetPeerConfiguration(appName);
			config.AcceptIncomingConnections = false;

			if (ip == null || appName == null || ip.Length == 0)
			{
#if UNITYDEBUG || UNITYRELEASE
				Debug.LogError("Connection to remote host must have a valid appname and IP address.");
				return false;
#else
				throw new NullReferenceException("Connection to remote host must have a valid appname and IP address.");
#endif
			}

			RecieverListener = listener;

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
#if UNITYDEBUG || UNITYRELEASE
				Debug.LogError("Attempted to start listener while listener is spinning.");
				return;
#else
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
#if UNITYDEBUG
			Debug.Log("Started network thread.");
#endif


			if(internalLidgrenClient == null || internalLidgrenClient == null)
			{
#if UNITYDEBUG || UNITYRELEASE
				Debug.LogError("Cannot start listening before connecting.");
#else
				throw new NullReferenceException("Cannot start listening before connecting. Internally a client object is null.");
#endif
			}

			NetIncomingMessage msg;
			HighLevelMessage hlmsg;

			while(_isConnected)
			{
				msg = internalLidgrenClient.WaitMessage(100);
				//msg = internalLidgrenClient.ReadMessage();
				hlmsg = internalLidgrenClient.ReadHighlevelMessage();

				ServiceLidgrenMessage(msg);
				ServiceHighLevelMessage(hlmsg);
			}

			networkThread = null;
		}

		private void ServiceHighLevelMessage(HighLevelMessage hlmsg)
		{
			if (hlmsg == null)
				return;
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
						Debug.LogError("Malformed packet recieved. Packet indicated that it was a status change but had no info.");
#else
						//TODO: What shall we do when the packet is malformed here?
#endif
					}
					break;
			}
		}

		private void HandleStatusChange(NetConnectionStatus status)
		{
			switch(status)
			{
				case NetConnectionStatus.Connected:
					lock(networkIncomingEnqueueSyncObj)
					{
						networkPackageQueue.Enqueue(() => this.RecieverListener.OnStatusChange(StatusChange.Connected));
					}
					break;
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
				Debug.LogError("Internal lidgren client is null. Do not invoke HailMessageGeneration via reflection.");
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

		//A deconstructor in C#? I too like to live dangerously...
		//Why is this here? This exists to stop the network thread from spinnning
		//in cases where the user leaves playmode in the Unity3D editor.
		~GladNetPeer()
		{
#if UNITYDEBUG || UNITYRELEASE
			_isConnected = false;
			if (networkThread != null)
				networkThread.Abort();
#endif	
		}
	}
}
