using Lidgren.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet.Common
{
	[ProtoContract]
	/// <summary>
	/// (Not fully supported yet) This class provides functionality to apply custom serializers and deserializers to classes.
	/// As long as a serializer follows the contract laid out by Serializer class then the serializer can be used and defined on a per package basis.
	/// </summary>
	public abstract class PacketBase
	{
		protected abstract SerializerBase PacketSerializer { get; }

		//Make this public to anticipate Webplayer use
		public PacketBase()
		{

		}

		public byte[] Serialize()
		{
			return PacketSerializer.Serialize(this);
		}

		public byte SerializerKey
		{
			get { return this.PacketSerializer.SerializerUniqueKey; }
		}
	}

	[ProtoContract]
	/// <summary>
	/// The packet base class that uses the default serialization method for GladNet.
	/// </summary>
	public abstract class Packet : PacketBase
	{
		public static EmptyPacket Empty = new EmptyPacket();

		protected override SerializerBase PacketSerializer
		{
			get { return Serializer<ProtobufNetSerializer>.Instance; }
		}

		internal static IList<int> ReferencedProtobufSubtypes = new List<int>();

		//Make this public to anticipate Webplayer use
		/// <summary>
		/// Protobuf-net parameterless constructor
		/// </summary>
		public Packet()
			: base()
		{

		}

		internal enum OperationType : byte
		{
			Event = 0,
			Request = 1,
			Response = 2
		}

		public enum DeliveryMethod
		{
			UnreliableAcceptDuplicate,
			UnreliableDiscardStale,
			ReliableUnordered,
			ReliableDiscardStale,
			ReliableOrdered
		}

		public enum SendResult : byte
		{
			FailedNotConnected = 0,
			Sent = 1,
			Queued = 2,
			Dropped = 3,
		}

		internal static DeliveryMethod LidgrenDeliveryMethodConvert(NetDeliveryMethod method)
		{
			switch (method)
			{
				case NetDeliveryMethod.ReliableOrdered:
					return DeliveryMethod.ReliableOrdered;
				case NetDeliveryMethod.ReliableSequenced:
					return DeliveryMethod.ReliableDiscardStale;
				case NetDeliveryMethod.ReliableUnordered:
					return DeliveryMethod.ReliableUnordered;
				case NetDeliveryMethod.Unreliable:
					return DeliveryMethod.UnreliableAcceptDuplicate;
				case NetDeliveryMethod.UnreliableSequenced:
					return DeliveryMethod.UnreliableDiscardStale;

				default:
					throw new LoggableException("Unsupported DeliverType: " + method.ToString() + " attempted for networked message.", null, Logger.LogType.Error);
			}
		}

		internal static NetDeliveryMethod LidgrenDeliveryMethodConvert(DeliveryMethod method)
		{
			switch (method)
			{
				case DeliveryMethod.ReliableOrdered:
					return NetDeliveryMethod.ReliableOrdered;

				case DeliveryMethod.ReliableDiscardStale:
					return NetDeliveryMethod.ReliableSequenced;

				case DeliveryMethod.ReliableUnordered:
					return NetDeliveryMethod.ReliableUnordered;

				case DeliveryMethod.UnreliableAcceptDuplicate:
					return NetDeliveryMethod.Unreliable;

				case DeliveryMethod.UnreliableDiscardStale:
					return NetDeliveryMethod.UnreliableSequenced;
				default:
					throw new LoggableException("Unsupported DeliverType: " + method.ToString() + " attempted for networked message.", null, Logger.LogType.Error);
			}
		}

		internal static readonly int PacketModelNumberOffset = 20;

		//For the love of all that is holy DO NOT touch this unless you KNOW the torment I have experienced trying to get this to work properly
		//It may not seem like much but it's gone through many revissions, others way more complex and were tossed out in lieu of a less cool design
		//but more reasonable.
		public static bool Register(Type t)
		{
			return Register(t, false);
		}

		internal static void LockInProtobufnet()
		{
			//This will let the serializer know of the base packet types via PacketBase
			RuntimeTypeModel.Default.CompileInPlace();
		}

		internal static bool Register(Type t, bool isInternal)
		{
			PacketAttribute attr = t.GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(PacketAttribute)) as PacketAttribute;

			if (attr == null)
				throw new LoggableException("Found derived type: " + t.Name + " of Packet that is not attributed by: " + typeof(PacketAttribute).Name + " all Protobut-net serializable " +
					"packets must be targeted by this attribute for key purposes.", null, Logger.LogType.Error);

			if (attr.UniquePacketKey <= Packet.PacketModelNumberOffset && !isInternal)
				throw new LoggableException("Found derived type: " + t.Name + " of Packet that has a packet unique key value of " + attr.UniquePacketKey + "." +
					" It is required that this key be greater than " + Packet.PacketModelNumberOffset + " as anything lower is reserved internally.", null, Logger.LogType.Error);

			if (ReferencedProtobufSubtypes.Contains(attr.UniquePacketKey))
				throw new LoggableException("Duplicate key " + attr.UniquePacketKey + " for Packet found on Type: " + t.Name + ". Key values must be distinct for a given system.",
					null, Logger.LogType.Error);
			else
				ReferencedProtobufSubtypes.Add(attr.UniquePacketKey);

			try
			{
				//The crown jewel.
				RuntimeTypeModel.Default.Add(typeof(Packet), false).AddSubType(attr.UniquePacketKey, t);
			}
			//This could happen if someone tries to register a packet after we've compiled or something
			catch (ProtoException e)
			{
				throw new LoggableException("Failed to register PacketType: " + t.FullName + " ID: " + attr.UniquePacketKey + "." +
					" You may be trying to register it with an invalid ID or multiple times.", e, Logger.LogType.Error);
			}
			return true;
		}

		public static void SetupProtoRuntimePacketInheritance()
		{
			//Avoid adding the type twice.
			if (RuntimeTypeModel.Default[typeof(PacketBase)].GetSubtypes().Select(x => x.FieldNumber == 1).Count() == 0)
				RuntimeTypeModel.Default.Add(typeof(PacketBase), false).AddSubType(1, typeof(Packet));
		}
	}
}
