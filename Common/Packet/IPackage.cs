using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IPackage
	{
		Packet.OperationType OperationType { get; }
		byte PacketCode { get; }

		byte[] InternalByteRepresentation { get; }

		byte SerializerKey { get; }

	}
}
