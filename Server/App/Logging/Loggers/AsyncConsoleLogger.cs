using GladNet.Server.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.App.Logging.Loggers
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
			: base(State.Error)
		{
			BlockingQueue = new BlockingCollection<string>();
		}

		public void SetState(State newState)
		{
			this.LoggerState = newState;
			Task.Factory.StartNew(() =>
				{
					while(true) Console.WriteLine(BlockingQueue.Take());
				}, TaskCreationOptions.LongRunning);
		}

		protected override void Log(string text, Logger.State state)
		{
			BlockingQueue.Add(state.ToString() + ": " + text);
		}

		protected override void Log(string text, object[] data, Logger.State state)
		{
			
		}

		protected override void Log(string text, string[] data, Logger.State state)
		{
			
		}

		protected override void Log(object obj, Logger.State state)
		{
			
		}
	}
}
