using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkPackage
	{
		bool wasEncrypted { get; }
		byte Code { get; }
		Packet PacketObject { get; }
	}
}
