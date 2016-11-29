/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: MainForm.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 30 $

	Date last modified: $Modtime: 11/28/12 10:20a $

	Project	: 
	Target	: 
	Tabs	: 

	Abstract:

Copyright (c) 2012 EPRI

Licensed by EPRI under one or more contributor license agreements.

See the NOTICE file distributed with this work for additional 
information regarding copyright ownership. EPRI licenses this 
file to you under the Apache License, Version 2.0 (the "; License; "); 
you may not use this file except in compliance with the License.
You may obtain a copy of the License at:
http://www.apache.org/licenses/LICENSE-2.0
  
Unless required by applicable law or agreed to in writing, software 
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
implied.  See the License for the specific language governing 
permissions and limitations under the License.
--------------------------------------------------------------------------
**********************	 Revision History.   *****************************
--------------------------------------------------------------------------
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp/MainForm.cs 30    12/10/12 3:00p Ronp $

$History: MainForm.cs $
 * 
 * *****************  Version 30  *****************
 * User: Ronp         Date: 12/10/12   Time: 3:00p
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * IUCTEMPLATE-10 User will misspell the file name or directory in EA Repo
 * File. An Unhandled Exception will occur. Double-click on attached .txt
 * file to see Exception text.
 * IUCTEMPLATE-11 User will make the EA Repo File path empty then click
 * Refresh. Error will occur. If user quits and restarts the application,
 * error will pop up before GUI is loaded. Please fix. Double-click on
 * attached .txt file to see Exception text.
 * 
 * IUCTEMPLATE-12 User will change "Export to file" directory to a
 * nonexistent one. Select a package for export then click Export.
 * Application will throw an error message. Double-click on attached .txt
 * file to see Exception text.
 * 
 * *****************  Version 29  *****************
 * User: Ronp         Date: 11/05/12   Time: 12:20p
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * Fix minor callback handler link for main menu.
 * 
 * *****************  Version 28  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * Update keyword expansion

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Xsl;
using System.Xml;

namespace UCRepoClientApp
{
    public partial class MainForm : Form
    {
        string _strUserResourcesFolder = "";
        string _strBaseRsourcesFolder = "";
        string _strBaseAppPath = "";

        FileSystemWatcher _watcher1 = null;
        FileSystemWatcher _watcher2 = null;

        public MainForm(int test)
        {
            InitializeComponent();
        }

        public void FormLogMsg(string strType, string strMsg)
        {
            richTextBox1.AppendText(strType + " : " + strMsg + "\r\n");
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.ScrollToCaret();
        }

        public void FormLogError(string strType, string strMsg)
        {
            richTextBoxErrors.AppendText(strType + " : " + strMsg + "\r\n");
            richTextBoxErrors.SelectionStart = richTextBox1.TextLength;
            richTextBoxErrors.SelectionLength = 0;
            richTextBoxErrors.ScrollToCaret();
        }

        public void CreateWatchers()
        {
            if (_watcher1 != null) return;

            //Create a new FileSystemWatcher.
            _watcher1 = new FileSystemWatcher();

            _watcher1.SynchronizingObject = this;

            //Set the filter to only catch docx files.
            _watcher1.Filter = "*.docx";

            //Subscribe to the Created event.
            _watcher1.Created += new FileSystemEventHandler(watcher_DirChanged);
            _watcher1.Changed += new FileSystemEventHandler(watcher_DirChanged);
            _watcher1.Deleted += new FileSystemEventHandler(watcher_DirChanged);
            _watcher1.Renamed += new RenamedEventHandler(watcher_DirChanged);


            //Set the path to C:\Temp\
            _watcher1.Path = _strUserResourcesFolder + @"\FilledInTemplates";

            //Enable the FileSystemWatcher events.
            _watcher1.EnableRaisingEvents = true;

            //Create a new FileSystemWatcher.
            _watcher2 = new FileSystemWatcher();

            _watcher2.SynchronizingObject = this;

            //Set the filter to only catch docx files.
            _watcher2.Filter = "*.xml";

            //Subscribe to the Created event.
            _watcher2.Created += new FileSystemEventHandler(watcher_DirChanged);
            _watcher2.Changed += new FileSystemEventHandler(watcher_DirChanged);
            _watcher2.Deleted += new FileSystemEventHandler(watcher_DirChanged);
            _watcher2.Renamed += new RenamedEventHandler(watcher_DirChanged);

            //Set the path to C:\Temp\
            _watcher2.Path = _strUserResourcesFolder + @"\FilledInTemplates";

            //Enable the FileSystemWatcher events.
            _watcher2.EnableRaisingEvents = true;
        }

