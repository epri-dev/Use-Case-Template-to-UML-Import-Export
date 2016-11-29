using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UCRepoClassLibrary;

namespace EPRi
{
    public partial class ExportForm : Form
    {
        #region Properties
        private List<LogMessage> Logs { get; set; }

        public EA.Repository mRepository;

        private bool mAllowClosing = true;
        #endregion

        public ExportForm(EA.Repository Repository)
        {
            InitializeComponent();
            this.Shown += ExportForm_Shown;
            this.FormClosing += ExportForm_FormClosing;

            mRepository = Repository;
        }

        protected void ExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mAllowClosing)
                e.Cancel = true;
        }

        protected void ExportForm_Shown(object sender, EventArgs e)
        {
            ToggleButtons();
            GetPackages();
            ToggleButtons();
        }

        public void FormLogMessage(string strMsg)
        {
            Application.DoEvents();
            if (Logs == null)
                Logs = new List<LogMessage>();

            Logs.Add(new LogMessage
            {
                type = LogMessageType.Message,
                Message = String.Format("{0}\r\n", strMsg)
            });

            richTextBox1.AppendText(String.Format("{0}\r\n", strMsg));
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.ScrollToCaret();
        }

        public void FormLogInfo(string strType, string strMsg)
        {
            Application.DoEvents();
            if (Logs == null)
                Logs = new List<LogMessage>();

            Logs.Add(new LogMessage
            {
                type = LogMessageType.Info,
                Message = String.Format("{0}: {1}\r\n", strType, strMsg)
            });

            if (checkBox1.Checked)
            {
                richTextBox1.AppendText(String.Format("{0}: {1}\r\n", strType, strMsg));
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;
                richTextBox1.ScrollToCaret();
            }
        }

        public void FormLogError(string strType, string strMsg)
        {
            Application.DoEvents();
            if (Logs == null)
                Logs = new List<LogMessage>();

            Logs.Add(new LogMessage
            {
                type = LogMessageType.Error,
                Message = String.Format("{0}\r\n", strMsg)
            });

            richTextBox1.AppendText(String.Format("{0}\r\n", strMsg));
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.ScrollToCaret();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "HTML Document(*.html)|*.html|All Files|*.html";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FormLogMessage(String.Format("Exporting to {0}", dlg.FileName));
                ClearLog();
                ToggleButtons();
                ExportPackage(dlg.FileName);
                ToggleButtons();

            }
        }

        private void ExportPackage(string filename)
        {
            string strUseCasePackage = comboBoxExpPackage.Text;
            try
            {
                EAExporter.LogMsgCallbackType myLogMsgCallback = new EAExporter.LogMsgCallbackType(this.FormLogInfo);
                EAExporter.LogMsgCallbackType myErrorMsgCallback = new EAExporter.LogMsgCallbackType(this.FormLogError);

                EAPluginExporter theEAExporter = new EAPluginExporter(myLogMsgCallback, myErrorMsgCallback);
                if (theEAExporter.SetupRepo(mRepository) != 0)
                    return;

                int iErrorCount = theEAExporter.Export(strUseCasePackage, filename);
                theEAExporter.Close();

                if (iErrorCount == 0)
                {
                    FormLogMessage("Export completed successfully");
                }
            }
            catch { }
        }

        private void btnRefreshPackages_Click(object sender, EventArgs e)
        {
            ToggleButtons();
            GetPackages();
            ToggleButtons();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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

        private void GetPackages()
        {
            comboBoxExpPackage.Items.Clear();
            ClearLog();

            try
            {
                EAExporter.LogMsgCallbackType myLogMsgCallback = new EAExporter.LogMsgCallbackType(this.FormLogInfo);
                EAExporter.LogMsgCallbackType myErrorMsgCallback = new EAExporter.LogMsgCallbackType(this.FormLogError);

                EAPluginExporter theEAExporter = new EAPluginExporter(myLogMsgCallback, myErrorMsgCallback);

                if (theEAExporter.SetupRepo(mRepository) != 0)
                    return;

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
            catch
            {
                FormLogMessage("Something unexpected happened while getting existing use cases.");
            }
        }

        private void ClearLog()
        {
            Logs = new List<LogMessage>();
            richTextBox1.Clear();
        }

        private void ToggleButtons()
        {
            btnRefreshPackages.Enabled = !btnRefreshPackages.Enabled;
            Export.Enabled = !Export.Enabled;
            mAllowClosing = !mAllowClosing;
        }

        public void AllowClosing()
        {
            mAllowClosing = true;
        }

    }
}
