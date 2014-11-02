using Common.Exceptions;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	internal class HigherLevelPacketHandler : HigherLevelPacketHandler<ProtobufNetSerializer>
	{

	}

	//TODO: Refactor. So much code duplication
	internal class HigherLevelPacketHandler<SerializerType> : HigherLevelPacketHandlerBase where SerializerType : SerializerBase
	{
		protected override EventPackage HandleEventPackage(HigherLevelPacket obj)
		{
			Packet p = null;

			try
			{
				if (obj.Data != null)
					p = Serializer<SerializerType>.Instance.Deserialize<Packet>(obj.Data);

				return new EventPackage(p == null ? new EmptyPacket(true) : p, obj.PacketCode, obj.EncryptionScheme != 0);
			}
			catch (SerializationException e)
			{
				//TODO: Better support for encryption
				return new EventPackage(new EmptyPacket(true), obj.PacketCode, obj.EncryptionScheme != 0);
			}
			catch (Exception e)
			{
				throw;
			}
		}

		protected override RequestPackage HandleRequestPackage(HigherLevelPacket obj)
		{
			Packet p = null;

			try
			{
				if (obj.Data != null)
					p = Serializer<SerializerType>.Instance.Deserialize<Packet>(obj.Data);

				return new RequestPackage(p == null ? new EmptyPacket(true) : p, obj.PacketCode, obj.EncryptionScheme != 0);
			}
			catch (SerializationException e)
			{
				//TODO: Better support for encryption
				return new RequestPackage(new EmptyPacket(true), obj.PacketCode, obj.EncryptionScheme != 0);
			}
			catch (Exception e)
			{
				throw;
			}
		}

		protected override ResponsePackage HandleResponsePackage(HigherLevelPacket obj)
		{
			Packet p = null;

			try
			{
				if (obj.Data != null)
					p = Serializer<SerializerType>.Instance.Deserialize<Packet>(obj.Data);

				return new ResponsePackage(p == null ? new EmptyPacket(true) : p, obj.PacketCode, obj.EncryptionScheme != 0);
			}
			catch (SerializationException e)
			{
				//TODO: Better support for encryption
				return new ResponsePackage(new EmptyPacket(true), obj.PacketCode, obj.EncryptionScheme != 0);
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}

	public abstract class HigherLevelPacketHandlerBase 
		: IHandler<HigherLevelPacket, EventPackage>, IHandler<HigherLevelPacket, RequestPackage>, IHandler<HigherLevelPacket, ResponsePackage>
	{
		EventPackage IHandler<HigherLevelPacket, EventPackage>.Handle(HigherLevelPacket obj)
		{
			return HandleEventPackage(obj);
		}

		RequestPackage IHandler<HigherLevelPacket, RequestPackage>.Handle(HigherLevelPacket obj)
		{
			return HandleRequestPackage(obj);
		}

		ResponsePackage IHandler<HigherLevelPacket, ResponsePackage>.Handle(HigherLevelPacket obj)
		{
			return HandleResponsePackage(obj);
		}

		protected abstract ResponsePackage HandleResponsePackage(HigherLevelPacket obj);
		protected abstract RequestPackage HandleRequestPackage(HigherLevelPacket obj);
		protected abstract EventPackage HandleEventPackage(HigherLevelPacket obj);
	}
}