        void watcher_DirChanged(object sender, FileSystemEventArgs e)
        {
            Form1_Load(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // string strPath;

            listBox1.Items.Clear();
            listBox2.Items.Clear();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                //In Debugging mode  
                _strBaseAppPath = Path.GetFullPath(Application.StartupPath) + @"\..\..\.."; ;
                _strBaseRsourcesFolder = Path.GetFullPath(Application.StartupPath) + @"\..\..\..";
                _strUserResourcesFolder = _strBaseRsourcesFolder;
            }
            else
            {
                //In Published mode  
                _strBaseAppPath = Path.GetFullPath(Application.StartupPath);
                _strBaseRsourcesFolder = Path.GetFullPath(Application.StartupPath) + @"\Resources";

                // now check to see if resource folder has been assigned by user:

                if (Properties.Settings.Default.ResourceFolder == "INIT")
                {
                    CreateNewUserDataFolder();

                    if (Properties.Settings.Default.ResourceFolder == "INIT")
                    {
                        // folder did not change
                        MessageBox.Show("User supplied folder must be specified before use.");

                        Export.Enabled = false;
                        ImportWordDoc.Enabled = false;
                        ImportXML.Enabled = false;
                        btnRefreshPackages.Enabled = false;
                        openDataFolderToolStripMenuItem.Enabled = false;
                        launchEAToolStripMenuItem.Enabled = false;
                        disableEAImagesShareToolStripMenuItem.Enabled = false;
                        openToolStripMenuItem.Enabled = false;

                        return;
                    }

                    _strUserResourcesFolder = Properties.Settings.Default.ResourceFolder;
                }
                else
                {
                    _strUserResourcesFolder = Properties.Settings.Default.ResourceFolder;

                    Export.Enabled = true;
                    ImportWordDoc.Enabled = true;
                    ImportXML.Enabled = true;
                    btnRefreshPackages.Enabled = true;
                    openDataFolderToolStripMenuItem.Enabled = true;
                    launchEAToolStripMenuItem.Enabled = true;
                    disableEAImagesShareToolStripMenuItem.Enabled = true;
                    openToolStripMenuItem.Enabled = true;
                }

            }

            DirectoryInfo di = new DirectoryInfo(_strUserResourcesFolder + @"\FilledInTemplates");

            FileInfo[] files = di.GetFiles("*.docx", SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                listBox1.Items.Add(file.Name);
            }

            if (files.Count() > 0)
            {
                listBox1.SelectedIndex = 0;
            }

            DirectoryInfo di2 = new DirectoryInfo(_strUserResourcesFolder + @"\FilledInTemplates");

            FileInfo[] files2 = di.GetFiles("*MODEL.xml", SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files2)
            {
                listBox2.Items.Add(file.Name);
            }

            if (files2.Count() > 0)
            {
                listBox2.SelectedIndex = 0;
            }

            if (Properties.Settings.Default.EAPFile == "INIT")
            {
                textBoxEAFile.Text = Path.GetFullPath(_strUserResourcesFolder + @"\EAResources\UCRepo.eap");
            }
            else
            {
                textBoxEAFile.Text = Path.GetFullPath(Properties.Settings.Default.EAPFile);
            }

            textBoxExportToFile.Text = Path.GetFullPath(_strUserResourcesFolder + @"\FilledInTemplates\TestOut.html");

            CreateWatchers();
        }


