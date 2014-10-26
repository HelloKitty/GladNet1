using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class RequestPackage : INetworkPackage
	{
		public bool wasEncrypted { get; private set; }

		public byte Code { get; private set; }

		public Packet PacketObject { get; private set; }

		public RequestPackage(Packet packet, byte code, bool encrypted)
		{
			PacketObject = packet;
			Code = code;
			wasEncrypted = encrypted;
		}
	}
}
