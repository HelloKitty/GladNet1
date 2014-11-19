#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	//TODO: Mark internal after testing.
	[ProtoContract]
	public class LidgrenTransferPacket
	{
		[ProtoMember(1, IsRequired = true)]
		private byte[] internalHighLevelMessageRepresentation;

		[ProtoMember(2)]
		private byte _OperationType;

		public Packet.OperationType OperationType
		{
			get { return (Packet.OperationType)_OperationType; }
		}

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
				messageContents = Serializer<GladNetProtobufNetSerializer>.Instance.Serialize(Packet.Empty);
				serializationKey = Serializer<GladNetProtobufNetSerializer>.Instance.SerializerUniqueKey;
			}

			_OperationType = (byte)opType;
			encryptionMethod = EncryptionMethod;

			PacketCode = packetCode;
		}

		public byte[] GetInternalBytes()
		{
			return internalHighLevelMessageRepresentation;
		}

		//Protobuf-net constructor
		private LidgrenTransferPacket()
		{
			isDecrypted = false;
		}
	}
}
