#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using Common.Register;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	public class ConnectionCollection<PeerType, LidgrenType> : IConnectionCollection<PeerType, LidgrenType> where PeerType : Peer
	{
		private readonly List<PeerType> RegisteredPeers;
		private readonly List<LidgrenType> RegisteredNetConnections;

		private readonly ConcurrentDictionary<long, ConnectionPair<LidgrenType, PeerType>> InternalPeerTable;

		internal ConnectionCollection()
		{
			this.RegisteredNetConnections = new List<LidgrenType>();
			this.RegisteredPeers = new List<PeerType>();
			InternalPeerTable = new ConcurrentDictionary<long, ConnectionPair<LidgrenType, PeerType>>();
		}

		//Hacky workaround for not being able to implement IEnumerable<T> for both PeerType and NetConnection.
		public static implicit operator List<PeerType>(ConnectionCollection<PeerType, LidgrenType> cc)
		{
			return cc.RegisteredPeers;
		}

		public static implicit operator List<LidgrenType>(ConnectionCollection<PeerType, LidgrenType> cc)
		{
			return cc.RegisteredNetConnections;
		}

		public ConnectionPair<LidgrenType, PeerType> GetValue(long key)
		{
			return this.InternalPeerTable[key];
		}

		public bool HasKey(long key)
		{
			return this.InternalPeerTable.ContainsKey(key);
		}

		public bool Register(LidgrenType netConnection, PeerType peer, long key)
		{
			if (HasKey(key))
				return false;

			return this.Register(new ConnectionPair<LidgrenType, PeerType>(netConnection, peer), key);
		}

		public bool Register(ConnectionPair<LidgrenType, PeerType> obj, long key)
		{
			if (HasKey(key))
				return false;

			this.InternalPeerTable[key] = obj;

			//Also add these elements to the internal lists for easy iterating
			RegisteredPeers.Add(obj.HighlevelPeer);
			RegisteredNetConnections.Add(obj.LidgrenPeer);

			return true;
		}

		public bool UnRegister(long key)
		{
			if(!HasKey(key))
				return false;

			ConnectionPair<LidgrenType, PeerType> TempPair;

			return this.RegisteredPeers.Remove(InternalPeerTable[key].HighlevelPeer) && this.RegisteredNetConnections.Remove(InternalPeerTable[key].LidgrenPeer)
				&& InternalPeerTable.TryRemove(key, out TempPair);
		}

		public ConnectionPair<LidgrenType, PeerType> this[long key]
		{
			get { return GetValue(key); }
		}

		public IEnumerator<ConnectionPair<LidgrenType, PeerType>> GetEnumerator()
		{
			return InternalPeerTable.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return InternalPeerTable.GetEnumerator();
		}

		public IList<LidgrenType> ToListOfLidgren()
		{
			return RegisteredNetConnections;
		}

		public IList<PeerType> ToListOfPeer()
		{
			return RegisteredPeers;
		}
	}
}
