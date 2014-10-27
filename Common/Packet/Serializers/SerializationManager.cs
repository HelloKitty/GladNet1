using Common.Register;
using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packet.Serializers
{
	public class SerializationManager : IRegisterable<SerializerBase, byte>, IHashContainer<SerializerBase, byte>
	{
		private Dictionary<byte, SerializerBase> RegisteredSerializers;

		public SerializationManager()
		{
			RegisteredSerializers = new Dictionary<byte, SerializerBase>();
		}

		public bool Register(SerializerBase instance, byte key)
		{
			if (!RegisteredSerializers.ContainsKey(instance.SerializerUniqueKey))
			{
				OverrideRegisteredSerializer(instance);
				return true;
			}
			else
				return false;
		}

		public bool UnRegister(byte key)
		{
			return RegisteredSerializers.Remove(key);
		}

		public void OverrideRegisteredSerializer(SerializerBase instance)
		{
			RegisteredSerializers[instance.SerializerUniqueKey] = instance;
		}

		public IEnumerable<SerializerBase> Serializers()
		{
			return RegisteredSerializers.Values;
		}

		public SerializerBase Get(byte key)
		{
			if (RegisteredSerializers.ContainsKey(key))
				return RegisteredSerializers[key];
			else
				return RegisteredSerializers[ProtobufNetSerializer.Instance.SerializerUniqueKey];
		}

		public bool HasKey(byte key)
		{
			return this.RegisteredSerializers.ContainsKey(key);
		}
	}
}
