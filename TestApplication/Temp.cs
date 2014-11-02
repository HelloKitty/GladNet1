using GladNet.Common;
using GladNet.Server.App.Logging.Loggers;
using GladNet.Server.Connections.Readers;
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
		public Templogger() : base(Logger.State.Debug) { }

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

	public class Temp : ServerCore<AsyncConsoleLogger>	
	{

		public Temp(string s) : base(AsyncConsoleLogger.Instance, "test", 5055, "hi") 
		{
			AsyncConsoleLogger.Instance.SetState(Logger.State.Debug);
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

		protected override void RegisterPackets(Func<Type, bool> registerAsDefaultFunc)
		{
			registerAsDefaultFunc(typeof(TestPacket));
			//registerAsDefaultFunc(typeof(EmptyPacket));
		}

		protected override byte ServerTypeUniqueByte
		{
			get { return 1; }
		}

		protected override ClientPeer OnAttemptedConnection(ConnectionRequest request)
		{
			this.ClassLogger.LogError("Recieved a connection success trying to create connection object for IP: " + request.RemoteConnectionEndpoint.ToString() + " ID: " + request.UniqueConnectionId);
			return new TestClientPeer(request);
		}
	}

	public class TestClientPeer : ClientPeer
	{
		public TestClientPeer(IConnectionDetails details) : base(details)
		{

		}

		public override void PackageRecieve(RequestPackage package)
		{
			Console.WriteLine("Recieved a package request.");
		}

		public override void OnDisconnection()
		{
			Console.WriteLine("Disconnected");
		}
	}
}
