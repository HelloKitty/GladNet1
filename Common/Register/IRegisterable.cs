#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using GladNet.Common.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IRegisterable<RegisterType, KeyType> : IHashContainer<RegisterType, KeyType>
	{
		RegisterType this[KeyType key] { get; }
		bool Register(RegisterType obj, KeyType key);
		bool UnRegister(KeyType key);

		void Clear();
	}

	public interface IRegisterable<RegisterType>
	{
		bool Register(RegisterType obj);
		bool UnRegister(RegisterType key);
	}
}
