#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
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
			throw new LoggableException("ServerPeer base recieved a RequestPackage but ServerPeer cannot handle this message type.", null, Logger.LogType.Error);
		}

		public override abstract void PackageRecieve(EventPackage package);

		public override abstract void PackageRecieve(ResponsePackage package);
	}
}
