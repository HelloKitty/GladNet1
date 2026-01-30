#region copyright
/// GladNet Copyright (C) 2014 X 
/// X
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

namespace GladNet.Client
{
	public interface IListener
	{
		void RegisterProtobufPackets(Func<Type, bool> registerMethod);

		void RecievePackage(EventPackage eventPackage, MessageInfo info);
		void RecievePackage(ResponsePackage responsePackage, MessageInfo info);

		void OnStatusChange(StatusChange status);
	}
}
