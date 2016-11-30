using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UCRepoClassLibrary;

namespace EPRi
{
    public partial class MainForm : Form
    {
        private List<LogMessage> Logs { get; set; }
        private bool mAllowClosing = true;
        public EA.Repository mRepository;
        public string mFilePath;
        public string mType;

        public MainForm(EA.Repository Repository, string documentPath, string type)
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;
            this.FormClosing += MainForm_FormClosing;

            mRepository = Repository;
            mFilePath = documentPath;
            mType = type;
            FormLogMessage(String.Format("Importing {0}", documentPath));
        }

        protected void MainForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Import();
        }

        public void FormLogMessage(string strMsg)
        {
            Application.DoEvents();
            Log(strMsg, LogMessageType.Message);
        }

        public void FormLogInfo(string strType, string strMsg)
        {
            Application.DoEvents();
            Log(String.Format("{0}: {1}", strType, CleanInput(strMsg)), LogMessageType.Info);

        }

        public void FormLogError(string strType, string strMsg)
        {
            Application.DoEvents();

            Log(CleanInput(strMsg), LogMessageType.Error);
        }

        private void Log(string strMsg, LogMessageType type)
        {
            if (Logs == null)
                Logs = new List<LogMessage>();

            Logs.Add(new LogMessage
            {
                type = LogMessageType.Error,
                Message = String.Format("{0}\r\n", strMsg)
            });

            if (checkBox1.Checked || type != LogMessageType.Info)
            {
                richTextBox1.AppendText(String.Format("{0}\r\n", strMsg));
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.ScrollToCaret();
            }
        }

        private string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\s\.\:@-\\]", "", RegexOptions.None);
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {

        }

        private void Import()
        {
            FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            try
            {
                if (String.IsNullOrEmpty(Properties.Settings.Default.DefaultResourceFolder) == false)
                    folderBrowserDialog.SelectedPath = Properties.Settings.Default.DefaultResourceFolder;
            }
            catch { }

            //folderBrowserDialog.RootFolder = Environment.SpecialFolder.Personal;
            folderBrowserDialog.Description = "Select a folder for support data files.";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Properties.Settings.Default.DefaultResourceFolder = folderBrowserDialog.SelectedPath;
                    Properties.Settings.Default.Save();
                }
                catch { }

                this.ToggleButtons();

                if (mType == "DOCX")
                    ImportDocument(folderBrowserDialog.SelectedPath);
                else if (mType == "XML")
                    ImportXML(folderBrowserDialog.SelectedPath);

                this.ToggleButtons();
            }

        }

        private void ImportDocument(string supportFilePath)
        {
            try
            {
                EAImporter.LogMsgCallbackType myLogMsgCallback = new EAImporter.LogMsgCallbackType(this.FormLogInfo);
                EAImporter.LogMsgCallbackType myErrorMsgCallback = new EAImporter.LogMsgCallbackType(this.FormLogError);

                var theEAImporter = new EAPluginImporter(myLogMsgCallback, myErrorMsgCallback);
                if (theEAImporter.SetupRepo(mRepository) != 0)
                    return;

                theEAImporter.SetupDataFolder(supportFilePath);

                int iErrorCount = theEAImporter.ImportWordDoc(mFilePath);
                theEAImporter.Close();

                if (iErrorCount == 0)
                {
                    FormLogMessage("Import completed successfully");
                }
                else
                {
                    FormLogMessage("Import ABORTED.");
                }
            }
            catch
            {
                FormLogMessage("Something unexpected happened while importing. Import Aborted");
            }


        }

        private void ImportXML(string supportFilePath)
        {
            try
            {
                EAImporter.LogMsgCallbackType myLogMsgCallback = new EAImporter.LogMsgCallbackType(this.FormLogInfo);
                EAImporter.LogMsgCallbackType myErrorMsgCallback = new EAImporter.LogMsgCallbackType(this.FormLogError);

                var theEAImporter = new EAPluginImporter(myLogMsgCallback, myErrorMsgCallback);
                if (theEAImporter.SetupRepo(mRepository) != 0)
                    return;

                theEAImporter.SetupDataFolder(supportFilePath);

                int iErrorCount = theEAImporter.ImportXMLFile(mFilePath);
                theEAImporter.Close();

                if (iErrorCount == 0)
                {
                    FormLogMessage("Import completed successfully");
                }
                else
                {
                    FormLogMessage("Import ABORTED.");
                }
            }
            catch
            {
                FormLogMessage("Something unexpected happened while importing. Import Aborted");
            }
        }

        protected void MainForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!mAllowClosing)
                e.Cancel = true;
        }

        protected void okButton_Click(object sender, EventArgs e)
        {
            Import();
        }

        protected void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            var logsToDisplay = Logs;
            if (!checkBox1.Checked)
            {
                logsToDisplay = Logs.Where(l => l.type != LogMessageType.Info).ToList();
            }

            foreach (var log in logsToDisplay)
            {
                richTextBox1.AppendText(log.Message);
            }

            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.ScrollToCaret();
        }

        public void ToggleButtons()
        {
            mAllowClosing = !mAllowClosing;
        }
    }
}
