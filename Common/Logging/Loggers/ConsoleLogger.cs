using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Logging.Loggers
{
	public class ConsoleLogger : Logger
	{

	//Can't use Lazy<T> in Unity
#if !UNITYDEBUG && !UNITYRELEASE
		/// <summary>
		/// Lazily loaded instance of the serializer.
		/// </summary>
		private static Lazy<ConsoleLogger> _Instance = new Lazy<ConsoleLogger>(() => { return new ConsoleLogger(LogType.Debug); }, true);

#else
		private static ConsoleLogger _Instance = new ConsoleLogger(LogType.Debug);
#endif

//Can't use Lazy<T> in Unity
#if !UNITYDEBUG && !UNITYRELEASE
		/// <summary>
		/// Public singleton access for the serializer instance.
		/// </summary>
		public static ConsoleLogger Instance { get { return _Instance.Value; } }
#else
		public static ConsoleLogger Instance { get { return ConsoleLogger._Instance; } }
#endif

		public ConsoleLogger(LogType state)
			: base(state)
		{

		}

		protected override void Log(string text, Logger.LogType state)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").Append(text);

			Console.WriteLine(builder.ToString());
		}

		protected override void Log(string text, Logger.LogType state, params object[] data)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").AppendFormat(text, data);

			Console.WriteLine(builder.ToString());
		}

		protected override void Log(string text, Logger.LogType state, params string[] data)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").AppendFormat(text, data);

			Console.WriteLine(builder.ToString());
		}

		protected override void Log(object obj, Logger.LogType state)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ").Append(obj == null ? "[NULL]" : obj.ToString());

			Console.WriteLine(builder.ToString());
		}
	}
}
