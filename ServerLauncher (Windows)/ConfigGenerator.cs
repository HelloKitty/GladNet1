using GladNet.Common;
using GladNet.Server.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application__GUI_
{
	public partial class ConfigGenerator : Form
	{
		public ConfigGenerator()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			int portInt;

			if(!int.TryParse(this.listenPortText.Text, out portInt) || portInt < 1 || portInt > 65535)
			{
				MessageBox.Show(this,"The port must be a valid integer between 1 and 65535.", "Invalid Port");
				return;
			}


			if (CreateConfigFile(dllNameText.Text, hailMessageText.Text,
				applicationNameText.Text, portInt, fileNameText.Text))
			{
				this.Close();
			}
		}

		private bool CreateConfigFile(string dllName, string hailMessage, string appName, int port, string fileName)
		{
			if (!dllName.Contains(".dll"))
				dllName += ".dll";

			ConfigInformation info = new ConfigInformation(dllName, hailMessage, appName, port);

			if (fileName.Length == 0)
				return false;

			using(StreamWriter sw = new StreamWriter(fileName + ".config", false))
			{
				try
				{
					string xmlString = Serializer<GladNetXmlSerializer>.Instance.SerializeToString(info);

					sw.Write(xmlString);
				}
				catch(LoggableException e)
				{
					MessageBox.Show(this, "Encountered Error: " + e.Message + 
						e.InnerException != null ? e.InnerException.Message : "");
					return false;
				}
			}

			MessageBox.Show(Owner, "Config file was successfully created in the root director named: " + fileNameText.Text + ".config",
					"Creation Result");
			return true;
		}
	}
}
