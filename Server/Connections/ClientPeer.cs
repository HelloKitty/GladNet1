#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
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

		public Packet.SendResult SendEvent(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel = 0, bool encrypt = false)
		{
			return this.SendEvent(packet, packetCode, deliveryMethod, channel, encrypt ? EncryptionBase.DefaultByte : EncryptionBase.NoEncryptionByte);
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
		public Packet.SendResult SendEvent(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel = 0, byte encrypt = EncryptionBase.NoEncryptionByte)
		{
			try
			{
				return this.SendMessage(Packet.OperationType.Event, packet, packetCode, deliveryMethod, channel, encrypt);
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
		public Packet.SendResult SendResponse(PacketBase packet, byte packetCode, Packet.DeliveryMethod deliveryMethod, int channel = 0, byte encrypt = EncryptionBase.NoEncryptionByte)
		{
			try
			{
				return this.SendMessage(Packet.OperationType.Response, packet, packetCode, deliveryMethod, channel, encrypt);
			}
			catch(LoggableException e)
			{
				throw;
			}
		}
	}
}
