using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections.Readers
{
	public abstract class ClientPeer : Peer
	{
		public ClientPeer(IConnectionDetails details)
			: base(details)
		{

		}

		public override abstract void PackageRecieve(RequestPackage package);
	}
}
