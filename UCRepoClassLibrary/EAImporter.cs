/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: EAImporter.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 16 $

	Date last modified: $Modtime: 11/28/12 11:54a $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/EAImporter.cs 16    12/10/12 3:00p Ronp $

$History: EAImporter.cs $
 * 
 * *****************  Version 16  *****************
 * User: Ronp         Date: 12/10/12   Time: 3:00p
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
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
 * *****************  Version 15  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
 * Update keyword expansion

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; // used for Application.DoEvents -- may need to remove.
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;
using System.Runtime.InteropServices;
using System.Management;
using Word = Microsoft.Office.Interop.Word;

using System.Security.Principal;
using System.Security.AccessControl;

namespace UCRepoClassLibrary
{
    public class EAImporter
    {
        public delegate void LogMsgCallbackType(string strType, string strMsg);


        #region Properties
        public static EA.Repository m_Repository;

        internal static ImportPackages m_Packages;
        internal static ImportInfoModels m_InfoModels;
        internal static ImportActors m_Actors;
        internal static ImportUseCase m_ImportUseCase;
        internal static ImportDomains m_Domains;
        internal static ImportRequirements m_Requirements;

        protected Boolean m_bCloseEAFileOnExit = false;
        protected Boolean m_bCloseEAOnExit = false;

        protected static LogMsgCallbackType m_LogMsgCallback;
        protected static LogMsgCallbackType m_ErrorMsgCallback;

        protected static Word._Application m_oWord = null;

        internal static int m_iActorBaseClsID = 0;
        internal static int m_iNarrativeBaseClsID;
        internal static int m_iReferenceInfoBaseClsID;
        internal static int m_iVersionInfoBaseClsID = 0;
        internal static int m_iGeneralRemarkBaseClsID = 0;
        internal static int m_iObjectiveBaseClsID = 0;
        internal static int m_iRef_DomainBaseClsID = 0;
        internal static int m_iUseCaseRelationsClsID = 0;
        internal static int m_iRefUseCaseClsID = 0;
        internal static int m_iConditionBaseClsID = 0;
        internal static int m_iPreConditionBaseClsID = 0;
        internal static int m_iRef_ActorClsID = 0;
        internal static int m_UseCaseBaseClsID = 0;
        internal static int m_ScenarioBaseClsID = 0;
        internal static int m_iBusinessCaseBaseClsID = 0;

        internal static string m_strImportFilePath = "";
        internal static string m_SharePath = "";

        internal static string ImportPackagePath { get; set; }

        internal string FileName { get; set; }

        internal string FilePath { get; set; }

        internal string XMLFilePath { get; set; }

        internal string XSLTPath { get; set; }

        internal static Dictionary<int, string> Locations { get; set; }
        #endregion

        #region Constructors
        public EAImporter(LogMsgCallbackType myLogMsgCallback, LogMsgCallbackType myErrorMsgCallback)
        {
            m_LogMsgCallback = myLogMsgCallback;
            m_ErrorMsgCallback = myErrorMsgCallback;
        }
        #endregion

        #region Logging
        public enum LogMsgType { Adding, Added, Exists, MiscExceptions, Info };
        /// <summary>
        /// LogMsg 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="strMsg"></param>
        static public void LogMsg(LogMsgType eType, string strMsg)
        {
            string strType = "";
            switch (eType)
            {
                case LogMsgType.Adding:
                    //strType = "Added";
                    strType = "Adding";
                    break;
                case LogMsgType.Added:
                    //strType = "Added";
                    strType = "\tAdded";
                    break;
                case LogMsgType.Exists:
                    //strType = "Exists";
                    strType = "\tAlready exists";
                    break;
                case LogMsgType.MiscExceptions:
                    strType = "Miscelaneous exception";
                    // dont log for now
                    return;
                case LogMsgType.Info:
                    strType = "Info";
                    break;
                default:
                    strType = "Unknown";
                    break;
            }

            m_LogMsgCallback(strType, strMsg);
        }
        public enum LogErrorLevel { A };
        /// <summary>
        /// LogError
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="strMsg"></param>
        public static void LogError(LogErrorLevel eLevel, string strMsg)
        {
            string strType = "";
            switch (eLevel)
            {
                case LogErrorLevel.A:
                    strType = "ERROR";
                    break;
                //case LogErrorLevel.MISC:
                //    strType = "MISC";
                //    break;
            }

            m_ErrorMsgCallback(strType, strMsg);
        }
        #endregion

