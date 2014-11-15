using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application__GUI_
{
	public partial class MainForm : Form
	{
		private AboutForm aboutForm = null;

		public MainForm()
		{
			InitializeComponent();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (aboutForm == null)
				aboutForm = new AboutForm();

			if(aboutForm.IsDisposed)
			{
				aboutForm = null;
				this.aboutToolStripMenuItem_Click(sender, e);
			}
			else
				aboutForm.Show();
		}
	}
}
