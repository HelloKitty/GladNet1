using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Client
{
	public class ServerConnectionDetails : IConnectionDetails
	{

		public IPEndPoint RemoteConnectionEndpoint { get; private set; }

		public long UniqueConnectionId { get; private set; }

		public NetConnection InternalNetConnection { get; private set; }

		public ServerConnectionDetails(IPEndPoint endPoint, long uniqueConnectionID, NetConnection netConnection)
		{
			InternalNetConnection = netConnection;
			RemoteConnectionEndpoint = endPoint;
			UniqueConnectionId = uniqueConnectionID;
		}
	}
}
