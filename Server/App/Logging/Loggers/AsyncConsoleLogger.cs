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
using System.Collections.Concurrent;
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
			: base(Logger.LogType.Debug)
		{
			BlockingQueue = new BlockingCollection<string>(); 
			
			Task.Factory.StartNew(() =>
			{
				while (true) Console.WriteLine(BlockingQueue.Take());
			}, TaskCreationOptions.LongRunning);
		}

		public void SetState(Logger.LogType newState)
		{
			lock (_Instance)
			{
				this.LoggerState = newState;
			}
		}

		protected override void Log(string text, Logger.LogType state)
		{
			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ");
			builder.Append(text);

			BlockingQueue.Add(builder.ToString());
		}

		protected override void Log(string text, Logger.LogType state, params object[] data)
		{
			StringBuilder builder = new StringBuilder();

			try
			{
				builder.AppendFormat(state.ToString() + ": " + text, data);
			}
			catch(Exception e)
			{
				this.LogError("Failed to log; check parameter list for logging. See exception for stack trace.");
				throw;
			}

			BlockingQueue.Add(builder.ToString());
		}

		protected override void Log(string text, Logger.LogType state, params string[] data)
		{
			StringBuilder builder = new StringBuilder();
			StringBuilder subBuilder = new StringBuilder(state.ToString());
			subBuilder.Append(": ");
			subBuilder.Append(text);

			try
			{
				builder.AppendFormat(subBuilder.ToString(), data);
			}
			catch(Exception e)
			{
				this.LogError("Failed to log; check parameter list for logging. See exception for stack trace.");
				throw;
			}
		}

		protected override void Log(object obj, Logger.LogType state)
		{
			if (obj == null)
				return;

			StringBuilder builder = new StringBuilder(state.ToString());
			builder.Append(": ");
			builder.Append(obj != null ? obj.ToString() : "[NULL]");

			this.BlockingQueue.Add(builder.ToString());
		}
	}
}
