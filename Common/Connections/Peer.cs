#region copyright
/// GladNet Copyright (C) 2014 X 
/// X
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GladNet.Common
{
	public abstract class Peer
	{
		//TODO: Find a way to make Unity handle internals.
#if UNITYDEBUG || UNITYRELEASE
		/// <summary>
		/// Provides a Dictionary/like datastructure to hold encryption object instances.
		/// </summary>
		public readonly EncryptionRegister EncryptionRegister;
#else

		/// <summary>
		/// Provides a Dictionary/like datastructure to hold encryption object instances.
		/// </summary>
		internal readonly EncryptionRegister EncryptionRegister;
#endif

		public IPEndPoint RemoteConnectionEndpoint { get; private set; }
		public long UniqueConnectionId { get; private set; }

#if !UNITYDEBUG && !UNITYRELEASE
		internal NetConnection InternalNetConnection { get; private set; }
#else
		protected NetConnection InternalNetConnection { get; private set; }
#endif

		public virtual bool isConnected
		{
			get { return InternalNetConnection != null && InternalNetConnection.Status == NetConnectionStatus.Connected; }
		}

		public Peer(IConnectionDetails details)
		{
			if(details != null)
				MemberwiseConnectionDetailsCopyToClass(details);

			EncryptionRegister = new EncryptionRegister();
		}

		public Peer(IConnectionDetails details, EncryptionRegister encryptRegister)
		{
			if(details != null)
				MemberwiseConnectionDetailsCopyToClass(details);

			EncryptionRegister = encryptRegister;
		}

		protected void SetConnectionDetails(NetConnection connection, IPEndPoint endPoint, long uniqueId)
		{
			UniqueConnectionId = uniqueId;
			InternalNetConnection = connection;
			RemoteConnectionEndpoint = endPoint;
		}

		protected void MemberwiseConnectionDetailsCopyToClass(IConnectionDetails details)
		{
			this.RemoteConnectionEndpoint = details.RemoteConnectionEndpoint;
			this.UniqueConnectionId = details.UniqueConnectionId;
			this.InternalNetConnection = details.InternalNetConnection;
		}

		//internal implict cast to a NetConnection for a peer.
		public static implicit operator NetConnection(Peer p)
		{
			return p.InternalNetConnection;
		}

		public abstract void PackageRecieve(RequestPackage package, MessageInfo info);
		public abstract void PackageRecieve(ResponsePackage package, MessageInfo info);
		public abstract void PackageRecieve(EventPackage package, MessageInfo info);

		public virtual void Disconnect()
		{
			if(InternalNetConnection != null)
				InternalNetConnection.Disconnect("Disconnecting");

			OnDisconnection();
		}

		protected abstract void OnDisconnection();

		//Unity really hates Internal. Unity I hate you.
		//TODO: Implementation encryption functionality
#if !UNITYDEBUG && !UNITYRELEASE
		internal virtual Packet.SendResult SendMessage(Packet.OperationType type, PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel = 0, byte encrypt = EncryptionBase.NoEncryptionByte, bool isInternal = false)
#else
		public virtual Packet.SendResult SendMessage(Packet.OperationType type, PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel = 0, byte encrypt = EncryptionBase.NoEncryptionByte, bool isInternal = false)
#endif
		{
			try
			{
				LidgrenTransferPacket transferPacket = new LidgrenTransferPacket(type, packet.SerializerKey, packetCode, packet.Serialize());

				if (encrypt != EncryptionBase.NoEncryptionByte)
				{
					EncryptionLidgrenPackage(encrypt, transferPacket);
				}

				//TODO: encryption because it's ready
				byte[] bytes = Serializer<GladNetProtobufNetSerializer>.Instance.Serialize(transferPacket);

				return (Packet.SendResult)this.InternalNetConnection.SendMessage(isInternal, bytes, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel);
			}
			catch (LoggableException e)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new LoggableException("Exception occured in serialization of packet.", e, LogType.Error);
			}
		}

		//This does not need to be in a unity peer. A Unity peer cannot broadcast.
#if !UNITYDEBUG && !UNITYRELEASE
		//TODO: One day we will need to optimize the ability to broadcast messages as we'll have to convert to a NetConnection list at some point when it's being called externally through
		//the exposed API of GladNet.
		protected void BroadcastEvent(IList<Peer> connections, PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = EncryptionBase.NoEncryptionByte, int channel = 0)
		{
			try
			{
				LidgrenTransferPacket transferPacket = new LidgrenTransferPacket(Packet.OperationType.Event, packet.SerializerKey, packetCode, packet.Serialize());

				if (encrypt != EncryptionBase.NoEncryptionByte)
				{
					EncryptionLidgrenPackage(encrypt, transferPacket);
				}

				//TODO: Encryption because it's ready
				byte[] bytes = Serializer<GladNetProtobufNetSerializer>.Instance.Serialize(transferPacket);


				//Inefficient O(n) casting to a NetConnection list. Not good.
				this.InternalNetConnection.Peer.SendMessage(false, connections.Select(x => x.InternalNetConnection).ToList(), bytes, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel);
			}
			catch(LoggableException e)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new LoggableException("Exception occured in serialization of packet.", e, LogType.Error);
			}
		}
#endif

		private void EncryptionLidgrenPackage(byte encrypt, LidgrenTransferPacket packet)
		{
				if (EncryptionRegister.HasKey(encrypt))
					packet.Encrypt(EncryptionRegister[encrypt]);
				else
					throw new LoggableException("Failed to encrypt package for Peer ID: "
						+ this.UniqueConnectionId + " With encryption ByteType: " + encrypt, null, LogType.Error);
		}
	}
}
