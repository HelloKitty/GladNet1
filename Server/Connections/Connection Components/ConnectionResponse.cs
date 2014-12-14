#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using GladNet.Common;
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

		public readonly object CallbackObj;

		internal bool Result { get; set; }

		public ConnectionResponse(IPEndPoint point, long uid, NetConnection connection, string hail, object callback)//, byte connectionType)
		{
			this.RemoteConnectionEndpoint = point;
			this.UniqueConnectionId = uid;
			this.InternalNetConnection = connection;
			HailUsed = hail;
			CallbackObj = callback;
		}
	}
}
