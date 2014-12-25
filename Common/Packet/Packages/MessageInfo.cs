using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public sealed class MessageInfo
	{
		public bool wasEncrypted { get; private set; }

		public byte EncryptionMethodType { get; private set; }

		public Packet.DeliveryMethod DeliveryMethod
		{
			get { return Packet.LidgrenDeliveryMethodConvert(deliveryMethod); }
		}

		public MessageInfo(bool wasencrypted, byte encryptionType, NetDeliveryMethod method)
		{
			deliveryMethod = method;
			wasEncrypted = wasencrypted;
			EncryptionMethodType = encryptionType;
		}

		private readonly NetDeliveryMethod deliveryMethod;

#if !UNITDEBUG && !UNITYRELEASE
		internal MessageInfo(LidgrenTransferPacket lidgrenPacket, NetDeliveryMethod delivery)
#else
		public MessageInfo(LidgrenTransferPacket lidgrenPacket, NetDeliveryMethod delivery)
#endif
		{
			if (lidgrenPacket == null)
				throw new ArgumentException("In Ctor of MessageInfo a null packet was passed.");
			deliveryMethod = delivery;
			this.EncryptionMethodType = lidgrenPacket.EncryptionMethodByte;
			this.wasEncrypted = lidgrenPacket.wasEncrypted;
		}

#if !UNITDEBUG && !UNITYRELEASE
		internal MessageInfo(NetDeliveryMethod delivery)
#else
		public MessageInfo(NetDeliveryMethod delivery)
#endif
		{
			deliveryMethod = delivery;
			this.EncryptionMethodType = 0;
			this.wasEncrypted = false;
		}


	}
}
