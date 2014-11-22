using GladNet.Common;
using GladNet.Server.Connections;
using GladNet.Server.Connections.Readers;
using GladNet.Server.Logging;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	internal class HighlevelMessageHandler : ILoggable
	{
		private readonly IConnectionCollection<ServerPeer, NetConnection> Servers;
		private readonly IConnectionCollection<ClientPeer, NetConnection> Clients;

		public Logger ClassLogger { get; private set; }

		public HighlevelMessageHandler(IConnectionCollection<ServerPeer, NetConnection> servers, IConnectionCollection<ClientPeer, NetConnection> clients, Logger logger)
		{
			Servers = servers;
			Clients = clients;
			ClassLogger = logger;
		}

		public NetConnectionStatus GetStatus(NetIncomingMessage msg)
		{
			if (msg.MessageType == NetIncomingMessageType.StatusChanged)
			{
				try
				{
					return (NetConnectionStatus)msg.ReadByte();
				}
				catch (NetException e)
				{
					//Catch and drop the exception; We want to return None and log something just as if it wasn't a statuschanged message.
				}
			}

			ClassLogger.LogWarn("Failed to get status byte from message with client ID: {0}", msg.SenderConnection.RemoteUniqueIdentifier);
			return NetConnectionStatus.None;
		}
	}
}
