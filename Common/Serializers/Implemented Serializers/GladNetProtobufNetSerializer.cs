#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using Common.Exceptions;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public sealed class GladNetProtobufNetSerializer : SerializerBase
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
				throw new SerializationException(typeof(DataType), typeof(GladNetProtobufNetSerializer), e, "Failed to serialize type.");
			}
		}

		public override DataType Deserialize<DataType>(byte[] bytes)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream(bytes))
				{
					ms.Position = 0;
					return Serializer.Deserialize<DataType>(ms);
				}
			}
			catch (Exception e)
			{
				throw new SerializationException(typeof(DataType), typeof(GladNetProtobufNetSerializer), e, "Failed to deserialize type.");
			}
		}

		public override byte SerializerUniqueKey
		{
			get { return 0; }
		}
	}
}
