using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	[ProtoContract]
	internal class LidgrenTransferPacket
	{
		[ProtoMember(1, IsRequired = true)]
		private byte[] internalHighLevelMessageRepresentation;

		[ProtoMember(2)]
		public Packet.OperationType OperationType { get; private set; }

		[ProtoMember(3)]
		public byte EncryptionMethod { get; private set; }

		[ProtoMember(4)]
		public byte SerializerKey { get; private set; }

		[ProtoMember(5)]
		public byte PacketCode { get; private set; }

		public bool wasEncrypted
		{
			get { return EncryptionMethod != 0; }
		}

		public bool isDecrypted { get; private set;}

		public LidgrenTransferPacket(Packet.OperationType opType, byte encryptionMethod, byte serializationKey, byte packetCode, byte[] messageContents)
		{
			//If someone chooses not to pass a packet then we'll use the empty packet.
			if(messageContents != null)
			{
				internalHighLevelMessageRepresentation = messageContents;
				SerializerKey = serializationKey;
			}
			else
			{
				messageContents = Serializer<ProtobufNetSerializer>.Instance.Serialize(Packet.Empty);
				serializationKey = Serializer<ProtobufNetSerializer>.Instance.SerializerUniqueKey;
			}

			OperationType = opType;
			encryptionMethod = EncryptionMethod;
			
			PacketCode = PacketCode;
		}

		public byte[] GetInternalBytes()
		{
			return internalHighLevelMessageRepresentation;
		}

		/*//TODO: Implement encryption
		public bool DecryptPackage()
		{

		}*/

		//Protobuf-net constructor
		private LidgrenTransferPacket()
		{
			isDecrypted = false;
		}
	}
}
