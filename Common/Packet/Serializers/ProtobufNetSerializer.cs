using Common.Exceptions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public sealed class ProtobufNetSerializer : SerializerBase
	{
		public override byte[] Serialize<DataType>(DataType obj)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					Serializer.Serialize(ms, obj);
					ms.Position = 0;
					return ms.ToArray();
				}
			}
			catch(Exception e)
			{
				throw new SerializationException(typeof(DataType), typeof(ProtobufNetSerializer), e, "Failed to serialize type.");
			}
		}

		public override DataType Deserialize<DataType>(byte[] bytes)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream(bytes))
					return Serializer.Deserialize<DataType>(ms);
			}
			catch (Exception e)
			{
				throw new SerializationException(typeof(DataType), typeof(ProtobufNetSerializer), e, "Failed to deserialize type.");
			}
		}

		public override byte SerializerUniqueKey
		{
			get { return 0; }
		}
	}
}
