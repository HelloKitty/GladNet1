using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Exceptions
{
	public class SerializationException : Exception
	{
		public readonly Type TypeInvolved;
		public readonly Type Serializer;

		public SerializationException(Type involvedType, Type serializerType, Exception innerException, string message) 
			: base(message, innerException)
		{
			this.TypeInvolved = involvedType;
			this.Serializer = serializerType;
		}
	}
}
