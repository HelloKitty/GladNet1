#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
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
using System.Threading.Tasks;

namespace GladNet.Server
{
	public abstract class Peer
	{
		public IPEndPoint RemoteConnectionEndpoint { get; private set; }
		public long UniqueConnectionId { get; private set; }
		internal NetConnection InternalNetConnection { get; private set; }

		public bool isConnected
		{
			get { return InternalNetConnection.Status == NetConnectionStatus.Connected; }
		}

		public Peer(IConnectionDetails details)
		{
			MemberwiseConnectionDetailsCopyToClass(details);
		}

		private void MemberwiseConnectionDetailsCopyToClass(IConnectionDetails details)
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

		public abstract void PackageRecieve(RequestPackage package);
		public abstract void PackageRecieve(ResponsePackage package);
		public abstract void PackageRecieve(EventPackage package);


		internal void InternalOnDisconnection()
		{
			InternalNetConnection.Disconnect("Disconnecting");
			OnDisconnection();
		}

		public abstract void OnDisconnection();

		//TODO: Implementation encryption functionality
		internal Packet.SendResult SendMessage(Packet.OperationType type, PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
		{
			try
			{
				LidgrenTransferPacket transferPacket = new LidgrenTransferPacket(type, encrypt, packet.SerializerKey, packetCode, packet.Serialize());

				byte[] bytes = Serializer<ProtobufNetSerializer>.Instance.Serialize(transferPacket);

				return (Packet.SendResult)this.InternalNetConnection.SendMessage(false, bytes, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel);
			}
			catch(Exception e)
			{
				throw new LoggableException("Exception occured in serialization of packet.", e, Logger.LogType.Error);
			}
		}

		//TODO: One day we will need to optimize the ability to broadcast messages as we'll have to convert to a NetConnection list at some point when it's being called externally through
		//the exposed API of GladNet.
		public void BroadcastEvent(IList<Peer> connections, PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
		{
			try
			{
				LidgrenTransferPacket transferPacket = new LidgrenTransferPacket(Packet.OperationType.Event, encrypt, packet.SerializerKey, packetCode, packet.Serialize());

				byte[] bytes = Serializer<ProtobufNetSerializer>.Instance.Serialize(transferPacket);


				//Inefficient O(n) casting to a NetConnection list. Not good.
				this.InternalNetConnection.Peer.SendMessage(false, connections.Select(x => x.InternalNetConnection).ToList(), bytes, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel);
			}
			catch(Exception e)
			{
				throw new LoggableException("Exception occured in serialization of packet.", e, Logger.LogType.Error);
			}
		}
	}
}
