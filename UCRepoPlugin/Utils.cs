using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EPRi
{
    public enum DocumentType
    {
        DOCX,
        XML
    }

    public class Utils
    {
        private static string checkAndCreateDefaultFolder()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\EPRi Share";

            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch
            {
                return String.Empty;
            }

            return folder;
        }

        private static bool isPathValid(string path)
        {
            bool flag = false;

            if (String.IsNullOrEmpty(path) || path.Length > 50)
            {
            }
            else if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                    flag = true;
                }
                catch { }
            }
            else
                flag = true;

            return flag;
        }

        public static string GetSharePath()
        {
            string errormessage = "The shared folder path exceeds the maximum character limit.\r\n{0}\r\nPlease choose another folder path that is shorter.";

            string selectedPath = String.Empty;
            try
            {
                if (String.IsNullOrEmpty(Properties.Settings.Default.ResourceFolder) == false)
                    selectedPath = Properties.Settings.Default.ResourceFolder;
            }
            catch { }

            if (!isPathValid(selectedPath))
            {
                selectedPath = checkAndCreateDefaultFolder();

                if (!isPathValid(selectedPath))
                {
                    if (selectedPath.Length > 50)
                        MessageBox.Show(String.Format(errormessage, selectedPath), "Message");

                    FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                    folderBrowserDialog.Description = "Select a folder for support data files. Creation of a new Epri Share folder is recommended.";

                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (folderBrowserDialog.SelectedPath.Length > 50)
                        {
                            var result = MessageBox.Show(String.Format(errormessage, folderBrowserDialog.SelectedPath), "Message", MessageBoxButtons.OKCancel);
                            if (result == System.Windows.Forms.DialogResult.OK)
                                selectedPath = GetSharePath();
                            else
                                selectedPath = string.Empty;
                        }
                        else
                            selectedPath = folderBrowserDialog.SelectedPath;
                    }
                    else
                        selectedPath = string.Empty;
                }

                try
                {
                    Properties.Settings.Default.ResourceFolder = selectedPath;
                    Properties.Settings.Default.Save();
                }
                catch { }
            }

            if (!String.IsNullOrEmpty(selectedPath))
            {
                if (!Directory.Exists(selectedPath))
                {
                    try
                    {
                        Directory.CreateDirectory(selectedPath);
                    }
                    catch
                    {
                        selectedPath = string.Empty;
                    }
                }
            }
            return selectedPath;
        }

    }
}
