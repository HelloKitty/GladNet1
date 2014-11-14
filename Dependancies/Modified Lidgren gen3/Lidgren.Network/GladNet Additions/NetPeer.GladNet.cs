using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lidgren.Network
{
	/// <summary>
	/// This partial piece represents GladNet specific additions to the NetPeer Lidgren source.
	/// </summary>
	public partial class NetPeer
	{
		/*public HighLevelMessage ReadHighlevelMessage()
		{
			HighLevelMessage retval;
			m_releasedIncomingHighlevelMessages.TryDequeue(out retval);
			return retval;
		}*/

		/*protected void ProcessHighlevelMessage(NetIncomingMessage msg)
		{
			try
			{
				//This is not required.
				//We are not interested in the first byte anymore.
				//msg.Position += 8;
#if DEBUGBUILD
				//TODO: Implement debug specific implementation.
#else
				//Deletete the internal message indentifier now.
				msg.ReadByte();
				//Add safety check to make sure this packet isn't malformed.
				Console.WriteLine("");
				foreach(byte b in msg.ReadBytes(msg.LengthBytes -1))
				{
					Console.Write(b + " ");
				}
				if (msg.LengthBytes > 1)
					m_releasedIncomingHighlevelMessages.Enqueue(new HighLevelMessage(msg.ReadBytes(msg.LengthBytes -1), msg));
#endif
			}
			catch (Exception e)
			{
				LogError("HigherLevelPacket failed to parse the bytes. Error: " + e.Message);
			}
		}*/

		public void SendMessage(bool isInternalMessage, IList<NetConnection> recipients, byte[] packet, NetDeliveryMethod method, int sequenceChannel)
		{
			NetOutgoingMessage msg = this.CreateMessage();
			msg.Write((byte)(isInternalMessage ? 0 : 255));
			msg.Write(packet);
			this.SendMessage(msg, recipients, method, sequenceChannel);
		}
	}
}
