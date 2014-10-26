using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	public abstract class ServerPeer : Peer
	{
		public ServerPeer(IConnectionDetails details) 
			: base(details)
		{

		}

		public override void PackageRecieve(RequestPackage package)
		{
			throw new Exception("ServerPeer base recieved a RequestPackage but ServerPeer cannot handle this message type.");
		}

		//protected abstract void PackageRecieve()
	}
}
