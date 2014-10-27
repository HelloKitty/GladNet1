using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class EmptyPacket : Packet
	{

		public EmptyPacket(bool malformed) 
			: base(malformed)
		{

		}

		public EmptyPacket()
			: base(false)
		{

		}
	}
}
