#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public abstract class Logger : ILogger
	{
		/// <summary>
		/// Indicates which state the logger is in.
		/// </summary>
		public LogType LoggerState { get; protected set; }

		public Logger(LogType loggerState)
		{
			LoggerState = loggerState;
		}

		/// <summary>
		/// Determines if the logger can log for a given state
		/// </summary>
		/// <param name="state">A logger state.</param>
		/// <returns>Indicates if the logger is in the given state.</returns>
		public bool isStateEnabled(LogType state)
		{
			return (LoggerState & state) == state;
		}

		//TODO: Documentation
		#region Error Logging methods
		public void LogError(string text)
		{
			if (this.isStateEnabled(LogType.Error))
				Log(text, LogType.Error);
		}
		public void LogError(string text, params object[] data)
		{
			if (this.isStateEnabled(LogType.Error))
				Log(text, LogType.Error, data);
		}
		public void LogError(string text, params string[] data)
		{
			if (this.isStateEnabled(LogType.Error))
				Log(text, LogType.Error, data);
		}
		public void LogError(object obj)
		{
			if (this.isStateEnabled(LogType.Error))
				Log(obj, LogType.Error);
		}
		#endregion

		//TODO: Documentation
		#region Warning logger methods
		public void LogWarn(string text)
		{
			if (this.isStateEnabled(LogType.Warn))
				Log(text, LogType.Warn);
		}
		public void LogWarn(string text, params object[] data)
		{
			if (this.isStateEnabled(LogType.Warn))
				Log(text, LogType.Warn, data);
		}
		public void LogWarn(string text, params string[] data)
		{
			if (this.isStateEnabled(LogType.Warn))
				Log(text, LogType.Warn, data);
		}
		public void LogWarn(object obj)
		{
			if (this.isStateEnabled(LogType.Warn))
				Log(obj, LogType.Warn);
		}
		#endregion

		//TODO: Documentation
		#region Debug logger methods
		public void LogDebug(string text)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(text, LogType.Debug);
		}
		public void LogDebug(string text, params object[] data)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(text, LogType.Debug, data);
		}
		public void LogDebug(string text, params string[] data)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(text, LogType.Debug, data);
		}
		public void LogDebug(object obj)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(obj, LogType.Debug);
		}
		#endregion

		#region True logger methods
		protected abstract void Log(string text, LogType state);
		protected abstract void Log(string text, LogType state, params object[] data);
		protected abstract void Log(string text, LogType state, params string[] data);
		protected abstract void Log(object obj, LogType state);
		#endregion
	}
}
