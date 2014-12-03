using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	public class PacketHandler : PacketParser
	{
		public bool DispatchMessage(Peer toPassTo, NetBuffer msg)
		{
			LidgrenTransferPacket packet = this.BuildTransferPacket(msg);

			if (toPassTo == null)
				return false;

			if(packet.isEncrypted)
			{
				if(!toPassTo.EncryptionRegister.HasKey(packet.EncryptionMethodByte))
				{
					ClassLogger.LogError("Failed to decrypt packet. Client requested unregistered method: " + packet.EncryptionMethodByte);
					return false;
				}


			}
			else
			{
				Dispatch(toPassTo, packet);
			}
		}

		private void Dispatch(Peer toPassTo, IPackage packet)
		{
			if (this.SerializerRegister.HasKey(packet.SerializerKey))
				//TODO: Refactor
				switch ((Packet.OperationType)packet.OperationType)
				{
					case Packet.OperationType.Event:
						EventPackage ePackage = GeneratePackage<EventPackage>(packet);
						if (ePackage != null)
							toPassTo.PackageRecieve(ePackage);
						break;
					case Packet.OperationType.Request:
						//ClassLogger.LogDebug("Hit request");
						RequestPackage rqPackage = GeneratePackage<RequestPackage>(packet);
						if (rqPackage != null)
						{
							//ClassLogger.LogDebug("About to call peer method");
							toPassTo.PackageRecieve(rqPackage);
						}
						break;
					case Packet.OperationType.Response:
						ResponsePackage rPackage = GeneratePackage<ResponsePackage>(packet);
						if (rPackage != null)
							toPassTo.PackageRecieve(rPackage);
						break;
				}
			else
				ClassLogger.LogError("Recieved a packet that cannot be handled due to not having a serializer registered with byte code: " + packet.SerializerKey);
		}

		private bool TryReadHailMessage(NetIncomingMessage msg, string expected)
		{
			try
			{
				return msg.SenderConnection.RemoteHailMessage != null &&
					expected == msg.SenderConnection.RemoteHailMessage.ReadString();
			}
			catch (NetException e)
			{
				//This exception will occur when we're reading from the buffer but we can't get the expected byte or the hail message
#if DEBUGBUILD
				this.ClassLogger.LogError("Failed to read hail message. Exception: " + e.Message);
#endif
				return false;
			}
		}
	}
}
