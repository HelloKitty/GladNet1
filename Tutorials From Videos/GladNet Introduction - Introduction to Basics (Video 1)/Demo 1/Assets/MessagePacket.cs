using GladNet.Common;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[PacketAttribute(21)]
[ProtoContract]
public class MessagePacket : Packet
{
	[ProtoMember(1)]
	public string Message { get; private set; }

	[ProtoMember(2)]
	public string Name { get; private set; }

	public MessagePacket(string message, string name)
	{
		Message = message;
		Name = name;
	}

	public string GetMessage()
	{
		return Name + ": " + Message;
	}

	/// <summary>
	/// Protobuf-net constructor
	/// </summary>
	public MessagePacket()
	{

	}
}
