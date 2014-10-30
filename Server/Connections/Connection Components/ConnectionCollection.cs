using Common.Register;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	public class ConnectionCollection<PeerType> : //IEnumerable<ConnectionPair<NetConnection, PeerType>>,
		IRegisterable<ConnectionPair<NetConnection, PeerType>, long> where PeerType : Peer
	{
		private readonly List<PeerType> RegisteredPeers;
		private readonly List<NetConnection> RegisteredNetConnections;

		private readonly IDictionary<long, ConnectionPair<NetConnection, PeerType>> InternalPeerTable;

		internal ConnectionCollection()
		{
			this.RegisteredNetConnections = new List<NetConnection>();
			this.RegisteredPeers = new List<PeerType>();
			InternalPeerTable = new Dictionary<long, ConnectionPair<NetConnection, PeerType>>();
		}

		/*public IEnumerator<ConnectionPair<NetConnection, PeerType>> GetEnumerator()
		{
			//This is fucking terrible, never call this.
			//Almost not even worth implementing IEnumerable
			return this.InternalPeerTable.Select(x => x.Value).GetEnumerator();
		}*/

		//Hacky workaround for not being able to implement IEnumerable<T> for both PeerType and NetConnection.
		public static implicit operator List<PeerType>(ConnectionCollection<PeerType> cc)
		{
			return cc.RegisteredPeers;
		}

		public static implicit operator List<NetConnection>(ConnectionCollection<PeerType> cc)
		{
			return cc.RegisteredNetConnections;
		}

		/*IEnumerator IEnumerable.GetEnumerator()
		{
			//This is fucking terrible, never call this.
			return this.InternalPeerTable.Select(x => x.Value).GetEnumerator();
		}*/

		public ConnectionPair<NetConnection, PeerType> Get(long key)
		{
			return this.InternalPeerTable[key];
		}

		public bool HasKey(long key)
		{
			return this.InternalPeerTable.ContainsKey(key);
		}

		public bool Register(NetConnection netConnection, PeerType peer, long key)
		{
			if (HasKey(key))
				return false;

			return this.Register(new ConnectionPair<NetConnection, PeerType>(netConnection, peer), key);
		}

		public bool Register(ConnectionPair<NetConnection, PeerType> obj, long key)
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

			return this.RegisteredPeers.Remove(InternalPeerTable[key].HighlevelPeer) && this.RegisteredNetConnections.Remove(InternalPeerTable[key].LidgrenPeer)
				&& InternalPeerTable.Remove(key);
		}
	}
}
