namespace Application__GUI_
{
	partial class AboutForm
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.GithubLink = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.ForumLink = new System.Windows.Forms.LinkLabel();
			this.YoutubeLink = new System.Windows.Forms.LinkLabel();
			this.InfoTextbox = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.pictureBox1.Image = global::ServerLauncher.Properties.Resources.GithubJetpack;
			this.pictureBox1.Location = new System.Drawing.Point(12, 48);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(112, 124);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// GithubLink
			// 
			this.GithubLink.AutoSize = true;
			this.GithubLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GithubLink.Location = new System.Drawing.Point(130, 48);
			this.GithubLink.Name = "GithubLink";
			this.GithubLink.Size = new System.Drawing.Size(119, 24);
			this.GithubLink.TabIndex = 1;
			this.GithubLink.TabStop = true;
			this.GithubLink.Text = "GitHub Repo";
			this.GithubLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GithubLink_LinkClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(69, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 31);
			this.label1.TabIndex = 2;
			this.label1.Text = "GladNet";
			// 
			// ForumLink
			// 
			this.ForumLink.AutoSize = true;
			this.ForumLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForumLink.Location = new System.Drawing.Point(130, 95);
			this.ForumLink.Name = "ForumLink";
			this.ForumLink.Size = new System.Drawing.Size(66, 24);
			this.ForumLink.TabIndex = 3;
			this.ForumLink.TabStop = true;
			this.ForumLink.Text = "Forum";
			this.ForumLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ForumLink_LinkClicked);
			// 
			// YoutubeLink
			// 
			this.YoutubeLink.AutoSize = true;
			this.YoutubeLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.YoutubeLink.Location = new System.Drawing.Point(130, 148);
			this.YoutubeLink.Name = "YoutubeLink";
			this.YoutubeLink.Size = new System.Drawing.Size(69, 24);
			this.YoutubeLink.TabIndex = 4;
			this.YoutubeLink.TabStop = true;
			this.YoutubeLink.Text = "Videos";
			this.YoutubeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.YoutubeLink_LinkClicked);
			// 
			// InfoTextbox
			// 
			this.InfoTextbox.DetectUrls = false;
			this.InfoTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.InfoTextbox.Location = new System.Drawing.Point(12, 175);
			this.InfoTextbox.Name = "InfoTextbox";
			this.InfoTextbox.ReadOnly = true;
			this.InfoTextbox.Size = new System.Drawing.Size(237, 96);
			this.InfoTextbox.TabIndex = 5;
			this.InfoTextbox.Text = "GladNet is a UDP based networking library based on Lidgren gen3 Free and open sou" +
    "rce, requring only attribution, and targeted at Unity3D game developers.\n\nGladNe" +
    "t (c) 2014 - X/Glader";
			// 
			// AboutForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(258, 273);
			this.Controls.Add(this.InfoTextbox);
			this.Controls.Add(this.YoutubeLink);
			this.Controls.Add(this.ForumLink);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.GithubLink);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "AboutForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "About";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.LinkLabel GithubLink;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel ForumLink;
		private System.Windows.Forms.LinkLabel YoutubeLink;
		private System.Windows.Forms.RichTextBox InfoTextbox;
	}
}