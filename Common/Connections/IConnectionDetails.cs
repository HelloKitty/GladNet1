#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GladNet.Common
{
	public interface IConnectionDetails
	{
		IPEndPoint RemoteConnectionEndpoint { get; }
		long UniqueConnectionId { get; }
		NetConnection InternalNetConnection { get; }
	}
}
