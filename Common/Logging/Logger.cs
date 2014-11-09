﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public abstract class Logger
	{
		[Flags]
		public enum LogType
		{
			Disabled = 0,
			Error = 2,
			Warn = 4 | Error,
			Debug = 8 | Warn
		}

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
		bool isStateEnabled(LogType state)
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
		public void LogError(string text, object[] data)
		{
			if (this.isStateEnabled(LogType.Error))
				Log(text, data, LogType.Error);
		}
		public void LogError(string text, string[] data)
		{
			if (this.isStateEnabled(LogType.Error))
				Log(text, data, LogType.Error);
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
		public void LogWarn(string text, object[] data)
		{
			if (this.isStateEnabled(LogType.Warn))
				Log(text, data, LogType.Warn);
		}
		public void LogWarn(string text, string[] data)
		{
			if (this.isStateEnabled(LogType.Warn))
				Log(text, data, LogType.Warn);
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
		public void LogDebug(string text, object[] data)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(text, data, LogType.Debug);
		}
		public void LogDebug(string text, string[] data)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(text, data, LogType.Debug);
		}
		public void LogDebug(object obj)
		{
			if (this.isStateEnabled(LogType.Debug))
				Log(obj, LogType.Debug);
		}
		#endregion

		#region True logger methods
		protected abstract void Log(string text, LogType state);
		protected abstract void Log(string text, object[] data, LogType state);
		protected abstract void Log(string text, string[] data, LogType state);
		protected abstract void Log(object obj, LogType state);
		#endregion
	}
}