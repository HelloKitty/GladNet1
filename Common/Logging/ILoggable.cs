using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Server.Logging
{
	/*public enum LoggerState
	{
		None,
		Debug,
		Error,
		Warn
	}*/

	public interface ILoggable
	{
		Logger ClassLogger { get; }
	}
}
