using GladNet.Common;
using GladNet.Server.Common;
using ServerLauncher.Pure_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application__GUI_
{
	public partial class MainForm : Form
	{
		private AboutForm aboutForm = null;
		private ConfigGenerator configGenerator = null;

		Dictionary<string, ConfigInformation> ConfigDictionary = new Dictionary<string, ConfigInformation>();
		Dictionary<int, ServerProcess> ServerProcesses = new Dictionary<int, ServerProcess>();

		public MainForm()
		{
			InitializeComponent();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GenerateFullFocusedForm<AboutForm>(ref aboutForm);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult r = ConfigFileOpenDialog.ShowDialog(this);

			if (r == DialogResult.OK)
				using (Stream stream = ConfigFileOpenDialog.OpenFile())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string xmlString = reader.ReadToEnd();

						try
						{
							var config = Serializer<GladNetXmlSerializer>.Instance.
								DeserializeFromString<ConfigInformation>(xmlString);

							AddNewConfigToList(config);
						}
						catch (LoggableException ee)
						{
							MessageBox.Show(this, "Failed to read the config file. Error: " + ee.Message, "Open Result");
							return;
						}
					}
				}
			else
				MessageBox.Show(this, "Failed to open a .config file for a server.", "Open Result");
		}

		private void AddNewConfigToList(ConfigInformation config)
		{
			if (ConfigDictionary.ContainsKey(config.DLLName))
				MessageBox.Show(this, "You've already loaded this config file.", "Open Result");
			else
			{
				this.ConfigDictionary.Add(config.DLLName, config);
				this.configList.Clear();

				foreach(var kp in ConfigDictionary)
				{
					configList.Items.Add(kp.Key);
				}
			}
		}

		private FormClosingEventHandler DisableUntilOtherDialogCloses()
		{
			this.Enabled = false;
			return new FormClosingEventHandler((object o, FormClosingEventArgs fcea) => this.Enabled = true);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GenerateFullFocusedForm<ConfigGenerator>(ref this.configGenerator);
		}

		private FormType GenerateFullFocusedForm<FormType>(ref FormType f)
			where FormType : Form, new()
		{
			if (f == null)
				f = new FormType();

			if (f.IsDisposed)
			{
				f = null;
				return GenerateFullFocusedForm<FormType>(ref f);
			}

			Point p = this.PointToScreen(Point.Empty);
			p = new Point(p.X + (this.Width / 2) - (f.Width / 2), p.Y);

			f.Location = p;
			f.FormClosing += DisableUntilOtherDialogCloses();
			f.Show(this);

			return f;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(this.configList.SelectedItems.Count == 0)
			{
				MessageBox.Show(this, "You must select a config to start a server.", "Server start result");
				return;
			}

			var listItem = this.configList.SelectedItems[0];
			string configKey = listItem.Text;

			//Should exist
			ConfigInformation info = this.ConfigDictionary[configKey];

			ServerProcess sProcess = new ServerProcess("ServerLoader.exe", info);

			this.ServerProcesses.Add(sProcess.UniqueProcessID, sProcess);
		}

		private void RegisterServerProcess(Process process, string configKey)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			foreach(var kp in ServerProcesses)
			{
				kp.Value.ShutdownServer();
			}
		}
	}
}
