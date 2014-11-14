using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lidgren.Network
{
	public partial class NetConnection
	{
		/// <summary>
		/// GladNet overload for sending a packet accross the wire in GladNet form. (Not used internally in Lidgren)
		/// </summary>
		/// <param name="isInternalMessage">Indicates whether the message is for internal handling or if it should be queued as a high level packet.</param>
		/// <param name="packet">Byte[] of the package to send.</param>
		/// <param name="method">The delivery method to be used to send the message.</param>
		/// <param name="sequenceChannel">The channel to use within the method.</param>
		public NetSendResult SendMessage(bool isInternalMessage, byte[] packet, NetDeliveryMethod method, int sequenceChannel)
		{
			NetOutgoingMessage msg = this.Peer.CreateMessage();
			msg.Write((byte)(isInternalMessage ? 0 : 255));
			msg.Write(packet);
			return this.Peer.SendMessage(msg, this, method, sequenceChannel);
		}
	}
}
