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
		protected PacketBase PacketConstructor(byte[] bytes, SerializerBase serializer)
		{
			if (bytes != null)
				return serializer.Deserialize<PacketBase>(bytes);
			else
				return null;
		}

		public NetworkPackageType BuildIncomingNetPackage<NetworkPackageType>(LidgrenTransferPacket lgPacket, SerializerBase serializer)
			where NetworkPackageType : NetworkPackage, new()
		{
			NetworkPackageType package = new NetworkPackageType();

			try
			{
				PacketBase p = PacketConstructor(lgPacket.GetInternalBytes(), serializer);

				package.FillPackage(p == null ? new MalformedPacket() : p, lgPacket.PacketCode,
					lgPacket.EncryptionMethod != 0);
			}
			catch (SerializationException e)
			{
				//TODO: Better support for encryption
				throw;
				package.FillPackage(new MalformedPacket(), lgPacket.PacketCode, lgPacket.EncryptionMethod != 0);
			}
			catch (Exception e)
			{
				throw;
			}

			return package;
		}
	}
}
