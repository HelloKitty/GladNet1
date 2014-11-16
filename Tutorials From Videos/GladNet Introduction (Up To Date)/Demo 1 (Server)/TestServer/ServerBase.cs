using GladNet.Common;
using GladNet.Server;
using GladNet.Server.Connections;
using GladNet.Server.Connections.Readers;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
	public class ServerBase : ServerCore
	{
		//APPNAME HAILMESSAGE PORT PIPEHANDLE
		public ServerBase(string applicationName, string hailMessage, int port) 
			: base(AsyncConsoleLogger.Instance, applicationName, port, hailMessage)
		{

		}

		protected override ClientPeer OnAttemptedConnection(ConnectionRequest request)
		{
			return new TestClient(request);
		}

		protected override ServerPeer OnConnectionSuccess(ConnectionResponse response)
		{
			return null;
		}

		protected override void OnShutdown()
		{
			
		}

		protected override void OnStartup()
		{
			this.ClassLogger.LogDebug("Server started.");
		}

		protected override byte ServerTypeUniqueByte
		{
			get { return 1; }
		}

		protected override void RegisterProtobufPackets(Func<Type, bool> registerAsDefaultFunc)
		{
			registerAsDefaultFunc(typeof(MessagePacket));
			registerAsDefaultFunc(typeof(ResponsePacket));
		}
	}

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
}
