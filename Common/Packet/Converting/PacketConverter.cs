#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using Common.Exceptions;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	//TODO: Better support for encryption
	public class PacketConverter : IPacketConverter
	{
		public PacketBase PacketConstructor(byte[] bytes, SerializerBase serializer)
		{
			if (bytes != null)
				return serializer.Deserialize<PacketBase>(bytes);
			else
				return null;
		}

		private NetworkPackageType GenerateFromPacket<NetworkPackageType>(IPackage packet, SerializerBase serializer)
			where NetworkPackageType : NetworkPackage, new()
		{
			NetworkPackageType package = new NetworkPackageType();

			try
			{
				PacketBase p = PacketConstructor(packet.InternalByteRepresentation, serializer);

				package.FillPackage(p == null ? new MalformedPacket() : p, packet.PacketCode);

				return package;
			}
			catch (SerializationException e)
			{
				throw new LoggableException("Failed to deserialize package.", e, Logger.LogType.Error);
			}
		}

		private bool DecryptIncomingPacket(IEncryptable packet, EncryptionBase decrypter)
		{
			if (packet.isEncrypted)
			{
				if (decrypter.EncryptionTypeByte == packet.EncryptionMethodByte)
					return packet.Decrypt(decrypter);
				else
					throw new LoggableException("Failed to decrypt byte[] blob due to decryptor object being of byte: " + decrypter.EncryptionTypeByte.ToString() +
				" and lidgren packet encryption byte being: " + packet.EncryptionMethodByte, null, Logger.LogType.Error);
			}
			else
				return false;
		}

		public NetworkPackageType Convert<NetworkPackageType>(IPackage package, SerializerBase serializer) 
			where NetworkPackageType : NetworkPackage, new()
		{
			return GenerateFromPacket<NetworkPackageType>(package, serializer);
		}

		public NetworkPackageType Convert<NetworkPackageType>(IEncryptablePackage package, SerializerBase serializer, EncryptionBase decryptor) 
			where NetworkPackageType : NetworkPackage, new()
		{
			if (decryptor != null)
				this.DecryptIncomingPacket(package, decryptor);

			NetworkPackageType dispatchPackage = GenerateFromPacket<NetworkPackageType>(package, serializer);

			if (package != null)
				dispatchPackage.EncryptionMethodByte = package.EncryptionMethodByte;

			return dispatchPackage;
		}

		#region Explictly implemented IConverter methods
		NetworkPackage IConverter<IEncryptablePackage, SerializerBase, EncryptionBase, NetworkPackage>.Convert(IEncryptablePackage obj1, SerializerBase obj2, EncryptionBase obj3)
		{
			return this.Convert<NetworkPackage>(obj1, obj2, obj3);
		}

		NetworkPackage IConverter<IPackage, SerializerBase, NetworkPackage>.Convert(IPackage obj1, SerializerBase obj2)
		{
			return this.Convert<NetworkPackage>(obj1, obj2);
		}
		#endregion
	}
}