        internal static string Recurse(XElement XelCurr)
        {
            string strErrorInfo = "";

            bool bRecurse = true;

            while (bRecurse)
            {
                string strElName = "";
                try
                {
                    strElName = XelCurr.Element("Name").Value;
                }
                catch (Exception ex2)
                {
                    EAImporter.LogMsg(LogMsgType.MiscExceptions, ex2.Message);
                }

                if (strElName != "")
                {
                    strErrorInfo = XelCurr.Name + "(" + strElName + ")/" + strErrorInfo;
                }
                else
                {
                    strErrorInfo = XelCurr.Name + "/" + strErrorInfo;
                }

                if (XelCurr.Parent == null)
                {
                    bRecurse = false;
                }
                else
                {
                    XelCurr = XelCurr.Parent;
                }
            }

            if (strErrorInfo.Contains("{"))
            {
                strErrorInfo = strErrorInfo.Remove(strErrorInfo.IndexOf("{"), strErrorInfo.IndexOf("}") - strErrorInfo.IndexOf("{") + 1);
            }

            return strErrorInfo;
        }

        internal static string GetRepositoryPath()
        {
            if (m_Repository == null)
            {
                return "";
            }

            string strPath = m_Repository.ConnectionString.Substring(0, m_Repository.ConnectionString.LastIndexOf("\\"));

            return strPath;
        }


