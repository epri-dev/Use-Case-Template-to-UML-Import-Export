using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UCRepoClassLibrary;
using System.Reflection;

namespace EPRi
{
    public class EAUseCasePlugin
    {
        MainForm importWindow;
        ExportForm exportWindow;

        //EA.Repository mRepository;

        public String EA_Connect(EA.Repository Repository)
        {
            //mRepository = Repository;
            // No special processing req'd
            return "";
        }

        public void EA_ShowHelp(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            MessageBox.Show("Help for: " + MenuName + "/" + ItemName);
        }

        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            if (Repository != null)
            {
                dynamic context = Repository.GetContextObject();
                if (context != null && context.ObjectType == (int)EA.ObjectType.otPackage)
                {
                    switch (MenuName)
                    {
                        case "":
                            return "-&EPRi";
                        case "-&EPRi":
                            if (context.ParentID == 0)
                            {
                                string[] ar = { "&Import Use Case", "&Export Use Case", "-", "&Copy Sample Files", "&About" };
                                return ar;
                            }
                            else
                            {
                                string[] ar = { "&Import Use Case", "-", "&Copy Sample Files", "&About" };
                                return ar;
                            }
                    }
                }

                switch (MenuName)
                {
                    case "":
                        return "-&EPRi";
                    case "-&EPRi":
                        string[] ar = { "&Copy Sample Files", "About" };
                        return ar;
                }
            }
            return "";
        }

        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            //var shareExists = EAPluginImporter.CheckIfShareExists();

            if (IsProjectOpen(Repository))
            {   // If no open project, disable all menu options
                //if (!shareExists)
                //{
                //    if (ItemName == "&Import Use Case")
                //        IsEnabled = false;

                //    else if (ItemName == "&Export")
                //        IsEnabled = false;
                //}
            }
            else if (Location != "MainMenu")
            {
                IsEnabled = false;
            }
        }


        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
                case "&Import Use Case":
                    Import(Repository);
                    break;
                case "&Export Use Case":
                    Export(Repository);
                    break;
                case "&Copy Sample Files":
                    CopySampleFiles();
                    break;
                case "&test":
                    var model = Repository.Models.GetByName("Usecaserepository");
                    break;
                case "&About":
                    var about = new AboutBox();
                    about.ShowDialog();
                    break;
            }
        }

        public void EA_Disconnect()
        {
            importWindow = null;
            exportWindow = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }



        private void Import(EA.Repository Repository)
        {
            //var shareExists = EAPluginImporter.CheckIfShareExists();
            //if (shareExists)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "Word 2007 Document(*.docx)|*.docx|XML Document(*.xml)|*.xml|All Files|*.docx;*.xml";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(dlg.FileName) == ".docx")
                        importWindow = new MainForm(Repository, dlg.FileName, "DOCX");
                    else if (Path.GetExtension(dlg.FileName) == ".xml")
                        importWindow = new MainForm(Repository, dlg.FileName, "XML");
                    importWindow.ShowDialog();
                }
            }
            //else
            //{
            //    MessageBox.Show("Please create a Data share folder first");
            //    CreateNewUserDataFolder();
            //}
        }

        private void Export(EA.Repository Repository)
        {
            exportWindow = new ExportForm(Repository);
            exportWindow.ShowDialog();
        }

        private void CopySampleFiles()
        {
            FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            folderBrowserDialog.Description = "Select a folder to copy sample data files.";

            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string strUserResourcesFolder = folderBrowserDialog.SelectedPath;

                try
                {
                    using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("EPRi.Samples.EmptyUCRepo.eap"))
                    {
                        using (var fileStream = File.Create(strUserResourcesFolder + "\\EmptyUCRepo.eap"))
                        {
                            strm.Seek(0, SeekOrigin.Begin);
                            strm.CopyTo(fileStream);
                        }
                    }
                }
                catch { }

                try
                {
                    using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("EPRi.Samples.ExchangeProfileXML.xml"))
                    {
                        using (var fileStream = File.Create(strUserResourcesFolder + "\\ExchangeProfileXML.xml"))
                        {
                            strm.Seek(0, SeekOrigin.Begin);
                            strm.CopyTo(fileStream);
                        }
                    }
                }
                catch { }

                try
                {
                    using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("EPRi.Samples.IEC_UseCaseTemplateGOOD.docx"))
                    {
                        using (var fileStream = File.Create(strUserResourcesFolder + "\\IEC_UseCaseTemplateGOOD.docx"))
                        {
                            strm.Seek(0, SeekOrigin.Begin);
                            strm.CopyTo(fileStream);
                        }
                    }
                }
                catch { }

                MessageBox.Show("Files copied successfully.", "Success");

            }
        }

        private void CreateNewUserDataFolder()
        {
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

            UCRepoClassLibrary.EAImporter theEAImporter;

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.DataShareLog);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(this.DataShareLog);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);
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

        public void DataShareLog(string strType, string strMsg)
        {
            MessageBox.Show(String.Format("{0}: {1}", strType, strMsg));
        }



        public void FormLogMsg(string strType, string strMsg)
        {
            MessageBox.Show(String.Format("{0}: {1}", strType, strMsg));
        }

        public void FormLogError(string strType, string strMsg)
        {
            MessageBox.Show(String.Format("{0}: {1}", strType, strMsg));
        }

    }
}
