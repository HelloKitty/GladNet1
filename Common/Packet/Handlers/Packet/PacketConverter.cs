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
	public class PacketConverter
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
				throw new LoggableException("Failed to deserialize package.", e, Logger.LogType.Error);
			}

			return package;
		}
	}
}
