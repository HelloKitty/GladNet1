using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class PacketAttribute : Attribute
	{
		public readonly int UniquePacketKey;

		public PacketAttribute(int uniqueKey)
		{
			UniquePacketKey = uniqueKey;
		}
	}
}
