using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class PacketHandler : PacketParser
	{
		protected IDictionary<byte, Func<EncryptionBase>> EncryptionFactory;

		public PacketHandler(Logger logger)
			: base(logger)
		{
			EncryptionFactory = new Dictionary<byte, Func<EncryptionBase>>();
		}

		public bool Register<T>(byte key) where T : EncryptionBase, new()
		{
			if (EncryptionFactory.ContainsKey(key))
				return false;

			EncryptionFactory.Add(key, () => { return new T(); });

			return true;
		}

		public bool DispatchMessage(Peer toPassTo, NetBuffer msg, bool isInternal)
		{
			LidgrenTransferPacket packet = this.BuildTransferPacket(msg);

			if (toPassTo == null)
				return false;

			if (!this.SerializerRegister.HasKey(packet.SerializerKey))
			{
				ClassLogger.LogError("Recieved a packet that cannot be handled due to not having a serializer registered with byte code: " + packet.SerializerKey);
				return false;
			}

			if(!isInternal)
				if(packet.isEncrypted)
				{
					return DispatchEncryptedMessage(toPassTo, packet);
				}
				else
				{
					return Dispatch(toPassTo, packet);
				}
			else
				return HandleInternalMessage(toPassTo, packet);
		}

		private bool HandleInternalMessage(Peer toPassTo, LidgrenTransferPacket packet)
		{
			if(packet.isEncrypted)
			{
				if (toPassTo.EncryptionRegister.HasKey(packet.EncryptionMethodByte))
					if (!packet.Decrypt(toPassTo.EncryptionRegister[packet.EncryptionMethodByte]))
					{
						ClassLogger.LogError("Failed to decrypt package from Peer ID: " 
							+ toPassTo.UniqueConnectionId + " with EncryptionByte: " + packet.EncryptionMethodByte);
						return false;
					}
			}

			switch((InternalPacketCode)packet.EncryptionMethodByte)
			{
				case InternalPacketCode.EncryptionRequest:
					return EncryptionRegisterFromWire(toPassTo, packet);
				default:
					return false;
			}
		}

		private bool EncryptionRegisterFromWire(Peer toPassTo, IPackage packet)
		{
			if (packet.InternalByteRepresentation != null)
			{
				EncryptionRequest eq = Serializer<GladNetProtobufNetSerializer>.Instance.
					Deserialize<PacketBase>(packet.InternalByteRepresentation) as EncryptionRequest;

				if (eq != null)
				{
					if (!EncryptionFactory.ContainsKey(eq.EncryptionByteType))
						ClassLogger.LogError("Failed to establish encryption from Peer ID: "
							+ toPassTo.UniqueConnectionId + " with EncryptionByte: " + eq.EncryptionByteType);

					EncryptionBase newEncryptionObj = EncryptionFactory[eq.EncryptionByteType]();

					toPassTo.EncryptionRegister.Register(EncryptionFactory[eq.EncryptionByteType](), eq.EncryptionByteType);

					bool result = toPassTo.EncryptionRegister[eq.EncryptionByteType]
						.SetNetworkInitRequiredData(eq.EncryptionInitInfo);

					if (result)
					{
						EncryptionRequest encryptionResponse =
							new EncryptionRequest(eq.EncryptionByteType, newEncryptionObj.NetworkInitRequiredData());

						toPassTo.SendMessage(Packet.OperationType.Response, encryptionResponse, (byte)InternalPacketCode.EncryptionRequest,
							Packet.DeliveryMethod.ReliableUnordered, 0, 0, true);
					}
				}
			}
			return false;
		}

		private bool DispatchEncryptedMessage(Peer toPassTo, LidgrenTransferPacket packet)
		{
			if (!toPassTo.EncryptionRegister.HasKey(packet.EncryptionMethodByte))
			{
				ClassLogger.LogError("Failed to decrypt packet. Client requested unregistered method: " + packet.EncryptionMethodByte);
				return false;
			}

			try
			{
				switch (packet.OperationType)
				{
					case Packet.OperationType.Event:
						EventPackage ep = this.GeneratePackage<EventPackage>(packet, toPassTo.EncryptionRegister[packet.EncryptionMethodByte]);
						if (ep != null)
							toPassTo.PackageRecieve(ep);
						return true;
					case Packet.OperationType.Request:
						RequestPackage rqp = this.GeneratePackage<RequestPackage>(packet, toPassTo.EncryptionRegister[packet.EncryptionMethodByte]);
						if (rqp != null)
							toPassTo.PackageRecieve(rqp);
						return true;
					case Packet.OperationType.Response:
						ResponsePackage rp = this.GeneratePackage<ResponsePackage>(packet, toPassTo.EncryptionRegister[packet.EncryptionMethodByte]);
						if (rp != null)
							toPassTo.PackageRecieve(rp);
						return true;
					default:
						return false;
				}
			}
			catch(LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? " Inner: " + e.InnerException : "");
				return false;
			}
		}

		private bool Dispatch(Peer toPassTo, IPackage packet)
		{
				//TODO: Refactor
				switch ((Packet.OperationType)packet.OperationType)
				{
					case Packet.OperationType.Event:
						EventPackage ePackage = GeneratePackage<EventPackage>(packet);
						if (ePackage != null)
							toPassTo.PackageRecieve(ePackage);
						return true;
					case Packet.OperationType.Request:
						//ClassLogger.LogDebug("Hit request");
						RequestPackage rqPackage = GeneratePackage<RequestPackage>(packet);
						if (rqPackage != null)
						{
							//ClassLogger.LogDebug("About to call peer method");
							toPassTo.PackageRecieve(rqPackage);
						}
						return true;
					case Packet.OperationType.Response:
						ResponsePackage rPackage = GeneratePackage<ResponsePackage>(packet);
						if (rPackage != null)
							toPassTo.PackageRecieve(rPackage);
						return true;

					default:
						return false;
				}
		}

		public bool TryReadHailMessage(NetIncomingMessage msg, string expected)
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
