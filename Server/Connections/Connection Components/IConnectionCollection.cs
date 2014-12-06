using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	public interface IConnectionCollection<PeerType, LidgrenType> : IEnumerable<ConnectionPair<LidgrenType, PeerType>>,
		IRegisterable<ConnectionPair<LidgrenType, PeerType>, long> where PeerType : Peer
	{
		IList<LidgrenType> ToListOfLidgren();
		IList<PeerType> ToListOfPeer();
	}
}
