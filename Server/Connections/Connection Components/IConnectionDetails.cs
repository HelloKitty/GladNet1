using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	public interface IConnectionDetails
	{
		IPEndPoint RemoteConnectionEndpoint { get; }
		long UniqueConnectionId { get; }
		NetConnection InternalNetConnection { get; }
	}
}
