using GladNet.Common;
using GladNet.Server.Logging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	[PacketAttribute(21)]
	[ProtoContract]
	public class TestPacket : Packet
	{
		[ProtoMember(1)]
		int i = 0;
		public TestPacket() : base(false)
		{

		}
	}

	public class Templogger : Logger
	{
		public Templogger() : base(Logger.State.Error) { }

		protected override void Log(string text, Logger.State state)
		{
			Console.WriteLine(text);
		}

		protected override void Log(string text, object[] data, Logger.State state)
		{

		}

		protected override void Log(string text, string[] data, Logger.State state)
		{

		}

		protected override void Log(object obj, Logger.State state)
		{

		}
	}

	public class Temp : ServerCore<Templogger>
	{

		public Temp(string s) : base(new Templogger(), "test", 5055, "hi") { }

		protected override Peer OnAttemptedConnection(ConnectionRequest request)
		{
			this.ClassLogger.LogError("Recieved a connection success trying to create connection object for IP: " + request.RemoteConnectionEndpoint.ToString() + " ID: " + request.UniqueConnectionId);
			return null;
		}

		protected override ServerPeer OnConnectionSuccess(ConnectionResponse response)
		{
			return null;
		}

		protected override void OnStartup()
		{

		}

		protected override void OnShutdown()
		{

		}
	}
}
