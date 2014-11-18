using GladNet.Common;
using GladNet.Server;
using GladNet.Server.Connections.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
	public class TestClient : ClientPeer
	{
		public TestClient(IConnectionDetails details)
			: base(details)
		{

		}

		public override void PackageRecieve(RequestPackage package)
		{
			switch(package.Code)
			{
				case 5:
					MessagePacket message = package.PacketObject as MessagePacket;

					if (message != null)
						AsyncConsoleLogger.Instance.LogDebug(message.GetMessage());

					this.SendResponse(new ResponsePacket("Okie dokie"), 5, Packet.DeliveryMethod.ReliableUnordered);
					break;			
			}
		}

		public override void OnDisconnection()
		{
			AsyncConsoleLogger.Instance.LogDebug("Disconnected a client.");
		}
	}
}
