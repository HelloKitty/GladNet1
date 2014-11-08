using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Implementation for loggable non-crashing exceptions. Exceptions raised that should be logged for the user of the library
	/// but that doesn't indicate an immediate need to terminate the runtime.
	/// </summary>
	public class LoggableException : Exception
	{
		public Logger.LogType LoggableType { get; private set; }

		public bool hasInner
		{
			get { return InnerException != null; }
		}
		

		public LoggableException(string message, Exception inner, Logger.LogType type)
			: base(message, inner)
		{
			this.LoggableType = type;
		}
	}
}
