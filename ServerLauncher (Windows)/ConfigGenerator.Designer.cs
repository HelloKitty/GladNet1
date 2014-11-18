namespace Application__GUI_
{
	partial class ConfigGenerator
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.dllNameText = new System.Windows.Forms.TextBox();
			this.hailMessageText = new System.Windows.Forms.TextBox();
			this.applicationNameText = new System.Windows.Forms.TextBox();
			this.listenPortText = new System.Windows.Forms.TextBox();
			this.passwordText = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.fileNameText = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.label1.Location = new System.Drawing.Point(12, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "DLL Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.label2.Location = new System.Drawing.Point(12, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Hail Message";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.label3.Location = new System.Drawing.Point(12, 93);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(131, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Application name";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.label4.Location = new System.Drawing.Point(12, 124);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(85, 20);
			this.label4.TabIndex = 3;
			this.label4.Text = "Listen Port";
			// 
			// dllNameText
			// 
			this.dllNameText.Location = new System.Drawing.Point(214, 28);
			this.dllNameText.Name = "dllNameText";
			this.dllNameText.Size = new System.Drawing.Size(100, 20);
			this.dllNameText.TabIndex = 4;
			// 
			// hailMessageText
			// 
			this.hailMessageText.Location = new System.Drawing.Point(214, 60);
			this.hailMessageText.Name = "hailMessageText";
			this.hailMessageText.Size = new System.Drawing.Size(100, 20);
			this.hailMessageText.TabIndex = 5;
			// 
			// applicationNameText
			// 
			this.applicationNameText.Location = new System.Drawing.Point(214, 93);
			this.applicationNameText.Name = "applicationNameText";
			this.applicationNameText.Size = new System.Drawing.Size(100, 20);
			this.applicationNameText.TabIndex = 6;
			// 
			// listenPortText
			// 
			this.listenPortText.Location = new System.Drawing.Point(214, 124);
			this.listenPortText.Name = "listenPortText";
			this.listenPortText.Size = new System.Drawing.Size(100, 20);
			this.listenPortText.TabIndex = 7;
			// 
			// passwordText
			// 
			this.passwordText.Location = new System.Drawing.Point(214, 155);
			this.passwordText.Name = "passwordText";
			this.passwordText.Size = new System.Drawing.Size(100, 20);
			this.passwordText.TabIndex = 9;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.label5.Location = new System.Drawing.Point(12, 155);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(158, 20);
			this.label5.TabIndex = 8;
			this.label5.Text = "Password (Will Hash)";
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
			this.button1.Location = new System.Drawing.Point(12, 228);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(92, 33);
			this.button1.TabIndex = 10;
			this.button1.Text = "Create";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// fileNameText
			// 
			this.fileNameText.Location = new System.Drawing.Point(214, 233);
			this.fileNameText.Name = "fileNameText";
			this.fileNameText.Size = new System.Drawing.Size(100, 20);
			this.fileNameText.TabIndex = 11;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.label6.Location = new System.Drawing.Point(128, 233);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(80, 20);
			this.label6.TabIndex = 12;
			this.label6.Text = "File Name";
			// 
			// ConfigGenerator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(326, 273);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.fileNameText);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.passwordText);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.listenPortText);
			this.Controls.Add(this.applicationNameText);
			this.Controls.Add(this.hailMessageText);
			this.Controls.Add(this.dllNameText);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ConfigGenerator";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Config Creator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox dllNameText;
		private System.Windows.Forms.TextBox hailMessageText;
		private System.Windows.Forms.TextBox applicationNameText;
		private System.Windows.Forms.TextBox listenPortText;
		private System.Windows.Forms.TextBox passwordText;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox fileNameText;
		private System.Windows.Forms.Label label6;
	}
}