using Common.Packet.Serializers;
using GladNet.Server.Logging;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class PacketParser : ILoggable
	{
		public Logger ClassLogger { get; private set; }

		protected readonly IRegisterable<SerializerBase, byte> SerializerRegister;

		protected readonly IPacketConverter Converter;

		public PacketParser(Logger logger)
		{
			ClassLogger = logger;

			SerializerRegister = new SerializationManager();
			Converter = new PacketConverter();
		}

		public PacketParser(Logger logger, IRegisterable<SerializerBase, byte> serializerRegister, IPacketConverter converter)
		{
			ClassLogger = logger;
			this.SerializerRegister = serializerRegister;
			Converter = converter;	
		}

		public LidgrenTransferPacket BuildTransferPacket(NetBuffer msg)
		{
#if DEBUGBUILD
			ClassLogger.LogDebug("Recieved a high level message from client ID: " + msg.SenderConnection.RemoteUniqueIdentifier);
#endif
			try
			{
				//Due to message recycling we cannot trust the internal array of data to be of only the information that should be used for this package.
				//We can trust the indicates size, not the length of .Data, and get a byte[] that represents the sent LidgrenTransferPacket.
				//However, this will incur a GC penalty which may become an issue; more likely to be an issue on clients.
				return Serializer<GladNetProtobufNetSerializer>.Instance.Deserialize<LidgrenTransferPacket>(msg.ReadBytes(msg.LengthBytes - msg.PositionInBytes));
			}
			catch (LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? e.InnerException.Message : "");
				return null;
			}
		}

		//(No longer internal due to Unity3D Requirements) This is internal because we don't want child classes having access to it but we need some derived classes to have access.
		protected PackageType GeneratePackage<PackageType>(IEncryptablePackage packet, EncryptionBase decrypter)
			where PackageType : NetworkPackage, new()
		{
			if (SerializerRegister.GetValue(packet.SerializerKey) == null)
				throw new LoggableException("Packet serializer not found with get.", null, Logger.LogType.Error);

			return Converter.Convert<PackageType>(packet, SerializerRegister[packet.SerializerKey], decrypter);
		}

		//(No longer internal due to Unity3D Requirements) This is internal because we don't want child classes having access to it but we need some derived classes to have access.
		protected PackageType GeneratePackage<PackageType>(IPackage packet)
			where PackageType : NetworkPackage, new()
		{
			if (SerializerRegister.GetValue(packet.SerializerKey) == null)
				throw new LoggableException("Packet serializer not found with get.", null, Logger.LogType.Error);

			return Converter.Convert<PackageType>(packet, SerializerRegister[packet.SerializerKey]);
		}

		public bool Register<T>() where T : SerializerBase
		{
			if (SerializerRegister.HasKey(Serializer<T>.Instance.SerializerUniqueKey))
				throw new LoggableException("Failed to register Serializer of Type: " + Serializer<T>.Instance.GetType().FullName + " due to a already inuse serializer key.",
					null, Logger.LogType.Error);

			return this.SerializerRegister.Register(Serializer<T>.Instance, Serializer<T>.Instance.SerializerUniqueKey);
		}
	}
}
