using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Logging
{
	public abstract class Logger
	{
		[Flags]
		public enum State
		{
			Disabled = 0,
			Error = 2,
			Warn = 4 | Error,
			Debug = 8 | Warn
		}

		/// <summary>
		/// Indicates which state the logger is in.
		/// </summary>
		public State LoggerState { get; protected set; }

		public Logger(State loggerState)
		{
			LoggerState = loggerState;
		}

		/// <summary>
		/// Determines if the logger can log for a given state
		/// </summary>
		/// <param name="state">A logger state.</param>
		/// <returns>Indicates if the logger is in the given state.</returns>
		bool isStateEnabled(State state)
		{
			return LoggerState.HasFlag(state);
		}

		//TODO: Documentation
		#region Error Logging methods
		public void LogError(string text)
		{
			if (this.isStateEnabled(State.Error))
				Log(text, State.Error);
		}
		public void LogError(string text, object[] data)
		{
			if (this.isStateEnabled(State.Error))
				Log(text, data, State.Error);
		}
		public void LogError(string text, string[] data)
		{
			if (this.isStateEnabled(State.Error))
				Log(text, data, State.Error);
		}
		public void LogError(object obj)
		{
			if (this.isStateEnabled(State.Error))
				Log(obj, State.Error);
		}
		#endregion

		//TODO: Documentation
		#region Warning logger methods
		public void LogWarn(string text)
		{
			if (this.isStateEnabled(State.Warn))
				Log(text, State.Warn);
		}
		public void LogWarn(string text, object[] data)
		{
			if (this.isStateEnabled(State.Warn))
				Log(text, data, State.Warn);
		}
		public void LogWarn(string text, string[] data)
		{
			if (this.isStateEnabled(State.Warn))
				Log(text, data, State.Warn);
		}
		public void LogWarn(object obj)
		{
			if (this.isStateEnabled(State.Warn))
				Log(obj, State.Warn);
		}
		#endregion

		//TODO: Documentation
		#region Debug logger methods
		public void LogDebug(string text)
		{
			if (this.isStateEnabled(State.Debug))
				Log(text, State.Debug);
		}
		public void LogDebug(string text, object[] data)
		{
			if (this.isStateEnabled(State.Debug))
				Log(text, data, State.Debug);
		}
		public void LogDebug(string text, string[] data)
		{
			if (this.isStateEnabled(State.Debug))
				Log(text, data, State.Debug);
		}
		public void LogDebug(object obj)
		{
			if (this.isStateEnabled(State.Debug))
				Log(obj, State.Debug);
		}
		#endregion

		#region True logger methods
		protected abstract void Log(string text, State state);
		protected abstract void Log(string text, object[] data, State state);
		protected abstract void Log(string text, string[] data, State state);
		protected abstract void Log(object obj, State state);
		#endregion
	}
}