        protected int AddIKBModel()
        {
            bool bNotFound = false;

            m_Repository.Models.Refresh();
            EA.Package selectedPackage = m_Repository.GetTreeSelectedPackage();

            try
            {
                if (selectedPackage.ParentID == 0 && selectedPackage.Name.ToLower() != "usecaserepository")
                    m_Packages.Add("UseCaseRepository", selectedPackage.Packages.GetByName("UseCaseRepository"));
                else if (selectedPackage.ParentID > 0)
                    m_Packages.Add("UseCaseRepository", selectedPackage.Packages.GetByName("UseCaseRepository"));
                else
                    m_Packages.Add("UseCaseRepository", m_Repository.Models.GetByName("UseCaseRepository"));
            }
            catch (Exception ex)
            {
                LogMsg(LogMsgType.MiscExceptions, m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
                bNotFound = true;
            }

            if ((m_Packages["UseCaseRepository"] == null) || (bNotFound))
            {
                if (selectedPackage.ParentID == 0 && selectedPackage.Name.ToLower() != "usecaserepository")
                    m_Packages.Add("UseCaseRepository", selectedPackage.Packages.AddNew("UseCaseRepository", "Nothing"));
                else if (selectedPackage.ParentID > 0)
                    m_Packages.Add("UseCaseRepository", selectedPackage.Packages.AddNew("UseCaseRepository", "Nothing"));
                else
                    m_Packages.Add("UseCaseRepository", m_Repository.Models.AddNew("UseCaseRepository", "Nothing"));

                ((EA.Package)m_Packages["UseCaseRepository"]).Update();
            }

            return 0;
        }

        protected int InitIKBStructure()
        {
            int iErrorCount = 0;

            LogMsg(LogMsgType.Info, "Initialize repository structure");

            //AddStereotype("Device");
            //AddStereotype("Application");
            //AddStereotype("Data");
            //AddStereotype("Facility");
            //AddStereotype("Organization");
            //AddStereotype("Person");
            //AddStereotype("Type");

            m_Repository.Models.Refresh();

            if (m_Packages.Add("UseCaseRepository", "DomainLibrary") == null) { iErrorCount++; };
            if (m_Packages.Add("UseCaseRepository", "ActorLibrary") == null) { iErrorCount++; };
            if (m_Packages.Add("UseCaseRepository", "InformationModelLibrary") == null) { iErrorCount++; };
            if (m_Packages.Add("UseCaseRepository", "RequirementLibrary") == null) { iErrorCount++; };
            if (m_Packages.Add("UseCaseRepository", "UseCaseLibrary") == null) { iErrorCount++; };

            LogMsg(LogMsgType.Info, "Initialized repository structure");

            return iErrorCount;
        }

        private int StartEA()
        {
            EA.App App;

            bool bEANotRunning = false;

            LogMsg(LogMsgType.Info, "Attempt connection to EA");

            try
            {
                App = (EA.App)Marshal.GetActiveObject("EA.App");

                LogMsg(LogMsgType.Info, "Connected to EA");

                m_Repository = App.Repository;

                m_bCloseEAOnExit = false;
            }
            catch (Exception ex)
            {
                //  could not get running EA
                bEANotRunning = true;

                if (m_Repository != null)
                {
                    try
                    {
                        LogMsg(LogMsgType.MiscExceptions, m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
                    }
                    catch (Exception ex2)
                    {
                    }
                }
                else
                {

                }
            }

            if (bEANotRunning)
            {
                // EA not running... start it up
                LogMsg(LogMsgType.Info, "Could not connect to running to EA ... start new instance");

                m_Repository = new EA.Repository();

                LogMsg(LogMsgType.Info, "Connected to EA");

                m_bCloseEAOnExit = true;

            }

            return 0;
        }

        /// <summary>
        /// Open
        /// </summary>
        public int Open(string strRepositoryFullPath)
        {
            //string ResourceFilePathPrefix;
            //string _strDataPath;

            strRepositoryFullPath = System.IO.Path.GetFullPath(strRepositoryFullPath);

            if (!System.IO.File.Exists(strRepositoryFullPath))
            {
                LogError(LogErrorLevel.A, "Could not load repository. File " + strRepositoryFullPath + " does not exist.");
                return 1;
            }

            //if (System.Diagnostics.Debugger.IsAttached)
            //{

            //    //In Debugging mode  
            //    ResourceFilePathPrefix = Path.GetFullPath(Application.StartupPath);

            //    _strDataPath = Path.GetFullPath(ResourceFilePathPrefix) + @"\..\..\..\UCRepoClientApp\Resources\libtidy.dll";

            //}
            //else
            //{
            //    //In Published mode  
            //    ResourceFilePathPrefix = Application.StartupPath;

            //    _strDataPath = Path.GetFullPath(ResourceFilePathPrefix) + @"\Resources\libtidy.dll";

            //}

            //// make sure libtidy.dll is in exec path:
            //string destFile = Application.StartupPath + @"\libtidy.dll";

            //if (!System.IO.File.Exists(destFile))
            //{
            //    System.IO.File.Copy(_strDataPath, destFile);//TBD
            //}


            int iRes = StartEA();


            if (m_Repository.ConnectionString == strRepositoryFullPath)
            {
                LogMsg(LogMsgType.Info, "Model already loaded.");
                m_bCloseEAFileOnExit = false;
            }
            else
            {
                LogMsg(LogMsgType.Info, "Opening model");
                try
                {
                    m_Repository.OpenFile(strRepositoryFullPath);
                }
                catch (Exception ex)
                {
                    LogError(LogErrorLevel.A, "Could not load repository. Error:" + ex.Message);
                    return 1;
                }

                m_bCloseEAFileOnExit = true;
                LogMsg(LogMsgType.Info, "Opened model");
            }

            m_oWord = new Word.Application();
            m_oWord.Visible = false;

            m_iActorBaseClsID = m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Actor").ElementID;
            m_iNarrativeBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Narrative").ElementID;
            m_iReferenceInfoBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Reference").ElementID;
            m_iVersionInfoBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("VersionInformation").ElementID;
            m_iGeneralRemarkBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("GeneralRemark").ElementID;
            m_iObjectiveBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Objective").ElementID;
            m_iRef_DomainBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Ref_Domain").ElementID;
            m_iUseCaseRelationsClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("UseCaseRelation").ElementID;
            m_iRefUseCaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Ref_UseCase").ElementID;
            m_iConditionBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Condition").ElementID;
            m_iPreConditionBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Precondition").ElementID;
            m_iRef_ActorClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Ref_Actor").ElementID;
            m_UseCaseBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("UseCase").ElementID;
            m_ScenarioBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Scenario").ElementID;
            m_iBusinessCaseBaseClsID = EAImporter.m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("BusinessCase").ElementID;

            return 0;
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            // close the repository and tidy up
            try
            {
                if (m_bCloseEAFileOnExit)
                {
                    m_bCloseEAFileOnExit = false;
                    m_Repository.CloseFile();
                }

                if (m_bCloseEAOnExit)
                {
                    m_Repository.Exit();
                    m_bCloseEAOnExit = false;
                }
            }
            catch { }

            try
            {
                if (m_oWord != null)
                {
                    m_oWord.Quit();
                }
            }
            catch { }

            m_oWord = null;
        }


        /// <summary>
        /// AddDiagram
        /// </summary>
        /// <param name="ePackage"></param>
        /// <returns>EA.Diagram</returns>
        internal static EA.Diagram AddDiagram(EA.Package eAPackage, string strDiagramName, string strEADiagramType)
        {
            string strLogInfo = "Diagram: " + strEADiagramType + " " + strDiagramName;

            LogMsg(LogMsgType.Adding, strLogInfo);
            EA.Diagram diagram = null;

            try
            {
                diagram = eAPackage.Diagrams.GetByName(strDiagramName);
            }
            catch (Exception ex)
            {
                LogMsg(LogMsgType.MiscExceptions, m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
            }

            if (diagram == null)
            {
                diagram = eAPackage.Diagrams.AddNew(strDiagramName, strEADiagramType);
                diagram.Update();
                eAPackage.Diagrams.Refresh();
                LogMsg(LogMsgType.Added, strLogInfo);
            }
            else
            {
                LogMsg(LogMsgType.Exists, strLogInfo);
            }

            return diagram;
        }


        protected virtual string SetupTempBaseline()
        {
            EA.Project iProject = m_Repository.GetProjectInterface();
            string strPackageGUID = ((EA.Package)m_Packages["UseCaseRepository"]).PackageGUID;

            // first check if baseline alread exists:
            string strBaseLines = iProject.GetBaselines(strPackageGUID, "");
            StringReader strXMLSource = new StringReader(strBaseLines);
            XDocument xBaselines = XDocument.Load(strXMLSource);
            string sBaselineGUID = "";
            try
            {
                sBaselineGUID = xBaselines.XPathSelectElement("EA.BaseLines/Baseline[@version='EPRiImport1A']").Attribute("guid").Value;
            }
            catch (Exception ex)
            {
                LogMsg(LogMsgType.MiscExceptions, "Exception caught checking for baseline. " + ex.Message);
            }

            if (sBaselineGUID != "")
            {
                throw new EpriException("Could not create temporary project baseline. Baseline 'EPRiImport 1A' already exsts. Please delete this baseline and make sure to check other baselines.", "");

                // seems to already exist
                //LogError(LogErrorLevel.A, "Could not create temporary project baseline. Baseline 'EPRiImport 1A' already exsts. Please delete this baseline and make sure to check other baselines.");

                return "";
            }


            bool bRes = iProject.CreateBaseline(strPackageGUID, "EPRiImport1A", "");

            if (bRes == false)
            {
                throw new EpriException("Could not create project baseline.", "");
                //LogError(LogErrorLevel.A, "Could not create project baseline");
                return "";
            }

            strBaseLines = iProject.GetBaselines(strPackageGUID, "");
            strXMLSource = new StringReader(strBaseLines);
            xBaselines = XDocument.Load(strXMLSource);
            sBaselineGUID = xBaselines.XPathSelectElement("EA.BaseLines/Baseline[@version='EPRiImport1A']").Attribute("guid").Value;

            return sBaselineGUID;
        }

        protected virtual void CleanupTempBaseline(string sBaselineGUID, int iErrorCount)
        {
            string strPackageGUID = ((EA.Package)m_Packages["UseCaseRepository"]).PackageGUID;
            EA.Project iProject = m_Repository.GetProjectInterface();

            if (iErrorCount != 0)
            {
                string strMergeInstructions = "<Merge><MergeItem guid=\"RestoreAll\" changed=\"true\" baselineOnly=\"true\" modelOnly=\"true\" moved=\"true\" fullRestore=\"false\" /></Merge>";
                string sRes = iProject.DoBaselineMerge(strPackageGUID, sBaselineGUID, strMergeInstructions, "");
            }

            iProject.DeleteBaseline(sBaselineGUID);
        }

        public int SetupPaths(string strPathToWordDoc, string strFileName, string strXSLTFullPath)
        {
            FilePath = System.IO.Path.GetFullPath(strPathToWordDoc);
            XSLTPath = System.IO.Path.GetFullPath(strXSLTFullPath);
            FileName = strFileName;

            strFileName = strFileName.Replace(".docx", "");
            XMLFilePath = FilePath + "\\" + strFileName + "_MODEL.xml";

            if (!System.IO.File.Exists(strPathToWordDoc + "\\" + FileName))
            {
                LogError(LogErrorLevel.A, "Could not load word document. File " + strPathToWordDoc + "\\" + strFileName + " does not exist.");
                return 1;
            }

            if (!System.IO.File.Exists(strXSLTFullPath))
            {
                LogError(LogErrorLevel.A, "Could not load xslt. File " + strXSLTFullPath + " does not exist.");
                return 1;
            }

            return 0;
        }

        internal virtual int ConvertWordTemplateToXML(string documentPath)
        {
            string strRootFileName = FileName.Replace(".docx", "");
            string strWordToFilteredHTMLFileName = FilePath + "\\" + strRootFileName + ".html";
            string strTidied = FilePath + "\\" + strRootFileName + "_TIDY.xml";

            // Convert word doc to filtered HTML
            m_oWord.Visible = false;

            m_oWord.Documents.Open(documentPath);
            m_oWord.Visible = false;
            m_oWord.Documents[1].SaveAs(strWordToFilteredHTMLFileName, Word.WdSaveFormat.wdFormatFilteredHTML);
            m_oWord.Visible = false;
            m_oWord.Documents.Close();
            m_oWord.Visible = false;

            // Cleanup filtered HTML -- convert it to valid XML
            TidyManaged.Document TidyDoc = null;
            try
            {
                TidyDoc = TidyManaged.Document.FromFile(strWordToFilteredHTMLFileName);
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not load libtidy.dll Please place this file in the executable path. Addtional info: " + ex.Message);
                return 1;
            }

            TidyDoc.OutputXhtml = true;
            TidyDoc.DefaultAltText = "";

            TidyDoc.MakeBare = true;
            TidyDoc.DropProprietaryAttributes = true;
            //TidyDoc.MakeClean = true; // true seems to strip important styles
            //TidyDoc.DropEmptyParagraphs = true;
            //TidyDoc.JoinClasses = true;

            TidyDoc.OutputXml = true;
            TidyDoc.WrapAt = 0;

            TidyDoc.ShowWarnings = false;
            TidyDoc.Quiet = true;

            TidyDoc.CleanAndRepair();

            string parsed = TidyDoc.Save();

            // save the XML for testing only
            //TidyDoc.Save(strTidied);
            parsed = parsed.Replace("<div class=\"WordSection1\">", "");
            parsed = parsed.Replace("<div class=\"WordSection2\">", "");
            parsed = parsed.Replace("<div class=\"WordSection3\">", "");
            parsed = parsed.Replace("<div class=\"WordSection4\">", "");
            parsed = parsed.Replace("</div>", "");
            parsed = parsed.Replace("<div class='c82'>", "");
            parsed = parsed.Replace("&nbsp;", "");
            parsed = parsed.Replace("<div style='padding:4.35pt 7.95pt 4.35pt 7.95pt'>", "");

            string str1 = "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"";
            string str2 = "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\" [ <!ENTITY nbsp \"&#x00A0;\"> ]";

            parsed = parsed.Replace(str1, str2);

            File.WriteAllText(strTidied, parsed);

            // transform the XML to "MODEL" xml file
            XslCompiledTransform xslt = new XslCompiledTransform();

            xslt.Load(XSLTPath);

            // Create the writer.
            XmlWriterSettings XWriterSettings = new XmlWriterSettings();
            XWriterSettings.Indent = true;
            XWriterSettings.IndentChars = "\t";
            XmlWriter writer = XmlWriter.Create(XMLFilePath, XWriterSettings);

            StringReader strXMLSource = new StringReader(parsed);
            XDocument xmlTree = XDocument.Load(strXMLSource);

            xslt.Transform(xmlTree.CreateReader(), writer);

            writer.Close();

            return 0;
        }

        public virtual int ImportWordDoc(string documentPath)
        {
            if (ConvertWordTemplateToXML(documentPath) == 0)
            {
                return (ImportXMLFile());
            }
            else
            {
                return 1;
            }
        }


        protected int ShareFolder(string FolderPath, string ShareName, string Description)
        {
            try
            {
                // Create a ManagementClass object
                ManagementClass managementClass = new ManagementClass("Win32_Share");

                // Create ManagementBaseObjects for in and out parameters
                ManagementBaseObject inParams = managementClass.GetMethodParameters("Create");
                ManagementBaseObject outParams;

                // Set the input parameters
                inParams["Description"] = Description;
                inParams["Name"] = ShareName;
                inParams["Path"] = FolderPath;
                inParams["Type"] = 0x0; // Disk Drive
                //Another Type:
                //        DISK_DRIVE = 0x0
                //        PRINT_QUEUE = 0x1
                //        DEVICE = 0x2
                //        IPC = 0x3
                //        DISK_DRIVE_ADMIN = 0x80000000
                //        PRINT_QUEUE_ADMIN = 0x80000001
                //        DEVICE_ADMIN = 0x80000002
                //        IPC_ADMIN = 0x8000003
                inParams["MaximumAllowed"] = null;
                inParams["Password"] = null;
                inParams["Access"] = null; // Make Everyone has full control access.                
                //inParams["MaximumAllowed"] = int maxConnectionsNum;

                // Invoke the method on the ManagementClass object
                outParams = managementClass.InvokeMethod("Create", inParams, null);
                // Check to see if the method invocation was successful

                Console.WriteLine("Creation of calculator process returned: " + outParams["returnValue"]);

                //0 
                //Success
                //2
                //Access Denied
                //8
                //Unknown Failure
                //9
                //Invalid Name
                //10
                //Invalid Level
                //21
                //Invalid Parameter
                //22
                //Duplicate Share
                //23
                //Redirected Path
                //24
                //Unknown Device or Directory
                //25
                //Net Name Not Found

                if ((uint)(outParams.Properties["ReturnValue"].Value) == 22)
                {
                    // share already exits
                }

                if (((uint)(outParams.Properties["ReturnValue"].Value) != 0) && ((uint)(outParams.Properties["ReturnValue"].Value) != 22))
                {
                    LogError(LogErrorLevel.A, "Error sharing MMS folders. Please make sure you run as Administrator. Error:" + outParams.Properties["ReturnValue"].Value);
                    return 1;
                }

                //user selection
                NTAccount ntAccount = new NTAccount("Everyone");

                //SID
                SecurityIdentifier userSID = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
                byte[] utenteSIDArray = new byte[userSID.BinaryLength];
                userSID.GetBinaryForm(utenteSIDArray, 0);

                //Trustee
                ManagementObject userTrustee = new ManagementClass(new ManagementPath("Win32_Trustee"), null);
                userTrustee["Name"] = "Everyone";
                userTrustee["SID"] = utenteSIDArray;

                //ACE
                ManagementObject userACE = new ManagementClass(new ManagementPath("Win32_Ace"), null);
                userACE["AccessMask"] = 2032127;                                 //Full access
                userACE["AceFlags"] = AceFlags.ObjectInherit | AceFlags.ContainerInherit;
                userACE["AceType"] = AceType.AccessAllowed;
                userACE["Trustee"] = userTrustee;

                ManagementObject userSecurityDescriptor = new ManagementClass(new ManagementPath("Win32_SecurityDescriptor"), null);
                userSecurityDescriptor["ControlFlags"] = 4; //SE_DACL_PRESENT
                userSecurityDescriptor["DACL"] = new object[] { userACE };
                //can declare share either way, where "ShareName" is the name used to share the folder
                //ManagementPath path = new ManagementPath("Win32_Share.Name='" + ShareName + "'");
                //ManagementObject share = new ManagementObject(path);
                ManagementObject share = new ManagementObject(managementClass.Path + ".Name='" + ShareName + "'");

                share.InvokeMethod("SetShareInfo", new object[] { Int32.MaxValue, Description, userSecurityDescriptor });

            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Error sharing folders. Please make sure you run as Administrator. ERROR: " + ex.Message);
                return 1;
            }

            return 0;
        }

        private bool CheckIfShareExists()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_share");
                ManagementClass mc = new ManagementClass("Win32_Share");

                foreach (ManagementObject mo in mc.GetInstances())
                {
                    if (mo["Name"].ToString() == "EAImages")
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not find EAImages share. Exception:" + ex.Message);
            }

            return false;
        }

        public int DisableCurrentShare()
        {

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_share");
                ManagementBaseObject outParams; ManagementClass mc = new ManagementClass("Win32_Share");

                foreach (ManagementObject mo in mc.GetInstances())
                {
                    if (mo["Name"].ToString() == "EAImages")
                    {
                        outParams = mo.InvokeMethod("Delete", null, null);
                        if (Convert.ToInt32(outParams.Properties["ReturnValue"].Value) == 0)
                        {
                            EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Successfully removed existing EAImages share.");
                            return 0;
                        }

                        EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Could not remove EAImages share. Error code:" + outParams.Properties["ReturnValue"].Value);
                        return 1;
                    }
                }
                //EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Could not remove EAImages share - share EAImages does not exist or could not be found.");
                return 1;
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Could not remove EAImages share. Exception:" + ex.Message);
            }

            return 1;
        }

        public virtual int CreateFolderForShare(string strTargetPath)
        {
            DisableCurrentShare();

            DirectoryInfo di;
            try
            {
                if (!Directory.Exists(strTargetPath))
                {
                    Directory.CreateDirectory(strTargetPath);
                }

                di = new DirectoryInfo(strTargetPath);
                DirectorySecurity dSecurity = di.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                di.SetAccessControl(dSecurity);

                if (di != null)
                {
                    if (ShareFolder(strTargetPath, "EAImages", "Storage for EA image links") != 0)
                    {
                        EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Successfully created EAImages share.");
                        return 1;
                    }
                }

            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Error sharing folders. Please make sure you run as Administrator. ERROR: " + ex.Message);

                return 1;
            }

            return 0;

        }

        public virtual int ImportXMLFile(string path, string filename)
        {
            FilePath = System.IO.Path.GetFullPath(path);
            XMLFilePath = String.Format("{0}//{1}", FilePath, filename);

            return ImportXMLFile();

        }

        public virtual int ImportXMLFile()
        {
            if (!System.IO.File.Exists(XMLFilePath))
            {
                LogError(LogErrorLevel.A, "Could not load xml document. File " + XMLFilePath + " does not exist.");
                return 1;
            }

            string strTargetPath = System.IO.Path.Combine(GetRepositoryPath(), "EAImages");

            if (!CheckIfShareExists())
            {
                LogError(LogErrorLevel.A, "EAImage share does not exist. Please create a user data folder.");
                return 1;
            }

            m_SharePath = @"\\127.0.0.1\EAImages";

            m_Repository.BatchAppend = true;

            int iErrorCount = 0;
            m_strImportFilePath = FilePath;

            m_Packages = new ImportPackages();
            m_InfoModels = new ImportInfoModels();
            m_Actors = new ImportActors();
            m_ImportUseCase = new ImportUseCase();
            m_Domains = new ImportDomains();
            m_Requirements = new ImportRequirements();

            XDocument xdoc = null;
            XmlNamespaceManager namespaces = null;
            try
            {
                xdoc = XDocument.Load(XMLFilePath);
                if (xdoc == null)
                {
                    LogError(LogErrorLevel.A, "Error loading xml file");
                    iErrorCount++;
                }
                else
                {
                    namespaces = new XmlNamespaceManager(new NameTable());
                    namespaces.AddNamespace("UC", xdoc.Document.Root.GetNamespaceOfPrefix("UC").NamespaceName);
                }
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "could not load XML file : " + ex.Message);
                iErrorCount++;
            }

            ReadLocations(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/Locations", namespaces));

            if (AddIKBModel() != 0)
            {
                m_Repository.BatchAppend = false;
                return 1;
            }

            string sBaselineGUID = "";
            try
            {
                sBaselineGUID = SetupTempBaseline();
            }
            catch (EpriException ex)
            {
                handleException(ex);
                return iErrorCount + 1;
            }
            catch
            {
                LogError(LogErrorLevel.A, "Error creating baseline.");
                return iErrorCount + 1;
            }

            if (sBaselineGUID == "")
            {
                m_Repository.BatchAppend = false;
                return 1;
            }




            if (iErrorCount == 0)
            {
                try
                {
                    iErrorCount += InitIKBStructure();
                    // first process groups - we allow impoting group definitions independant of all other components
                    iErrorCount = iErrorCount + m_Domains.Import(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/DomainLibrary", namespaces));

                    // Actors can be imported indepenant of all other components
                    iErrorCount = iErrorCount + m_Actors.Import(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/ActorLibrary", namespaces));

                    // Information classes can be imported indepenant of all other components
                    iErrorCount = iErrorCount + m_InfoModels.Import(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/InformationModelLibrary", namespaces), m_Packages["UseCaseRepository/InformationModelLibrary"]);

                    // Import Use Case
                    // ??? TBD: should we handle importing multiple use cases?
                    iErrorCount = iErrorCount + m_ImportUseCase.ProcessUseCase(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/UseCaseLibrary/UseCase", namespaces));
                }
                catch (EpriException ex)
                {
                    handleException(ex);
                    //LogError(LogErrorLevel.A, "Error loading xml file");
                    iErrorCount++;
                }
                catch
                {
                    LogError(LogErrorLevel.A, "Error occured while importing.");
                    //LogError(LogErrorLevel.A, "Error loading xml file");
                    iErrorCount++;
                }
            }

            try
            {
                CleanupTempBaseline(sBaselineGUID, iErrorCount);
            }
            catch
            {
                LogError(LogErrorLevel.A, "Error removing temp baseline EPRiImport 1A. Please delete this baseline and make sure to check other baselines.");
            }

            m_Packages = null;
            m_InfoModels = null;
            m_Actors = null;
            m_ImportUseCase = null;
            m_Domains = null;
            m_Requirements = null;

            m_Repository.BatchAppend = false;

            return iErrorCount;

        }

        protected void handleException(EpriException ex)
        {
            if (ex != null)
            {
                StringBuilder sb = new StringBuilder();

                var sections = ex.location.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                sb.AppendLine("Error During Import Found In:");

                foreach (var section in sections)
                {
                    int id;
                    if (int.TryParse(section, out id))
                    {
                        if (Locations.ContainsKey(id))
                        {
                            sb.AppendLine("\t" + Locations[id]);
                        }
                    }
                }
                sb.AppendLine("Message: ");
                sb.AppendLine("\t" + ex.Message);

                LogError(LogErrorLevel.A, sb.ToString());
            }
        }

        protected void handleException(Exception ex)
        {
            if (ex != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ERROR During Import: {0}", ex.Message);

                LogError(LogErrorLevel.A, sb.ToString());
            }
        }

        protected void ReadLocations(XElement element)
        {
            try
            {
                if (Locations == null)
                    Locations = new Dictionary<int, string>();

                IEnumerable<XElement> elLoctions = element.Elements("Location");

                foreach (XElement elLoction in elLoctions)
                {
                    var elId = elLoction.Element("id");
                    var elName = elLoction.Element("name");
                    if (elId != null && elName != null)
                    {
                        int id;
                        if (int.TryParse(elId.Value, out id))
                        {
                            Locations.Add(id, elName.Value);
                        }
                    }
                    //location
                }
            }
            catch { }
        }
    }
}
