using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IConverter<FromType, ToType> where ToType : class, new()
	{
		ToType Convert(FromType obj);
	}

	public interface IConverter<FromType1, FromType2, ToType> where ToType : class, new()
	{
		ToType Convert(FromType1 obj1, FromType2 obj2);
	}

	public interface IConverter<FromType1, FromType2, FromType3, ToType> where ToType : class, new()
	{
		ToType Convert(FromType1 obj1, FromType2 obj2, FromType3 obj3);
	}
}
