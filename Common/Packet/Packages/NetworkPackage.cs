#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkPackage
	{
		public byte EncryptionMethodByte { get; internal set; }
		public byte Code { get; protected set; }
		public PacketBase PacketObject { get; protected set; }

		public void FillPackage(PacketBase packet, byte code)
		{
			PacketObject = packet;
			Code = code;
		}
	}
}
