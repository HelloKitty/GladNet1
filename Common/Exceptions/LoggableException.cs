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

namespace GladNet.Common
{
	/// <summary>
	/// Implementation for loggable non-crashing exceptions. Exceptions raised that should be logged for the user of the library
	/// but that doesn't indicate an immediate need to terminate the runtime.
	/// </summary>
	public class LoggableException : Exception
	{
		public LogType LoggableType { get; private set; }

		public bool hasInner
		{
			get { return InnerException != null; }
		}
		

		public LoggableException(string message, Exception inner, LogType type)
			: base(message, inner)
		{
			this.LoggableType = type;
		}
	}
}
