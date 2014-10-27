using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	interface IHandler<T>
	{
		void Handle(T obj);
	}

	interface IHandler<ObjectToHandleType, PossibleReturnType>
	{
		PossibleReturnType Handle(ObjectToHandleType obj);
	}
}
