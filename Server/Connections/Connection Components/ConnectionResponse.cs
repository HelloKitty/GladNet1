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
		public long UniqueConnectionId
		{
			get { return InternalNetConnection != null ? InternalNetConnection.RemoteUniqueIdentifier : 0; }
		}

		public NetConnection InternalNetConnection { get; private set; }

		public readonly string HailUsed;

		public readonly object CallbackObj;

		internal bool Result { get; set; }

		public ConnectionResponse(IPEndPoint point, NetConnection connection, string hail, object callback)//, byte connectionType)
		{
			this.RemoteConnectionEndpoint = point;
			this.InternalNetConnection = connection;
			HailUsed = hail;
			CallbackObj = callback;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("{0}:{1} ID: {2}", this.RemoteConnectionEndpoint.Address, this.RemoteConnectionEndpoint.Port, this.UniqueConnectionId);
			return builder.ToString();
		}
	}
}
