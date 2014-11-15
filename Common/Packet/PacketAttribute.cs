using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class PacketAttribute : Attribute
	{
		public readonly int UniquePacketKey;

		public PacketAttribute(int uniqueKey)
		{
			UniquePacketKey = uniqueKey;
		}
	}
}
