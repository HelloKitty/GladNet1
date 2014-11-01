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
	public abstract class Peer : IConnectionDetails
	{
		#region IConnectionDetails Implementation
		public IPEndPoint RemoteConnectionEndpoint { get; private set; }
		public long UniqueConnectionId { get; private set; }
		public NetConnection InternalNetConnection { get; private set; }
		#endregion

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

		//TODO: Implementation encryption functionality
		internal Packet.SendResult SendMessage<T>(Packet.OperationType type, Packet<T> packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
			where T : SerializerBase
		{
			byte[] bytes = packet.Serialize();
			return (Packet.SendResult)InternalNetConnection.SendMessage(false, bytes, (byte)type, Serializer<T>.Instance.SerializerUniqueKey, packetCode,
				Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel, encrypt);
		}

		/*
		internal void BroadcastMessage<T>(Packet.OperationType type, Packet<T> packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
			where T : SerializerBase
		{
			BroadcastMessage(type, (List<NetConnection>)this.Peers, packet, packetCode, deliveryMethod, encrypt, channel);
		}*/

		//TODO: One day we will need to optimize the ability to broadcast messages as we'll have to convert to a NetConnection list at some point when it's being called externally through
		//the exposed API of GladNet.
		public void BroadcastEvent<T>(IList<Peer> connections, Packet<T> packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
			where T : SerializerBase
		{
			byte[] bytes = packet.Serialize();
			//Inefficient O(n) casting to a NetConnection list. Not good.
			this.InternalNetConnection.Peer.SendMessage
				(false, connections.Select(x => x.InternalNetConnection).ToList(), bytes, (byte)Packet.OperationType.Event, Serializer<T>.Instance.SerializerUniqueKey, packetCode, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel, encrypt);
		}
	}
}
