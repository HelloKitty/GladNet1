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
using UnityEngine;

namespace GladNet.Client
{
#if UNITYDEBUG || UNITYRELEASE
	public class UnityLogger : Logger
	{
		public UnityLogger(GladNet.Common.LogType state)
			:base(state)
		{

		}

		protected override void Log(string text, GladNet.Common.LogType state)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").Append(text);

			switch(state)
			{
				case GladNet.Common.LogType.Debug:
					Debug.Log(builder.ToString());
					break;
				case GladNet.Common.LogType.Warn:
					Debug.LogWarning(builder.ToString());
					break;
				case GladNet.Common.LogType.Error:
					Debug.LogError(builder.ToString());
					break;
			}
		}

		protected override void Log(string text, GladNet.Common.LogType state, params object[] data)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").AppendFormat(text, data);

			switch(state)
			{
				case GladNet.Common.LogType.Debug:
					Debug.Log(builder.ToString());
					break;
				case GladNet.Common.LogType.Warn:
					Debug.LogWarning(builder.ToString());
					break;
				case GladNet.Common.LogType.Error:
					Debug.LogError(builder.ToString());
					break;
			}
		}

		protected override void Log(string text, GladNet.Common.LogType state, params string[] data)
		{
			this.Log(text, state, (object[])data);
		}

		protected override void Log(object obj, GladNet.Common.LogType state)
		{
			this.Log((string)(obj == null ? "[NULL]" : obj.ToString()), state);
		}
	}
#endif
}
