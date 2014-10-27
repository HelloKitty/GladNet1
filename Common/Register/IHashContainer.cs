using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Register
{
	public interface IHashContainer<TObj, TKey>
	{
		TObj Get(TKey key);
		bool HasKey(TKey key);
	}
}
