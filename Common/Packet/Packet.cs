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
	/// <summary>
	/// (Not fully supported yet) This class provides functionality to apply custom serializers and deserializers to classes.
	/// As long as a serializer follows the contract laid out by Serializer class then the serializer can be used and defined on a per package basis.
	/// </summary>
	/// <typeparam name="SerializerType">Type of serializer desired.</typeparam>
	public abstract class Packet<SerializerType> where SerializerType : SerializerBase
	{
		protected Packet()
		{

		}

		public byte[] Serialize()
		{
			return Serializer<SerializerType>.Instance.Serialize(this);
		}
	}

	[ProtoContract]
	/// <summary>
	/// The packet base class that uses the default serialization method for GladNet.
	/// </summary>
	public abstract class Packet : Packet<ProtobufNetSerializer>
	{
		public static EmptyPacket Empty = new EmptyPacket();

		public readonly bool isMalformed;

		internal static IList<int> ReferencedProtobufSubtypes = new List<int>();

		/// <summary>
		/// Protobuf-net parameterless constructor
		/// </summary>
		private Packet() 
			: base()
		{

		}

		public Packet(bool malformed)
		{
			this.isMalformed = malformed;
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
			switch(method)
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
					throw new Exception("Unsupported DeliverType: " + method.ToString() + " attempted for networked message.");
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
					throw new Exception("Unsupported DeliverType: " + method.ToString() + " attempted for networked message.");
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
			RuntimeTypeModel.Default.CompileInPlace();
		}

		internal static bool Register(Type t, bool isInternal)
		{
			PacketAttribute attr = t.GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(PacketAttribute)) as PacketAttribute;

			if (attr == null)
				throw new Exception("Found derived type: " + t.Name + " of Packet that is not attributed by: " + typeof(PacketAttribute).Name + " all Protobut-net serializable " +
					"packets must be targeted by this attribute for key purposes.");

			if (attr.UniquePacketKey <= Packet.PacketModelNumberOffset && !isInternal)
				throw new Exception("Found derived type: " + t.Name + " of Packet that has a packet unique key value of " + attr.UniquePacketKey + "." +
					" It is required that this key be greater than " + Packet.PacketModelNumberOffset + " as anything lower is reserved internally.");

			if (ReferencedProtobufSubtypes.Contains(attr.UniquePacketKey))
				throw new Exception("Duplicate key " + attr.UniquePacketKey + " for Packet found on Type: " + t.Name + ". Key values must be distinct for a given system.");
			else
				ReferencedProtobufSubtypes.Add(attr.UniquePacketKey);

			//The crown jewel.
			RuntimeTypeModel.Default.Add(typeof(Packet), false).AddSubType(attr.UniquePacketKey, t);			

			return true;
		}
	}
}
