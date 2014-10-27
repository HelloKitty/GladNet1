using Common.Register;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packet.Handlers
{
	internal class LidgrenMessageConverter : IRegisterable<HigherLevelPacketHandlerBase, byte>, IHashContainer<HigherLevelPacketHandlerBase, byte>
	{
		private Dictionary<byte, HigherLevelPacketHandlerBase> HandlerCollection;

		public LidgrenMessageConverter()
		{
			HandlerCollection = new Dictionary<byte, HigherLevelPacketHandlerBase>();
		}

		public bool Register(HigherLevelPacketHandlerBase obj, byte key)
		{
			if(HandlerCollection.ContainsKey(key))
				return false;

			HandlerCollection.Add(key, obj);
			return true;
		}

		public bool UnRegister(byte key)
		{
			return this.HandlerCollection.Remove(key);
		}

		public HigherLevelPacketHandlerBase Get(byte key)
		{
			return HandlerCollection[key];
		}

		public bool HasKey(byte key)
		{
			return HandlerCollection.ContainsKey(key);
		}
	}
}
