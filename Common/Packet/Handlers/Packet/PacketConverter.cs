using Common.Exceptions;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{

	internal class PacketConverter
	{
		private readonly SerializerBase Serializer;

		protected PacketBase PacketConstructor(byte[] bytes)
		{
			if (bytes != null)
				return Serializer.Deserialize<PacketBase>(bytes);
			else
				return null;
		}

		public NetworkPackageType BuildIncomingNetPackage<NetworkPackageType>(LidgrenTransferPacket lgPacket)
			where NetworkPackageType : NetworkPackage, new()
		{
			NetworkPackageType package = new NetworkPackageType();

			try
			{
				PacketBase p = PacketConstructor(lgPacket.GetInternalBytes());

				package.FillPackage(p == null ? new MalformedPacket() : p, lgPacket.PacketCode,
					lgPacket.EncryptionMethod != 0);
			}
			catch (SerializationException e)
			{
				//TODO: Better support for encryption
				package.FillPackage(new MalformedPacket(), lgPacket.PacketCode, lgPacket.EncryptionMethod != 0);
			}
			catch (Exception e)
			{
				throw;
			}

			return package;
		}
	}

	/*internal class ProtobufNetPacketConverter
	{
		public NetworkPackageType BuildIncomingNetPackage<NetworkPackageType>(LidgrenTransferPacket lgPacket)
			where NetworkPackageType : NetworkPackage<ProtobufNetSerializer>, new()
		{
			NetworkPackageType package = new NetworkPackageType();

			try
			{
				Packet p = PacketConstructor(lgPacket.GetInternalBytes());

				package.FillPackage(p == null ? new MalformedPacket() : p, lgPacket.PacketCode,
					lgPacket.EncryptionMethod != 0);
			}
			catch (SerializationException e)
			{
				//TODO: Better support for encryption
				package.FillPackage(new MalformedPacket(), lgPacket.PacketCode, lgPacket.EncryptionMethod != 0);
			}
			catch (Exception e)
			{
				throw;
			}

			return package;
		}

		protected Packet PacketConstructor(byte[] bytes)
		{
			if (bytes != null)
				return Serializer<ProtobufNetSerializer>.Instance.Deserialize<Packet>(bytes);
			else
				return null;
		}
	}*/
}
