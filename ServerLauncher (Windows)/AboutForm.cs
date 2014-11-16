using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application__GUI_
{
	public partial class AboutForm : Form
	{
		private string GithubRepoURL = @"https://github.com/HeIIoKitty/GladNet";
		private string ForumURL = @"http://forum.unity3d.com/threads/gladnet-free-opensource-networking-library.280016/";
		private string YoutubeURL = @"https://www.youtube.com/user/hcflowen/videos";

		public AboutForm()
		{
			InitializeComponent();
		}

		private void GithubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenWebpage(GithubRepoURL);
		}

		private void OpenWebpage(string url)
		{
			Process.Start(url);
		}

		private void ForumLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenWebpage(ForumURL);
		}

		private void YoutubeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenWebpage(YoutubeURL);
		}
	}
}
