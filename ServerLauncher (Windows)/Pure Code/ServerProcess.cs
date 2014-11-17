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
		//These all need to be non static as it doesn't broadcast.
		//Data it sends is first come first serve off the handle
		private readonly AnonymousPipeServerStream serverBroadcastingPipe;

		private readonly StreamWriter serverStreamWriter;

		public ConfigInformation Config { get; private set; }

		public Action OnExited;

		private readonly Process ApplicationProcess;

		public long MemoryUsage
		{
			get { return ApplicationProcess.WorkingSet64; }
		}

		public int UniqueProcessID { get; private set; }

		//DLLNAME APPNAME HAILMESSAGE PORT PIPEHANDLE
		public ServerProcess(string path, ConfigInformation info)
		{
			serverBroadcastingPipe = 
			new AnonymousPipeServerStream(PipeDirection.Out, System.IO.HandleInheritability.Inheritable);

			serverStreamWriter = new StreamWriter(serverBroadcastingPipe);

			try
			{
				Config = info;
				string clientString = serverBroadcastingPipe.GetClientHandleAsString();

				ApplicationProcess = Process.Start(new ProcessStartInfo()
				{
					Arguments = Config.DLLName + " " + Config.ApplicationName + " " + Config.HailMessage + " " +
					Config.Port.ToString() + " " + clientString,
					UseShellExecute = false,
					FileName = path
				});

				UniqueProcessID = ApplicationProcess.Id;

				ApplicationProcess.EnableRaisingEvents = true;

				ApplicationProcess.Exited += new EventHandler(OnExit);
			}
			catch(NullReferenceException e)
			{
				MessageBox.Show(e.Message + " " + e.Data + e.StackTrace);
			}
		}

		private void OnExit(object o, EventArgs e)
		{
			if (OnExited != null)
				OnExited();
		}

		public void ShutdownServer()
		{
			serverStreamWriter.AutoFlush = true;
			serverStreamWriter.WriteLine("[SHUTDOWN] " + ApplicationProcess.Id.ToString());
		}

		public void Dispose()
		{
			this.ApplicationProcess.Dispose();
			this.serverStreamWriter.Dispose();
			this.serverBroadcastingPipe.Dispose();
		}
	}
}
