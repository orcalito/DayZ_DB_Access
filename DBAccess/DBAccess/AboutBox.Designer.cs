namespace DBAccess
{
	partial class AboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			this.logoPictureBox = new System.Windows.Forms.PictureBox();
			this.okButton = new System.Windows.Forms.Button();
			this.labelProductName = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textboxDesc = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
			this.SuspendLayout();
			//
			// logoPictureBox
			//
			this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
			this.logoPictureBox.Location = new System.Drawing.Point(12, 12);
			this.logoPictureBox.Name = "logoPictureBox";
			this.logoPictureBox.Size = new System.Drawing.Size(131, 66);
			this.logoPictureBox.TabIndex = 12;
			this.logoPictureBox.TabStop = false;
			//
			// okButton
			//
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(509, 317);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 24;
			this.okButton.Text = "Ok";
			//
			// labelProductName
			//
			this.labelProductName.AutoSize = true;
			this.labelProductName.Location = new System.Drawing.Point(150, 12);
			this.labelProductName.Name = "labelProductName";
			this.labelProductName.Size = new System.Drawing.Size(121, 13);
			this.labelProductName.TabIndex = 26;
			this.labelProductName.Text = "DayZ DataBase Access";
			//
			// linkLabel1
			//
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(149, 56);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(227, 13);
			this.linkLabel1.TabIndex = 27;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "https://github.com/orcalito/DayZ_DB_Access";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			//
			// labelCopyright
			//
			this.labelCopyright.AutoSize = true;
			this.labelCopyright.Location = new System.Drawing.Point(150, 34);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(43, 13);
			this.labelCopyright.TabIndex = 28;
			this.labelCopyright.Text = "Orcalito";
			//
			// labelVersion
			//
			this.labelVersion.Location = new System.Drawing.Point(311, 12);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(64, 13);
			this.labelVersion.TabIndex = 29;
			this.labelVersion.Text = "...";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
			//
			// label4
			//
			this.label4.Location = new System.Drawing.Point(310, 34);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label4.Size = new System.Drawing.Size(64, 13);
			this.label4.TabIndex = 28;
			this.label4.Text = "2013 - 2014";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			//
			// textboxDesc
			//
			this.textboxDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textboxDesc.Location = new System.Drawing.Point(12, 84);
			this.textboxDesc.Name = "textboxDesc";
			this.textboxDesc.ReadOnly = true;
			this.textboxDesc.Size = new System.Drawing.Size(571, 227);
			this.textboxDesc.TabIndex = 30;
			this.textboxDesc.Text = "";
			//
			// AboutBox
			//
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(596, 352);
			this.Controls.Add(this.textboxDesc);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelCopyright);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.labelProductName);
			this.Controls.Add(this.logoPictureBox);
			this.Controls.Add(this.okButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(403, 390);
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "DDBA";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox logoPictureBox;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label labelProductName;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RichTextBox textboxDesc;
	}
}
