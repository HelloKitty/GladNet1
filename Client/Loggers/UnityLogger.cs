using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GladNet.Client
{
#if UNITYDEBUG || UNITYRELEASE
	public class UnityLogger : Logger
	{
		public UnityLogger(LogType state)
			:base(state)
		{

		}

		protected override void Log(string text, Logger.LogType state)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").Append(text);

			switch(state)
			{
				case LogType.Debug:
					Debug.Log(builder.ToString());
					break;
				case LogType.Warn:
					Debug.LogWarning(builder.ToString());
					break;
				case LogType.Error:
					Debug.LogError(builder.ToString());
					break;
			}
		}

		protected override void Log(string text, Logger.LogType state, params object[] data)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").Append(text);

			switch(state)
			{
				case LogType.Debug:
					Debug.LogFormat(builder.ToString(), data);
					break;
				case LogType.Warn:
					Debug.LogWarningFormat(builder.ToString(), data);
					break;
				case LogType.Error:
					Debug.LogErrorFormat(builder.ToString(), data);
					break;
			}
		}

		protected override void Log(string text, Logger.LogType state, string[] data)
		{
			throw new NotImplementedException();
		}

		protected override void Log(object obj, Logger.LogType state)
		{
			throw new NotImplementedException();
		}
	}
#endif
}
