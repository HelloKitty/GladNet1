using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	[Flags]
	public enum LogType
	{
		Disabled = 0,
		Error = 2,
		Warn = 4 | Error,
		Debug = 8 | Warn
	}

	public interface ILogger
	{

		bool isStateEnabled(LogType state);
		//TODO: Documentation
		#region Error Logging methods
		void LogError(string text);
		void LogError(string text, params object[] data);
		void LogError(string text, params string[] data);
		void LogError(object obj);
		#endregion

		//TODO: Documentation
		#region Warning logger methods
		void LogWarn(string text);
		void LogWarn(string text, params object[] data);
		void LogWarn(string text, params string[] data);
		void LogWarn(object obj);
		#endregion

		//TODO: Documentation
		#region Debug logger methods
		void LogDebug(string text);
		void LogDebug(string text, params object[] data);
		void LogDebug(string text, params string[] data);
		void LogDebug(object obj);
		#endregion

	}
}
