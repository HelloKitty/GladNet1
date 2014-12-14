#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using GladNet.Common.Register;
using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common.Serializers
{
	public class SerializationManager : IRegisterable<SerializerBase, byte>
	{
		private Dictionary<byte, SerializerBase> RegisteredSerializers;

		public SerializationManager()
		{
			RegisteredSerializers = new Dictionary<byte, SerializerBase>();
		}

		public bool Register(SerializerBase obj, byte key)
		{
			if (!RegisteredSerializers.ContainsKey(obj.SerializerUniqueKey))
			{
				OverrideRegisteredSerializer(obj);
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
			if(instance != null)
				RegisteredSerializers[instance.SerializerUniqueKey] = instance;
		}

		public IEnumerable<SerializerBase> Serializers()
		{
			return RegisteredSerializers.Values;
		}

		public SerializerBase GetValue(byte key)
		{
			if (RegisteredSerializers.ContainsKey(key))
				return RegisteredSerializers[key];
			else
				return RegisteredSerializers[Serializer<GladNetProtobufNetSerializer>.Instance.SerializerUniqueKey];
		}

		public bool HasKey(byte key)
		{
			return this.RegisteredSerializers.ContainsKey(key);
		}

		public SerializerBase this[byte key]
		{
			get { return GetValue(key); }
		}


		public void Clear()
		{
			this.RegisteredSerializers.Clear();
		}
	}
}
