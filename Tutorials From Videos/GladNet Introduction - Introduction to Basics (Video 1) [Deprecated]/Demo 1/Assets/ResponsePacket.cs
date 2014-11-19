using GladNet.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[PacketAttribute(22)]
[ProtoContract]
public class ResponsePacket : Packet
{
	[ProtoMember(1)]
	public string Response { get; private set; }

	public ResponsePacket(string response)
	{
		Response = response;
	}

	public ResponsePacket()
	{

	}
}
