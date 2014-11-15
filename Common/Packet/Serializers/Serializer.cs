#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
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
		//TODO: Thread safety
		//Can't use Lazy<T> in Unity
#if !UNITYDEBUG && !UNITYRELEASE
		/// <summary>
		/// Lazily loaded instance of the serializer.
		/// </summary>
		private static Lazy<T> _Instance = new Lazy<T>(Activator.CreateInstance<T>, true);
#else
		private static T _Instance = Activator.CreateInstance<T>();
#endif

	//Can't use Lazy<T> in Unity
#if !UNITYDEBUG && !UNITYRELEASE
		/// <summary>
		/// Public singleton access for the serializer instance.
		/// </summary>
		public static T Instance { get { return _Instance.Value; } }
#else
		public static T Instance { get { return _Instance; } }
#endif

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
