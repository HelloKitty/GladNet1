using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet
{
	public class HighLevelMessage
	{
		public byte[] PacketBytes { get; private set; }
		public NetIncomingMessage OriginalLidgrenMessage { get; private set; }

		public HighLevelMessage(byte[] bytes, NetIncomingMessage msg)
		{
			PacketBytes = bytes;
			OriginalLidgrenMessage = msg;
		}
	}
}
