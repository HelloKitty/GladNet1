using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Future implementation for version to version runtime import of packet keys.
	/// Designed to obsure and break malicious code that is based on packet keys.
	/// Similar approaches may be taken for packet strucutres, not just keys, too.
	/// A future implementation for this non-default protection will be provided.
	/// </summary>
	[RegisteredPacket]
	[ProtoContract]
	internal class ProtobufSyncPackage : Packet
	{
		private readonly object syncobj = new object();

		[ProtoMember(1)]
		private Dictionary<int, string> _TypeIncludeList;
		public IEnumerable<KeyValuePair<int, string>> TypeIncludeList
		{
			get { return _TypeIncludeList.ToList(); }
		}

		public ProtobufSyncPackage(int includeSize) 
		{
			_TypeIncludeList = new Dictionary<int, string>(includeSize);
		}

		/// <summary>
		/// Protobuf-net constructor
		/// </summary>
		protected ProtobufSyncPackage()
		{

		}

		public void RegisterProtobufType(int key, string typeName, bool threadSafeAdd)
		{
			if (threadSafeAdd)
				lock (syncobj)
				{
					_TypeIncludeList.Add(key, typeName);
				}
			else
				_TypeIncludeList.Add(key, typeName);
		}
	}
}
