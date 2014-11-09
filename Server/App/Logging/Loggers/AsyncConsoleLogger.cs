using GladNet.Common;
using GladNet.Server.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server
{
	public sealed class AsyncConsoleLogger : Logger
	{
		private BlockingCollection<string> BlockingQueue;

		/// <summary>
		/// Lazily loaded instance of the serializer.
		/// </summary>
		private static Lazy<AsyncConsoleLogger> _Instance = new Lazy<AsyncConsoleLogger>(() => { return new AsyncConsoleLogger(); }, true);

		/// <summary>
		/// Public singleton access for the serializer instance.
		/// </summary>
		public static AsyncConsoleLogger Instance { get { return _Instance.Value; } }

		private AsyncConsoleLogger() 
			: base(Logger.LogType.Error)
		{
			BlockingQueue = new BlockingCollection<string>();
		}

		public void SetState(Logger.LogType newState)
		{
			this.LoggerState = newState;
			Task.Factory.StartNew(() =>
				{
					while(true) Console.WriteLine(BlockingQueue.Take());
				}, TaskCreationOptions.LongRunning);
		}

		protected override void Log(string text, Logger.LogType state)
		{
			BlockingQueue.Add(state.ToString() + ": " + text);
		}

		protected override void Log(string text, object[] data, Logger.LogType state)
		{
			
		}

		protected override void Log(string text, string[] data, Logger.LogType state)
		{
			
		}

		protected override void Log(object obj, Logger.LogType state)
		{
			
		}
	}
}
