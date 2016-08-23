namespace UCRepoClientApp
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.ImportWordDoc = new System.Windows.Forms.Button();
            this.richTextBoxErrors = new System.Windows.Forms.RichTextBox();
            this.Export = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.ImportXML = new System.Windows.Forms.Button();
            this.textBoxEAFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxExportToFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchEAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewUserDataFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableEAImagesShareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRefreshPackages = new System.Windows.Forms.Button();
            this.comboBoxExpPackage = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(4, 45);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(752, 127);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(4, 374);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(252, 82);
            this.listBox1.TabIndex = 1;
            // 
            // ImportWordDoc
            // 
            this.ImportWordDoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportWordDoc.Location = new System.Drawing.Point(4, 462);
            this.ImportWordDoc.Name = "ImportWordDoc";
            this.ImportWordDoc.Size = new System.Drawing.Size(95, 32);
            this.ImportWordDoc.TabIndex = 2;
            this.ImportWordDoc.Text = "Import Word";
            this.ImportWordDoc.UseVisualStyleBackColor = true;
            this.ImportWordDoc.Click += new System.EventHandler(this.ImportWordDoc_Click);
            // 
            // richTextBoxErrors
            // 
            this.richTextBoxErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxErrors.Location = new System.Drawing.Point(4, 194);
            this.richTextBoxErrors.Name = "richTextBoxErrors";
            this.richTextBoxErrors.Size = new System.Drawing.Size(752, 101);
            this.richTextBoxErrors.TabIndex = 4;
            this.richTextBoxErrors.Text = "";
            // 
            // Export
            // 
            this.Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Export.Location = new System.Drawing.Point(696, 323);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(60, 20);
            this.Export.TabIndex = 5;
            this.Export.Text = "Export";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Errors:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Information:";
            // 
            // listBox2
            // 
            this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(262, 374);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(252, 82);
            this.listBox2.TabIndex = 1;
            // 
            // ImportXML
            // 
            this.ImportXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportXML.Location = new System.Drawing.Point(262, 462);
            this.ImportXML.Name = "ImportXML";
            this.ImportXML.Size = new System.Drawing.Size(89, 32);
            this.ImportXML.TabIndex = 7;
            this.ImportXML.Text = "Import XML";
            this.ImportXML.UseVisualStyleBackColor = true;
            this.ImportXML.Click += new System.EventHandler(this.ImportXML_Click);
            // 
            // textBoxEAFile
            // 
            this.textBoxEAFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxEAFile.Location = new System.Drawing.Point(101, 297);
            this.textBoxEAFile.Name = "textBoxEAFile";
            this.textBoxEAFile.Size = new System.Drawing.Size(528, 20);
            this.textBoxEAFile.TabIndex = 9;
            this.textBoxEAFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxEAFile.TextChanged += new System.EventHandler(this.textBoxEAFile_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 304);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "EA Repo File:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 330);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Package for export:";
            // 
            // textBoxExportToFile
            // 
            this.textBoxExportToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxExportToFile.Location = new System.Drawing.Point(101, 349);
            this.textBoxExportToFile.Name = "textBoxExportToFile";
            this.textBoxExportToFile.Size = new System.Drawing.Size(528, 20);
            this.textBoxExportToFile.TabIndex = 12;
            this.textBoxExportToFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 356);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Export to file:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(761, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.launchEAToolStripMenuItem,
            this.openDataFolderToolStripMenuItem,
            this.createNewUserDataFolderToolStripMenuItem,
            this.disableEAImagesShareToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // launchEAToolStripMenuItem
            // 
            this.launchEAToolStripMenuItem.Name = "launchEAToolStripMenuItem";
            this.launchEAToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.launchEAToolStripMenuItem.Text = "Launch EA";
            this.launchEAToolStripMenuItem.Click += new System.EventHandler(this.launchEAToolStripMenuItem_Click);
            // 
            // openDataFolderToolStripMenuItem
            // 
            this.openDataFolderToolStripMenuItem.Name = "openDataFolderToolStripMenuItem";
            this.openDataFolderToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.openDataFolderToolStripMenuItem.Text = "Open Data Folder";
            this.openDataFolderToolStripMenuItem.Click += new System.EventHandler(this.openDataFolderToolStripMenuItem_Click);
            // 
            // createNewUserDataFolderToolStripMenuItem
            // 
            this.createNewUserDataFolderToolStripMenuItem.Name = "createNewUserDataFolderToolStripMenuItem";
            this.createNewUserDataFolderToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.createNewUserDataFolderToolStripMenuItem.Text = "Create New User Data Folder";
            this.createNewUserDataFolderToolStripMenuItem.Click += new System.EventHandler(this.createNewUserDataFolderToolStripMenuItem_Click);
            // 
            // disableEAImagesShareToolStripMenuItem
            // 
            this.disableEAImagesShareToolStripMenuItem.Name = "disableEAImagesShareToolStripMenuItem";
            this.disableEAImagesShareToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.disableEAImagesShareToolStripMenuItem.Text = "Disable EAImages Share";
            this.disableEAImagesShareToolStripMenuItem.Click += new System.EventHandler(this.disableEAImagesShareToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnRefreshPackages
            // 
            this.btnRefreshPackages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefreshPackages.Location = new System.Drawing.Point(635, 323);
            this.btnRefreshPackages.Name = "btnRefreshPackages";
            this.btnRefreshPackages.Size = new System.Drawing.Size(55, 20);
            this.btnRefreshPackages.TabIndex = 17;
            this.btnRefreshPackages.Text = "Refresh";
            this.btnRefreshPackages.UseVisualStyleBackColor = true;
            this.btnRefreshPackages.Click += new System.EventHandler(this.btnRefreshPackages_Click);
            // 
            // comboBoxExpPackage
            // 
            this.comboBoxExpPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxExpPackage.FormattingEnabled = true;
            this.comboBoxExpPackage.Location = new System.Drawing.Point(101, 323);
            this.comboBoxExpPackage.Name = "comboBoxExpPackage";
            this.comboBoxExpPackage.Size = new System.Drawing.Size(528, 21);
            this.comboBoxExpPackage.TabIndex = 19;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 506);
            this.Controls.Add(this.comboBoxExpPackage);
            this.Controls.Add(this.btnRefreshPackages);
            this.Controls.Add(this.textBoxExportToFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxEAFile);
            this.Controls.Add(this.ImportXML);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.richTextBoxErrors);
            this.Controls.Add(this.ImportWordDoc);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "UCRepoClientApp";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        public System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button ImportWordDoc;
        public System.Windows.Forms.RichTextBox richTextBoxErrors;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button ImportXML;
        private System.Windows.Forms.TextBox textBoxEAFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxExportToFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDataFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewUserDataFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableEAImagesShareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem launchEAToolStripMenuItem;
        private System.Windows.Forms.Button btnRefreshPackages;
        private System.Windows.Forms.ComboBox comboBoxExpPackage;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
    }
}

