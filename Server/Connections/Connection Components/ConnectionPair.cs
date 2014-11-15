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
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	public class ConnectionPair<LidgrenType, PeerType> where PeerType : Peer
	{
		public PeerType HighlevelPeer { get; private set; }
		internal LidgrenType LidgrenPeer { get; set; }

		public static implicit operator PeerType(ConnectionPair<LidgrenType, PeerType> obj)
		{
			return obj.HighlevelPeer;
		}

		public ConnectionPair(LidgrenType l, PeerType p)
		{
			HighlevelPeer = p;
			LidgrenPeer = l;
		}
	}
}