        private void Export_Click(object sender, EventArgs e)
        {
            if (comboBoxExpPackage.Text == "")
            {
                return;
            }

            UCRepoClassLibrary.EAExporter theEAExporter;

            richTextBox1.Clear();
            richTextBoxErrors.Clear();

            UCRepoClassLibrary.EAExporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAExporter.LogMsgCallbackType(this.FormLogMsg);
            UCRepoClassLibrary.EAExporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAExporter.LogMsgCallbackType(this.FormLogError);

            theEAExporter = new UCRepoClassLibrary.EAExporter(myLogMsgCallback, myErrorMsgCallback);

            string strXSLTFullPath = _strBaseRsourcesFolder + @"\Translators\IntermediateToXHTML.xslt";
            string strRepositoryFullPath = textBoxEAFile.Text;
            string strUseCasePackage = comboBoxExpPackage.Text;
            string strOutputFileName = textBoxExportToFile.Text;

            theEAExporter.Open(strRepositoryFullPath);


            theEAExporter.Export(strUseCasePackage, strXSLTFullPath, strOutputFileName);

            UCRepoClassLibrary.EAExporter.LogMsg(UCRepoClassLibrary.EAExporter.LogMsgType.Info, "Export complete");

            theEAExporter.Close();

            Form1_Load(sender, e); // refresh screen
        }

