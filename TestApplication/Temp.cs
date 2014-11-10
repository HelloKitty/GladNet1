using GladNet.Common;
using GladNet.Server.Connections.Readers;
using GladNet.Server.Logging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
		public TestPacket()
		{

		}
	}

	public class Templogger : Logger
	{
		public Templogger() : base(Logger.LogType.Debug) { }

		protected override void Log(string text, Logger.LogType state)
		{
			Console.WriteLine(text);
		}

		protected override void Log(string text, Logger.LogType state, params object[] data)
		{
			throw new NotImplementedException();
		}

		protected override void Log(string text, Logger.LogType state, params string[] data)
		{
			throw new NotImplementedException();
		}

		protected override void Log(object obj, Logger.LogType state)
		{
			throw new NotImplementedException();
		}
	}

	public class Temp : ServerCore
	{

		public Temp(string s) : base(AsyncConsoleLogger.Instance, "test", 5056, "hiya")
		{
			AsyncConsoleLogger.Instance.SetState(Logger.LogType.Debug);
		}

		protected override ServerPeer OnConnectionSuccess(ConnectionResponse response)
		{
			AsyncConsoleLogger.Instance.LogDebug("Successfully connected to a subserver!");
			return null;
		}

		protected override void OnConnectionFailure(ConnectionResponse response)
		{
			base.OnConnectionFailure(response);

			AsyncConsoleLogger.Instance.LogDebug("Failed to connect to a subserver.");
		}

		protected override void OnStartup()
		{
			this.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5055), "hi");
		}

		protected override void OnShutdown()
		{

		}

		protected override void RegisterProtobufPackets(Func<Type, bool> registerAsDefaultFunc)
		{
			registerAsDefaultFunc(typeof(TestPacket));
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
			AsyncConsoleLogger.Instance.LogDebug("Recieved a package request.");
		}

		public override void OnDisconnection()
		{
			AsyncConsoleLogger.Instance.LogDebug("Disconnected");
		}
	}
}
