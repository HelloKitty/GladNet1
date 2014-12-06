using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	[PacketAttribute(3)]
	[ProtoContract]
	public class EncryptionRequest : Packet
	{
		[ProtoMember(1)]
		public byte EncryptionByteType { get; private set; }

		[ProtoMember(2)]
		public byte[] EncryptionInitInfo { get; private set; }

		public EncryptionRequest(byte encryptionType, byte[] encryptInfo)
		{
			EncryptionByteType = encryptionType;
			EncryptionInitInfo = encryptInfo;
		}

		/// <summary>
		/// Protobuf-net constructor
		/// </summary>
		public EncryptionRequest()
		{

		}
	}
}
