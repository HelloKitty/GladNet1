using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class PacketHandler : PacketParser
	{
		protected IDictionary<byte, Func<EncryptionBase>> EncryptionFactory;

		public PacketHandler(ILogger logger)
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

		public virtual bool DispatchMessage(Peer toPassTo, NetIncomingMessage msg, bool isInternal)
		{
			
			if (msg == null)
				throw new LoggableException("When dispatched NetBuffer was found to be null.", null, LogType.Error);

			LidgrenTransferPacket packet = this.BuildTransferPacket(msg);

			if (toPassTo == null)
				return false;

			if (!this.SerializerRegister.HasKey(packet.SerializerKey))
			{
				ClassLogger.LogError("Recieved a packet that cannot be handled due to not having a serializer registered with byte code: " + packet.SerializerKey);
				return false;
			}

			if (packet == null)
			{
				ClassLogger.LogError("Lidgren packet built to null.");
				return false;
			}

			if (toPassTo == null)
			{
				ClassLogger.LogError("When attempted to dispatch the Peer passed was found to be null.");
				return false;
			}

#if UNITYDEBUG || DEBUG
			this.ClassLogger.LogDebug("About to handle packet. Encrypted: " + packet.isEncrypted.ToString() + " EncryptionCode: " + packet.EncryptionMethodByte);
#endif

			if (!isInternal)
				if (packet.isEncrypted)
				{
					return DispatchEncryptedMessage(toPassTo, packet, msg.DeliveryMethod);
				}
				else
				{
					return Dispatch(toPassTo, packet, msg.DeliveryMethod);
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

			PacketBase deserializedPacketBase = InternalPacketConstructor(packet);

			switch(packet.OperationType)
			{
				case Packet.OperationType.Request:
					return this.ProcessInternalRequest((InternalPacketCode)packet.PacketCode, deserializedPacketBase, toPassTo);
				case Packet.OperationType.Response:
					return this.ProcessInternalResponse((InternalPacketCode)packet.PacketCode, deserializedPacketBase, toPassTo);
				case Packet.OperationType.Event:
					throw new LoggableException("GladNet currently does not support internal events.", null, LogType.Error);
				default:
					return false;
			}
		}

		private PacketBase InternalPacketConstructor(IPackage package)
		{
			SerializerBase serializer = this.SerializerRegister[package.SerializerKey];

			if (serializer == null)
			{
				ClassLogger.LogError("Failed to deserialize internal message with serializer key: " + package.SerializerKey);
				return null;
			}

			PacketBase packet = this.Converter.PacketConstructor(package.InternalByteRepresentation, serializer);

			if (packet == null)
			{
				ClassLogger.LogError("Recieved a null internal package. Code: {0} SerializerKey: {1}",
					package.PacketCode, package.SerializerKey);
			}

			return packet;
		}

		protected virtual bool ProcessInternalRequest(InternalPacketCode code, PacketBase packet, Peer peer)
		{
			switch (code)
			{
				case InternalPacketCode.EncryptionRequest:
					return EncryptionRegisterFromWire(peer, packet);
				default:
					return false;
			}
		}

		protected virtual bool ProcessInternalResponse(InternalPacketCode code, PacketBase packet, Peer peer)
		{
			switch(code)
			{
				case InternalPacketCode.EncryptionRequest:
					return ProcessEncryptionResponse(peer, packet);
				default:
					return false;
			}
		}

		private bool ProcessEncryptionResponse(Peer peer, PacketBase packet)
		{
			EncryptionRequest eq = packet as EncryptionRequest;

			if (eq == null)
			{
				ClassLogger.LogError("Recieved encryption request with null packet.");
				return false;
			}

			if(!peer.EncryptionRegister.HasKey(eq.EncryptionByteType))
			{
				ClassLogger.LogError("Recieved an encryption request response from the server for ByteType: {0} but the client is unaware of that type."
					, eq.EncryptionByteType);
				return false;
			}

			//TODO: Verify that this is working. Callback at one point was not working.
			//This will set the server's init info. In the case of the default, for example, it will set the Diffiehelmman public key.
			//With this the server and client have established a shared secret and can now pass messages, with the IV, to eachother securely.
			//In the case of a custom method it is User defined and should be referenced.
			bool result = peer.EncryptionRegister[eq.EncryptionByteType].SetNetworkInitRequiredData(eq.EncryptionInitInfo);

			Action callback = peer.EncryptionRegister[eq.EncryptionByteType].OnEstablished;

			if (callback != null)
			{
				callback();
			}

			return result;
		}

		private bool EncryptionRegisterFromWire(Peer toPassTo, PacketBase packet)
		{
			EncryptionRequest eq = packet as EncryptionRequest;

			if(eq == null)
			{
				ClassLogger.LogError("Recieved encryption request with null packet.");
				return false;
			}

			if (!EncryptionFactory.ContainsKey(eq.EncryptionByteType))
			{
				ClassLogger.LogError("Failed to establish encryption from Peer ID: "
					+ toPassTo.UniqueConnectionId + " with EncryptionByte: " + eq.EncryptionByteType);
				return false;
			}

			EncryptionBase newEncryptionObj = EncryptionFactory[eq.EncryptionByteType]();

			toPassTo.EncryptionRegister.Register(newEncryptionObj, eq.EncryptionByteType);

			bool result = toPassTo.EncryptionRegister[eq.EncryptionByteType]
				.SetNetworkInitRequiredData(eq.EncryptionInitInfo);

			if (result)
			{
				EncryptionRequest encryptionResponse =
					new EncryptionRequest(eq.EncryptionByteType, newEncryptionObj.NetworkInitRequiredData());

				toPassTo.SendMessage(Packet.OperationType.Response, encryptionResponse, (byte)InternalPacketCode.EncryptionRequest,
					Packet.DeliveryMethod.ReliableUnordered, 0, 0, true);
			}

			return false;
		}

		private bool DispatchEncryptedMessage(Peer toPassTo, LidgrenTransferPacket packet, NetDeliveryMethod method)
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
							toPassTo.PackageRecieve(ep, new MessageInfo(packet, method));
						return true;
					case Packet.OperationType.Request:
						RequestPackage rqp = this.GeneratePackage<RequestPackage>(packet, toPassTo.EncryptionRegister[packet.EncryptionMethodByte]);
						if (rqp != null)
							toPassTo.PackageRecieve(rqp, new MessageInfo(packet, method));
						return true;
					case Packet.OperationType.Response:
						ResponsePackage rp = this.GeneratePackage<ResponsePackage>(packet, toPassTo.EncryptionRegister[packet.EncryptionMethodByte]);
						if (rp != null)
							toPassTo.PackageRecieve(rp, new MessageInfo(packet, method));
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

		private bool Dispatch(Peer toPassTo, IPackage packet, NetDeliveryMethod method)
		{
			try
			{
				//TODO: Refactor
				switch ((Packet.OperationType)packet.OperationType)
				{
					case Packet.OperationType.Event:
						EventPackage ePackage = GeneratePackage<EventPackage>(packet);
						if (ePackage != null)
							toPassTo.PackageRecieve(ePackage, new MessageInfo(method));
						return true;
					case Packet.OperationType.Request:
						//ClassLogger.LogDebug("Hit request");
						RequestPackage rqPackage = GeneratePackage<RequestPackage>(packet);
						if (rqPackage != null)
						{
							//ClassLogger.LogDebug("About to call peer method");
							toPassTo.PackageRecieve(rqPackage, new MessageInfo(method));
						}
						return true;
					case Packet.OperationType.Response:
						ResponsePackage rPackage = GeneratePackage<ResponsePackage>(packet);
						if (rPackage != null)
							toPassTo.PackageRecieve(rPackage, new MessageInfo(method));
						return true;

					default:
						return false;
				}
			}
			catch (LoggableException e)
			{
				ClassLogger.LogError(e.Message + e.InnerException != null ? " Inner: " + e.InnerException : "");
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
