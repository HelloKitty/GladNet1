using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class EventPackage : INetworkPackage
	{
		public bool wasEncrypted { get; private set; }

		public byte Code { get; private set; }

		public Packet PacketObject { get; private set; }

		public EventPackage(Packet packet, byte code, bool encrypted)
		{
			PacketObject = packet;
			Code = code;
			wasEncrypted = encrypted;
		}
	}
}
