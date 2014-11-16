using GladNet.Common;
using GladNet.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerLauncher.Pure_Code
{
	public class ServerProcess : IDisposable
	{

		private static Lazy<AnonymousPipeServerStream> _serverBroadcastingPipe =
			new Lazy<AnonymousPipeServerStream>(ServerProcess.ConstructServerStream, true);

		private static AnonymousPipeServerStream serverBroadcastingPipe
		{
			get { return _serverBroadcastingPipe.Value; }
		}

		private static AnonymousPipeServerStream ConstructServerStream()
		{
			return new AnonymousPipeServerStream(PipeDirection.Out, System.IO.HandleInheritability.Inheritable);
		}

		private static StreamWriter serverStreamWriter = new StreamWriter(serverBroadcastingPipe);

		public ConfigInformation Config { get; private set; }

		private readonly Process ApplicationProcess;

		public long MemoryUsage
		{
			get { return ApplicationProcess.WorkingSet64; }
		}

		public int UniqueProcessID
		{
			get { return ApplicationProcess.Id; }
		}

		//DLLNAME APPNAME HAILMESSAGE PORT PIPEHANDLE
		public ServerProcess(string path, ConfigInformation info)
		{
			try
			{
				Config = info;
				string clientString = ServerProcess.serverBroadcastingPipe.GetClientHandleAsString();

				ApplicationProcess = Process.Start(new ProcessStartInfo()
				{
					Arguments = Config.DLLName + " " + Config.ApplicationName + " " + Config.HailMessage + " " +
					Config.Port.ToString() + " " + clientString,
					UseShellExecute = true,
					FileName = path
				});
			}
			catch(NullReferenceException e)
			{
				MessageBox.Show(e.Message + " " + e.Data + e.StackTrace);
			}
		}

		public void ShutdownServer()
		{
			serverStreamWriter.AutoFlush = true;
			serverStreamWriter.WriteLine("[SHUTDOWN] " + ApplicationProcess.Id.ToString());
		}

		public void Dispose()
		{
			this.ApplicationProcess.Dispose();
		}
	}
}
