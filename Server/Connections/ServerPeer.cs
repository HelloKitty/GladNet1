#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using GladNet.Common;
using GladNet.Server.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Connections
{
	public abstract class ServerPeer : Peer, ILoggable
	{
		public Logger ClassLogger { get; protected set; }

		public bool isAuthenticated { get; protected set; }

		public ServerPeer(IConnectionDetails details, Logger logger) 
			: base(details)
		{
			
		}

		public override void PackageRecieve(RequestPackage package)
		{
			throw new LoggableException("ServerPeer base recieved a RequestPackage but ServerPeer cannot handle this message type.", null, Logger.LogType.Error);
		}

		public override abstract void PackageRecieve(EventPackage package);

		public override abstract void PackageRecieve(ResponsePackage package);

		public bool EstablishEncryption(Action OnSuccess)
		{
			return this.Register(new DiffieHellmanAESEncryptor(), OnSuccess);
		}

		//This is not needed by the client
		/// <summary>
		/// Registers an encryption object. This will request that the encryption be established by sending a message to the server.
		/// Do not try to register an encryption object if you are not connected.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="encryptorInstance"></param>
		/// <returns></returns>
		public bool Register<T>(T encryptorInstance, Action OnSuccess) where T : EncryptionBase
		{
			if (encryptorInstance == null)
			{
				ClassLogger.LogError("Cannot register a null encryption object.");
				return false;
			}

			if (this.EncryptionRegister.HasKey(encryptorInstance.EncryptionTypeByte))
			{
				ClassLogger.LogError("Tried to register an already known encryption object.");
				return false;
			}

			encryptorInstance.OnEstablished += OnSuccess;

			this.EncryptionRegister.Register(encryptorInstance, encryptorInstance.EncryptionTypeByte);

			PacketBase packet = new EncryptionRequest(encryptorInstance.EncryptionTypeByte, encryptorInstance.NetworkInitRequiredData());

			return this.SendMessage(Packet.OperationType.Request, packet, (byte)InternalPacketCode.EncryptionRequest,
				Packet.DeliveryMethod.ReliableUnordered, 0, 0, true) != Packet.SendResult.FailedNotConnected;
		}
	}
}
