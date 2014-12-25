using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GladNet.Common
{
	public class GladNetXmlSerializer : SerializerBase
	{
		public override byte SerializerUniqueKey
		{
			get { return 2; }
		}

		public override byte[] Serialize<DataType>(DataType obj)
		{
			try
			{
				var xml = new XmlSerializer(typeof(DataType));

				using (var ms = new MemoryStream())
				{
					xml.Serialize(ms, obj);
					ms.Position = 0;
					return ms.ToArray();
				}
			}
			catch(Exception e)
			{
				throw new LoggableException("Failed to serialize object with xml to byte[]", e, LogType.Error);
			}
		}

		public string SerializeToString<DataType>(DataType obj)
		{
			try
			{
				var xml = new XmlSerializer(typeof(DataType));

				using (TextWriter tw = new StringWriter())
				{
					xml.Serialize(tw, obj);
					return tw.ToString();
				}
			}
			catch(Exception e)
			{
				throw new LoggableException("Failed to serialize object with xml to string", e, LogType.Error);
			}
		}

		public override DataType Deserialize<DataType>(byte[] bytes)
		{
			try
			{
				var xml = new XmlSerializer(typeof(DataType));

				using (var ms = new MemoryStream(bytes))
				{
					return (DataType)xml.Deserialize(ms);
				}
			}
			catch(Exception e)
			{
				throw new LoggableException("Failed to deserialize object with xml from byte[]", e, LogType.Error);
			}
		}

		public DataType DeserializeFromString<DataType>(string xmlStringRepresentation)
		{
			try
			{
				var xml = new XmlSerializer(typeof(DataType));

				using (TextReader tr = new StringReader(xmlStringRepresentation))
				{
					return (DataType)xml.Deserialize(tr);
				}
			}
			catch(Exception e)
			{
				throw new LoggableException("Failed to deserialize object with xml from string", e, LogType.Error);
			}
		}
	}
}
