using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GladNet.Common
{
	public abstract class Serializer<T> : SerializerBase where T : SerializerBase
	{
		/// <summary>
		/// Lazily loaded instance of the serializer.
		/// </summary>
		private static Lazy<T> _Instance = new Lazy<T>(Activator.CreateInstance<T>, true);

		/// <summary>
		/// Public singleton access for the serializer instance.
		/// </summary>
		public static T Instance { get { return _Instance.Value; } }

		public override abstract byte[] Serialize<DataType>(DataType obj);

		public override abstract DataType Deserialize<DataType>(byte[] bytes);

		public override abstract byte SerializerUniqueKey { get; }
	}

	public abstract class SerializerBase
	{
		public abstract byte SerializerUniqueKey { get; }

		public abstract byte[] Serialize<DataType>(DataType obj);

		public abstract DataType Deserialize<DataType>(byte[] bytes);
	}
}
