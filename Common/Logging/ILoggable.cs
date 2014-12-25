#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
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
		ILogger ClassLogger { get; }
	}
}
