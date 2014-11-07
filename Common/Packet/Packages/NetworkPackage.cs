using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkPackage
	{
		public bool wasEncrypted { get; protected set; }
		public byte Code { get; protected set; }
		public PacketBase PacketObject { get; protected set; }

		public void FillPackage(PacketBase packet, byte code, bool encrypted)
		{
			PacketObject = packet;
			Code = code;
			wasEncrypted = encrypted;
		}
	}
}
