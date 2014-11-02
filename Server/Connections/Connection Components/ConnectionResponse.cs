using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	public class ConnectionResponse : IConnectionDetails
	{
		public IPEndPoint RemoteConnectionEndpoint { get; private set; }
		public long UniqueConnectionId { get; private set; }
		public NetConnection InternalNetConnection { get; private set; }

		public readonly string HailUsed;

		internal readonly bool Result;

		public ConnectionResponse(IPEndPoint point, long uid, NetConnection connection, bool result, string hail)//, byte connectionType)
		{
			this.RemoteConnectionEndpoint = point;
			this.UniqueConnectionId = uid;
			this.InternalNetConnection = connection;
			Result = result;
			HailUsed = hail;
		}
	}
}
