#region copyright
/// GladNet Copyright (C) 2014 X 
/// X
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GladNet.Common
{
	//TODO: Mark internal after testing.
	[ProtoContract]
	public class LidgrenTransferPacket : IEncryptablePackage
	{
		[ProtoMember(1, IsRequired = true)]
		public byte[] InternalByteRepresentation { get; protected set; }

		[ProtoMember(2)]
		public Packet.OperationType OperationType { get; private set; }

		[ProtoMember(3)]
		public byte EncryptionMethodByte { get; protected set; }

		[ProtoMember(4)]
		public byte SerializerKey { get; private set; }

		[ProtoMember(5)]
		public byte PacketCode { get; private set; }

		[ProtoMember(6, IsRequired = false)]
		private byte[] _EncryptionAdditionalBlob;
		/// <summary>
		/// This will cost us nothing if it's not needed and set to null in Protobufnet
		/// </summary>
		public byte[] EncryptionAdditionalBlob
		{
			get { return this._EncryptionAdditionalBlob; }
		}

		private bool decrypted;
		public virtual bool isEncrypted
		{
			get { return this.EncryptionMethodByte != EncryptionBase.NoEncryptionByte && !decrypted; }
		}

		public virtual bool wasEncrypted
		{
			get { return this.EncryptionMethodByte != EncryptionBase.NoEncryptionByte; }
		}

		public LidgrenTransferPacket(Packet.OperationType opType, byte serializationKey, byte packetCode, byte[] messageContents)
		{
			//If someone chooses not to pass a packet then we'll use the empty packet.
			if(messageContents != null)
			{
				InternalByteRepresentation = messageContents;
				SerializerKey = serializationKey;
			}
			else
			{
				messageContents = Serializer<GladNetProtobufNetSerializer>.Instance.Serialize(Packet.Empty);
				serializationKey = Serializer<GladNetProtobufNetSerializer>.Instance.SerializerUniqueKey;
			}

			OperationType = opType;
			PacketCode = packetCode;

			EncryptionMethodByte = EncryptionBase.NoEncryptionByte;
			decrypted = false;
		}

		//Protobuf-net constructor
		private LidgrenTransferPacket()
		{
			decrypted = false;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("LidgrenTransferPacket - ");

			builder.AppendFormat("OperationType: {0} PacketCode: {1} EncryptionMethod: {2} SerializerKey: {3}",
				this.OperationType.ToString(), this.PacketCode, this.EncryptionMethodByte, this.SerializerKey);

			return builder.ToString();
		}

		public virtual void Encrypt(EncryptionBase encryptionObject)
		{
			if (InternalByteRepresentation != null)
			{
				//Sets the encryption method used via  byte so remote recievers will know how to handle the
				//encrypted byte[]
				this.EncryptionMethodByte = encryptionObject.EncryptionTypeByte;

				try
				{
					this.InternalByteRepresentation = encryptionObject.Encrypt(InternalByteRepresentation,
						out _EncryptionAdditionalBlob);
				}
				catch (CryptographicException e)
				{
					throw new LoggableException("Failed to encrypt LidgrenPacket: " + this.ToString(), e, LogType.Error);
				}
			}
		}

		public virtual bool Decrypt(EncryptionBase encryptionObject)
		{
			if (InternalByteRepresentation != null && EncryptionMethodByte != EncryptionBase.NoEncryptionByte)
			{
				try
				{
					this.InternalByteRepresentation = encryptionObject.Decrypt(InternalByteRepresentation,
						_EncryptionAdditionalBlob);

					decrypted = true;

					return this.InternalByteRepresentation != null;
				}
				catch(CryptographicException e)
				{
					throw new LoggableException("Failed to decrypt LidgrenPacket: " + this.ToString(), e, LogType.Error);
				}
			}
			else
				return false;
		}
	}
}
