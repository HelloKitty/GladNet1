using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	internal interface IListener : IPollable
	{
		bool isListening { get; }

		void Start();

		int ListenerCount { get; }

		void DisconnectListeners(string reason = "Unknown");
	}
}
