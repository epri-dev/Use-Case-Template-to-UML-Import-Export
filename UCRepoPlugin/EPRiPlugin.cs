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
                            return "-&EPRI Use Case Importer";
                        case "-&EPRI Use Case Importer":
                            if (context.ParentID == 0)
                            {
                                string[] ar = { "&Import Use Case", "&Export Use Case", "-", "&First time config", "&About" };
                                return ar;
                            }
                            else
                            {
                                string[] ar = { "&Import Use Case", "-", "&First time config", "&About" };
                                return ar;
                            }
                    }
                }

                switch (MenuName)
                {
                    case "":
                        return "-&EPRI Use Case Importer";
                    case "-&EPRI Use Case Importer":
                        string[] ar = { "&First time config", "&About" };
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
                case "&First time config":
                    InitPlugin();
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
            try
            {
                //Repository.GetProjectInterface().ReloadProject();
                //Repository.OpenFile(Repository.ConnectionString);
            }
            catch { }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Word 2007 Document(*.docx)|*.docx|XML Document(*.xml)|*.xml|All Files|*.docx;*.xml";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string extension = String.Empty;
                try
                {
                    extension = Path.GetExtension(dlg.FileName);
                }
                catch { }


                if (extension == ".docx")
                    importWindow = new MainForm(Repository, dlg.FileName, DocumentType.DOCX);
                else if (extension == ".xml")
                    importWindow = new MainForm(Repository, dlg.FileName, DocumentType.XML);
                else
                {
                    MessageBox.Show("Invalid file type selected. Only \".docx\" and \".xml\" files are accepted. please select a new file and try again.", "EPRi Use Case Plugin");
                    return;
                }

                string sharePath = Utils.GetSharePath();
                if (String.IsNullOrEmpty(sharePath))
                {
                    importWindow.Dispose();
                    return;
                }

                copySampleFiles(sharePath);

                importWindow.sharePath = sharePath;
                importWindow.ShowDialog();
            }

        }

        private void Export(EA.Repository Repository)
        {
            try
            {
                Repository.OpenFile(Repository.ConnectionString);
            }
            catch { }

            exportWindow = new ExportForm(Repository);
            exportWindow.ShowDialog();
        }

        private void InitPlugin()
        {
            string strUserResourcesFolder = Utils.GetSharePath();
            if (String.IsNullOrEmpty(strUserResourcesFolder))
                return;

            copySampleFiles(strUserResourcesFolder);

            var help = new HelpBox(strUserResourcesFolder);
            help.ShowDialog();
        }

        private void copySampleFiles(string strUserResourcesFolder)
        {
            try
            {
                if (!File.Exists(strUserResourcesFolder + "\\EmptyUCRepo.eap"))
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
            }
            catch { }

            try
            {
                if (!File.Exists(strUserResourcesFolder + "\\ExchangeProfileXML.xml"))
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
            }
            catch { }

            try
            {
                if (!File.Exists(strUserResourcesFolder + "\\IEC_UseCaseTemplateGOOD.docx"))
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
            }
            catch { }

            //MessageBox.Show("Files copied successfully.", "Success");

        }


        private void CreateNewUserDataFolder()
        {
            string strUserResourcesFolder = Utils.GetSharePath();
            if (String.IsNullOrEmpty(strUserResourcesFolder))
                return;

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
            MessageBox.Show(String.Format("{0}: {1}", strType, strMsg), "EPRi Use Case Plugin");
        }

        public void FormLogMsg(string strType, string strMsg)
        {
            MessageBox.Show(String.Format("{0}: {1}", strType, strMsg), "EPRi Use Case Plugin");
        }

        public void FormLogError(string strType, string strMsg)
        {
            MessageBox.Show(String.Format("{0}: {1}", strType, strMsg), "EPRi Use Case Plugin");
        }

    }
}
