using GladNet.Common;
using GladNet.Server.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	[PacketAttribute(5)]
	public class TestPacket : Packet
	{

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
