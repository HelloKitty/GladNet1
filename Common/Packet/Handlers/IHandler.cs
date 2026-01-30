#region copyright
/// GladNet Copyright (C) 2014 X 
/// X
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
	public interface IHandler<T>
	{
		void Handle(T obj);
	}

	public interface IHandler<ObjectToHandleType, PossibleReturnType>
	{
		PossibleReturnType Handle(ObjectToHandleType obj);
	}
}
