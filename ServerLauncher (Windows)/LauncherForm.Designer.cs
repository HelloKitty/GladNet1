namespace Application__GUI_
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ConfigFileOpenDialog = new System.Windows.Forms.OpenFileDialog();
			this.configList = new System.Windows.Forms.ListView();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.startAllButton = new System.Windows.Forms.Button();
			this.stopAllButton = new System.Windows.Forms.Button();
			this.stopSelectedButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.runningServerList = new System.Windows.Forms.ListView();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(502, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
			this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.newToolStripMenuItem.Text = "&New Config";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
			this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.openToolStripMenuItem.Text = "&Open Config";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem.Text = "&About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// ConfigFileOpenDialog
			// 
			this.ConfigFileOpenDialog.DefaultExt = "config";
			this.ConfigFileOpenDialog.FileName = ".config";
			this.ConfigFileOpenDialog.Filter = "Config Files|*.config";
			// 
			// configList
			// 
			this.configList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.configList.LabelWrap = false;
			this.configList.Location = new System.Drawing.Point(292, 89);
			this.configList.MultiSelect = false;
			this.configList.Name = "configList";
			this.configList.Size = new System.Drawing.Size(198, 204);
			this.configList.TabIndex = 1;
			this.configList.UseCompatibleStateImageBehavior = false;
			this.configList.View = System.Windows.Forms.View.List;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
			this.label1.Location = new System.Drawing.Point(286, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(204, 31);
			this.label1.TabIndex = 2;
			this.label1.Text = "Loaded Configs";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(292, 299);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(92, 32);
			this.button1.TabIndex = 3;
			this.button1.Text = "Start Selected";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// startAllButton
			// 
			this.startAllButton.Location = new System.Drawing.Point(398, 299);
			this.startAllButton.Name = "startAllButton";
			this.startAllButton.Size = new System.Drawing.Size(92, 32);
			this.startAllButton.TabIndex = 4;
			this.startAllButton.Text = "Start All";
			this.startAllButton.UseVisualStyleBackColor = true;
			this.startAllButton.Click += new System.EventHandler(this.button2_Click);
			// 
			// stopAllButton
			// 
			this.stopAllButton.Location = new System.Drawing.Point(118, 299);
			this.stopAllButton.Name = "stopAllButton";
			this.stopAllButton.Size = new System.Drawing.Size(92, 32);
			this.stopAllButton.TabIndex = 8;
			this.stopAllButton.Text = "Stop All";
			this.stopAllButton.UseVisualStyleBackColor = true;
			this.stopAllButton.Click += new System.EventHandler(this.stopAllButton_Click);
			// 
			// stopSelectedButton
			// 
			this.stopSelectedButton.Location = new System.Drawing.Point(12, 299);
			this.stopSelectedButton.Name = "stopSelectedButton";
			this.stopSelectedButton.Size = new System.Drawing.Size(92, 32);
			this.stopSelectedButton.TabIndex = 7;
			this.stopSelectedButton.Text = "Stop Selected";
			this.stopSelectedButton.UseVisualStyleBackColor = true;
			this.stopSelectedButton.Click += new System.EventHandler(this.stopSelectedButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
			this.label2.Location = new System.Drawing.Point(6, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(216, 31);
			this.label2.TabIndex = 6;
			this.label2.Text = "Running Servers";
			// 
			// runningServerList
			// 
			this.runningServerList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.runningServerList.LabelWrap = false;
			this.runningServerList.Location = new System.Drawing.Point(12, 89);
			this.runningServerList.MultiSelect = false;
			this.runningServerList.Name = "runningServerList";
			this.runningServerList.Size = new System.Drawing.Size(198, 204);
			this.runningServerList.TabIndex = 5;
			this.runningServerList.UseCompatibleStateImageBehavior = false;
			this.runningServerList.View = System.Windows.Forms.View.List;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 386);
			this.Controls.Add(this.stopAllButton);
			this.Controls.Add(this.stopSelectedButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.runningServerList);
			this.Controls.Add(this.startAllButton);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.configList);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.Text = "Server Launcher";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog ConfigFileOpenDialog;
		private System.Windows.Forms.ListView configList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button startAllButton;
		private System.Windows.Forms.Button stopAllButton;
		private System.Windows.Forms.Button stopSelectedButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView runningServerList;



	}
}

