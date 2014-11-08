using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	[PacketAttribute(2)]
	[ProtoContract]
	public class EmptyPacket : Packet
	{
		public EmptyPacket()
			: base()
		{

		}
	}

	public class MalformedPacket : Packet
	{
		public bool isMalformed { get; private set; }

		public MalformedPacket()
		{
			isMalformed = true;
		}
	}
}
