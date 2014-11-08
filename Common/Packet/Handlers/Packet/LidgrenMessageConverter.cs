using Common.Register;
using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Packet.Handlers
{
	internal class LidgrenMessageConverter : IRegisterable<PacketConverter, byte>
	{
		private Dictionary<byte, PacketConverter> HandlerCollection;

		public LidgrenMessageConverter()
		{
			HandlerCollection = new Dictionary<byte, PacketConverter>();
		}

		public bool Register(PacketConverter obj, byte key)
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

		public PacketConverter GetValue(byte key)
		{
			return HandlerCollection[key];
		}

		public bool HasKey(byte key)
		{
			return HandlerCollection.ContainsKey(key);
		}

		public PacketConverter this[byte key]
		{
			get { return GetValue(key); }
		}
	}
}