        private void ImportXML_Click(object sender, EventArgs e)
        {
            UCRepoClassLibrary.EAImporter theEAImporter;
            string strPath;

            richTextBox1.Clear();
            richTextBoxErrors.Clear();

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogMsg);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogError);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);

            string strRepositoryFullPath = textBoxEAFile.Text;

            theEAImporter.Open(strRepositoryFullPath);

            strPath = _strUserResourcesFolder + @"\FilledInTemplates";

            int iErrorCount = theEAImporter.ImportXMLFile(strPath, listBox2.Text);

            if (iErrorCount == 0)
            {
                UCRepoClassLibrary.EAImporter.LogMsg(UCRepoClassLibrary.EAImporter.LogMsgType.Info, "Import completed successfully");
            }
            else
            {
                UCRepoClassLibrary.EAImporter.LogMsg(UCRepoClassLibrary.EAImporter.LogMsgType.Info, "Import ABORTED. Errors found. Error count=" + iErrorCount.ToString());
            }

            theEAImporter.Close();

            Form1_Load(sender, e); // refresh screen
        }

        private void ImportWordDoc_Click(object sender, EventArgs e)
        {
            DebugXML();
            return;

            UCRepoClassLibrary.EAImporter theEAImporter;
            string strPathtoWordDoc;
            string strFileName;

            richTextBox1.Clear();
            richTextBoxErrors.Clear();

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogMsg);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogError);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);

            string strRepositoryFullPath = textBoxEAFile.Text;

            if (theEAImporter.Open(strRepositoryFullPath) != 0)
            {
                // failed
                return;
            }

            strPathtoWordDoc = _strUserResourcesFolder + @"\FilledInTemplates";
            strFileName = strPathtoWordDoc + "\\" + listBox1.Text;

            string strXSLTFullPath = _strBaseRsourcesFolder + @"\Translators\EUToIntermediate.xslt";

            int iErrorCount = theEAImporter.SetupPaths(strPathtoWordDoc, listBox1.Text, strXSLTFullPath);

            if (iErrorCount == 0)
            {
                iErrorCount = theEAImporter.ImportWordDoc(strFileName);
            }

            if (iErrorCount == 0)
            {
                UCRepoClassLibrary.EAImporter.LogMsg(UCRepoClassLibrary.EAImporter.LogMsgType.Info, "Import completed successfully");
            }
            else
            {
                UCRepoClassLibrary.EAImporter.LogMsg(UCRepoClassLibrary.EAImporter.LogMsgType.Info, "Import ABORTED. Errors found. Error count=" + iErrorCount.ToString());
            }

            theEAImporter.Close();

            Form1_Load(sender, e); // refresh screen
        }

        private void CreateNewUserDataFolder()
        {
            string strBaseResourceFolder = "";

            if (System.Diagnostics.Debugger.IsAttached)
            {
                //In Debugging mode  
                strBaseResourceFolder = Path.GetFullPath(Application.StartupPath) + @"\..\..\..\..";
            }
            else
            {
                //In Published mode  
                strBaseResourceFolder = Path.GetFullPath(Application.StartupPath) + @"\Resources";
            }

            FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Personal;

            folderBrowserDialog.Description = "Select a folder for support data files. Creation of a new UCRepository folder is recommended. Sample files will automatically be copied to the specified folder.";
            string strUserResourcesFolder;

            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                strUserResourcesFolder = folderBrowserDialog.SelectedPath;
            }
            else
            {
                return;
            }

            try
            {
                if (!Directory.Exists(strUserResourcesFolder + @"\EAResources"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(strUserResourcesFolder + @"\EAResources");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create directory for EAResources: " + ex.Message);
                return;
            }

            try
            {
                if (!Directory.Exists(strUserResourcesFolder + @"\FilledInTemplates"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(strUserResourcesFolder + @"\FilledInTemplates");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create directory for FilledInTemplates: " + ex.Message);
                return;
            }

            try
            {
                System.IO.File.Copy(strBaseResourceFolder + @"\EAResources\ImportAll.cmd", strUserResourcesFolder + @"\ImportAll.cmd", true);
                System.IO.File.Copy(strBaseResourceFolder + @"\EAResources\ExchangeProfileXMI.xml", strUserResourcesFolder + @"\EAResources\ExchangeProfileXMI.xml", true);
                System.IO.File.Copy(strBaseResourceFolder + @"\EAResources\UCRepo.eap", strUserResourcesFolder + @"\EAResources\UCRepo.eap", true);
                System.IO.File.Copy(strBaseResourceFolder + @"\EAResources\EmptyUCRepo.eap", strUserResourcesFolder + @"\EAResources\EmptyUCRepo.eap", true);
                System.IO.File.Copy(strBaseResourceFolder + @"\EAResources\Help.rtf", strUserResourcesFolder + @"\EAResources\Help.rtf", true);
                System.IO.File.Copy(strBaseResourceFolder + @"\FilledInTemplates\IEC_UseCaseTemplateGOOD.docx", strUserResourcesFolder + @"\FilledInTemplates\IEC_UseCaseTemplateGOOD.docx", true);
                System.IO.File.Copy(strBaseResourceFolder + @"\TC8Template\IEC_UseCaseTemplate.docx", strUserResourcesFolder + @"\FilledInTemplates\IEC_UseCaseTemplate.docx", true);

                string strCommand = "call \"" + _strBaseAppPath + "\\UCRepoClientApp.exe\" %1 %2 %3 %4 %5 %6";

                StreamWriter outfile = new StreamWriter(strUserResourcesFolder + @"\ucrepoclientapp.cmd", true);

                if (outfile != null)
                {
                    outfile.WriteLine(strCommand);
                    outfile.Close();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying file(s) to destination folder: " + ex.Message);
                return;
            }

            UCRepoClassLibrary.EAImporter theEAImporter;

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogMsg);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogError);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);

            string strRepositoryFullPath = textBoxEAFile.Text;

            int iResult = theEAImporter.CreateFolderForShare(strUserResourcesFolder + @"\EAResources\EAImages");

            // 0 = success other is failure

            if (iResult != 0)
            {
                MessageBox.Show("Error creating EAImages share folder.");
                return;
            }

            Properties.Settings.Default.ResourceFolder = strUserResourcesFolder;
            Properties.Settings.Default.Save();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 box = new AboutBox1();
            box.ShowDialog();
        }

        private void openDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", _strUserResourcesFolder);
        }

        private void createNewUserDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewUserDataFolder();
            Form1_Load(sender, e);
        }

        private void disableEAImagesShareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult oResult = MessageBox.Show("Warning: please be certain you wish to disable the share. Import/Export functionality will not function until a new share is created.", "UCRepoClientApp", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (oResult == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            UCRepoClassLibrary.EAImporter theEAImporter;

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogMsg);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.FormLogError);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);

            int iResult = theEAImporter.DisableCurrentShare();
        }

        private void launchEAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(textBoxEAFile.Text))
            {
                MessageBox.Show("EA file does not exist: " + textBoxEAFile.Text, "UCRepoClientApp", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                return;
            }

            Process.Start(textBoxEAFile.Text);
        }

        private void textBoxEAFile_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EAPFile = textBoxEAFile.Text;
            Properties.Settings.Default.Save();
        }

        private void btnRefreshPackages_Click(object sender, EventArgs e)
        {
            comboBoxExpPackage.Items.Clear();

            UCRepoClassLibrary.EAExporter theEAExporter;

            UCRepoClassLibrary.EAExporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAExporter.LogMsgCallbackType(this.FormLogMsg);
            UCRepoClassLibrary.EAExporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAExporter.LogMsgCallbackType(this.FormLogError);

            theEAExporter = new UCRepoClassLibrary.EAExporter(myLogMsgCallback, myErrorMsgCallback);

            string strRepositoryFullPath = textBoxEAFile.Text;

            if (theEAExporter.Open(strRepositoryFullPath) != 0)
            {
                // failed
                return;
            }

            List<string> strListOfPackages = theEAExporter.GetListOfUseCases();

            if (strListOfPackages != null)
            {
                foreach (string strPackage in strListOfPackages)
                {
                    comboBoxExpPackage.Items.Add(strPackage);
                }
            }

            theEAExporter.Close();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HelpForm box = new HelpForm(_strUserResourcesFolder + @"\EAResources\Help.rtf");

            box.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string strfolderName = "";

            try
            {
                strfolderName = new FileInfo(textBoxEAFile.Text).DirectoryName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid folder specified: " + ex.Message, "UCRepoClientApp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            openFileDialog1.InitialDirectory = strfolderName;
            openFileDialog1.Filter = "Enterprise Architect Repository|*.eap";
            openFileDialog1.Title = "Select an Enterprise Architect Repository file";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxEAFile.Text = openFileDialog1.FileName;
                Properties.Settings.Default.EAPFile = textBoxEAFile.Text;
                Properties.Settings.Default.Save();

                launchEAToolStripMenuItem_Click(sender, e);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void DebugXML()
        {
            FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            //folderBrowserDialog.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            folderBrowserDialog.Description = "Select a folder for support data files.";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                FormLogMsg("INFO", folderBrowserDialog.SelectedPath);
            }

            //XslCompiledTransform xslt = new XslCompiledTransform();

            //xslt.Load(@"C:\Users\P-Dev\Desktop\TuskSoft\EPRi\UCRepo\EUToIntermediate.xslt");

            //// Create the writer.
            //XmlWriterSettings XWriterSettings = new XmlWriterSettings();
            //XWriterSettings.Indent = true;
            //XWriterSettings.IndentChars = "\t";
            //XmlWriter writer = XmlWriter.Create(@"C:\Users\P-Dev\Desktop\TuskSoft\EPRi\UCRepo\FilledInTemplates\IEC_UseCaseTemplate_Model.xml", XWriterSettings);

            //StringReader strXMLSource = new StringReader(File.ReadAllText(@"C:\Users\P-Dev\Desktop\TuskSoft\EPRi\UCRepo\FilledInTemplates\IEC_UseCaseTemplateGOOD_TIDY.xml"));
            //XDocument xmlTree = XDocument.Load(strXMLSource);

            //xslt.Transform(xmlTree.CreateReader(), writer);

            //writer.Close();
        }

    }

}

