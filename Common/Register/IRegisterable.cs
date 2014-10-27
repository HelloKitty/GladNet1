using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IRegisterable<RegisterType, KeyType>
	{
		bool Register(RegisterType obj, KeyType key);
		bool UnRegister(KeyType key);
	}

	public interface IRegisterable<RegisterType>
	{
		bool Register(RegisterType obj);
	}
}
