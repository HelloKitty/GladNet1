using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections.Readers
{
	public abstract class ClientPeer : Peer
	{
		public ClientPeer(IConnectionDetails details)
			: base(details)
		{

		}

		public override abstract void PackageRecieve(RequestPackage package);

		public override void PackageRecieve(ResponsePackage package)
		{
			throw new LoggableException("ClientPeer recieved a ResponsePackage but Peer cannot handle this message type.", null, Logger.LogType.Error);
		}

		public override void PackageRecieve(EventPackage package)
		{
			throw new LoggableException("ClientPeer recieved a EventPackage but Peer cannot handle this message type.", null, Logger.LogType.Error);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="packetCode"></param>
		/// <param name="deliveryMethod"></param>
		/// <param name="encrypt"></param>
		/// <param name="channel"></param>
		/// <exception cref="LoggableException">Throws a loggable exception generally when packet serialization fails. You should catch this.</exception>
		/// <returns></returns>
		public Packet.SendResult SendEvent(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
		{
			try
			{
				return this.SendMessage(Packet.OperationType.Event, packet, packetCode, deliveryMethod, encrypt, channel);
			}
			catch(LoggableException e)
			{
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="packetCode"></param>
		/// <param name="deliveryMethod"></param>
		/// <param name="encrypt"></param>
		/// <param name="channel"></param>
		/// <exception cref="LoggableException">Throws a loggable exception generally when packet serialization fails. You should catch this.</exception>
		/// <returns></returns>
		public Packet.SendResult SendResponse(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, byte encrypt = 0, int channel = 0)
		{
			try
			{
				return this.SendMessage(Packet.OperationType.Response, packet, packetCode, deliveryMethod, encrypt, channel);
			}
			catch(LoggableException e)
			{
				throw;
			}
		}
	}
}
