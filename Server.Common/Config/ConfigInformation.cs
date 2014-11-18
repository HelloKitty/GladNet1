using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Common
{
	[ProtoContract]
	public class ConfigInformation
	{
		//Due to XML we must make the setters public.
		[ProtoMember(1, IsRequired=true)]
		public string DLLName { get; set; }

		[ProtoMember(2, IsRequired=true)]
		public string HailMessage { get; set; }

		[ProtoMember(3, IsRequired=true)]
		public string ApplicationName { get; set; }

		[ProtoMember(4, IsRequired = true)]
		public int Port { get; set; }

		public ConfigInformation(string dllname, string hailmessage, string appname, int port)
		{
			DLLName = dllname;
			HailMessage = hailmessage;
			ApplicationName = appname;
			Port = port;
		}

		protected ConfigInformation()
		{

		}
	}
}
