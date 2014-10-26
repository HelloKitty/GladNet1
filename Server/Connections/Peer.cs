using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
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

		public abstract void PackageRecieve(RequestPackage package);

		//TODO: Implementation encryption functionality
		internal void SendMessage(Packet.OperationType type, Packet packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
		{
			byte[] bytes = packet.Serialize();
			InternalNetConnection.SendMessage(false, bytes, (byte)type, packetCode, Packet.LidgrenDeliveryMethodConvert(deliveryMethod), channel, encrypt);
		}
	}
}
