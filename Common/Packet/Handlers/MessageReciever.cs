#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using GladNet.Common.Serializers;
using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/*public abstract class MessageReciever
	{
		protected readonly PacketParser NetworkMessageHandler;

		public MessageReciever(Logger logger)
		{
			NetworkMessageHandler = new PacketParser(logger);
		}

		public bool RegisterProtobufPacket(Type t)
		{
			return Packet.Register(t, false);
		}

		/// <summary>
		/// Called internally when the application starts up. The implementing class should register all the custom Protobuf-net (The default serializer for the Packet base class)
		/// packet classes so that the serializer is aware of them for both sending and recieving the packages. All internal packages that aren't written by the implementing library are handled
		/// internally.
		/// </summary>
		/// <param name="registerAsDefaultFunc">The defauly packet registeration function.</param>
		protected abstract void RegisterProtobufPackets(Func<Type, bool> registerAsDefaultFunc);
	}*/
}
