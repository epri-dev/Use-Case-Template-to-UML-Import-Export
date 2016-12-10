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
using System.Reflection;

namespace UCRepoClassLibrary
{
    public class EAPluginImporter : EAImporter
    {
        public string TempName { get; set; }

        public string XMLData { get; set; }

        #region Constructors
        public EAPluginImporter(LogMsgCallbackType myLogMsgCallback, LogMsgCallbackType myErrorMsgCallback)
            : base(myLogMsgCallback, myErrorMsgCallback)
        {
        }

        #endregion

        public int SetupRepo(EA.Repository Repository)
        {
            m_Repository = Repository;
            m_bCloseEAOnExit = false;
            m_bCloseEAFileOnExit = false;
            ParsePackagePath(Repository);

            try
            {
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
            }
            catch
            {
                LogError(LogErrorLevel.A, "Unable to locate Exchange Profile package.\r\nIf you have not loaded an Exchange Profile please use the ExchangeProfileXML.xml copied with sample files.\r\nIf you have already loaded an exchange profile, please reload your EAP file to apply these changes.");
                return 1;
            }

            try
            {
                m_oWord = new Word.Application();
                m_oWord.Visible = false;
            }
            catch
            {
                LogError(LogErrorLevel.A, "Microsoft Word does not appear to be installed. Please install the application and try again.");
                return 1;
            }
            return 0;
        }

        private void ParsePackagePath(EA.Repository Repository)
        {
            string packagepath = String.Empty;

            EA.Package package = Repository.GetTreeSelectedPackage();
            if (package == null)
            {
                return;
            }
            int parentPackageId = package.ParentID;

            if (package.Name.ToLower() == "usecaserepository")
            {
                return;
            }

            while (parentPackageId > 0)
            {
                var parent = Repository.GetPackageByID(parentPackageId);
                if (parent != null)
                {
                    packagepath = parent.Name + "/" + packagepath;
                    parentPackageId = parent.ParentID;
                }
                else
                    parentPackageId = 0;
            }

            //if (package.ParentID > 0)
            //{
            packagepath += package.Name + "/";
            //}

            ImportPackagePath = packagepath;
            //package.
        }

        public void SetupDataFolder(string path)
        {
            m_SharePath = path;
        }

        private string GetLocalFolder()
        {
            string strdirectory = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Volatile Environment").GetValue("APPDATA").ToString();
            strdirectory = String.Format("{0}\\EPRi", strdirectory);

            if (!Directory.Exists(strdirectory))
                Directory.CreateDirectory(strdirectory);

            return strdirectory;
        }

        private string GetTempFolder()
        {
            if (String.IsNullOrEmpty(TempName))
            {
                DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan timeSpan = DateTime.UtcNow - epochStart;
                TempName = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            }

            string strdirectory = String.Format("{0}\\temp\\{1}", GetLocalFolder(), TempName);

            if (!Directory.Exists(strdirectory))
                Directory.CreateDirectory(strdirectory);

            return strdirectory;
        }

        private void CleanupTempFolder()
        {
            try
            {
                var tempFolder = GetTempFolder();

                Directory.Delete(tempFolder, true);
            }
            catch { }
        }

        public override int ImportWordDoc(string documentPath)
        {
            if (!System.IO.File.Exists(documentPath))
            {
                LogError(LogErrorLevel.A, "Could not load word document. File " + documentPath + " does not exist.");
                return 1;
            }

            m_strImportFilePath = GetTempFolder();

            if (ConvertWordTemplateToXML(documentPath) == 0)
            {
                return (ImportXMLFile());
            }
            else
            {
                return 1;
            }
        }

        internal override int ConvertWordTemplateToXML(string documentPath)
        {
            string strWordToFilteredHTMLFileName = m_strImportFilePath + "\\document.html";
            string strTidied = m_strImportFilePath + "\\document_TIDY.xml";
            XMLFilePath = m_strImportFilePath + "\\document_MODEL.xml";

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
            using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("UCRepoClassLibrary.Translators.EUToIntermediate.xslt"))
            {
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    xslt.Load(reader);
                }
            }

            StringBuilder sb = new StringBuilder();

            // Create the writer.
            XmlWriterSettings XWriterSettings = new XmlWriterSettings();
            XWriterSettings.Indent = true;
            XWriterSettings.IndentChars = "\t";

            XmlWriter writer = XmlWriter.Create(sb, XWriterSettings);

            StringReader strXMLSource = new StringReader(parsed);

            try
            {
                XDocument xmlTree = XDocument.Load(strXMLSource);
                xslt.Transform(xmlTree.CreateReader(), writer);
            }
            catch
            {
                LogError(LogErrorLevel.A, "The use case document selected is not in the expected format. Please see the sample file located at " + m_SharePath + "\\IEC_UseCaseTemplateGOOD.docx for an example of the correct format.");
                return 1;
            }
            writer.Close();

            XMLData = sb.ToString();
            File.WriteAllText(XMLFilePath, XMLData);

            return 0;
        }

        public static bool CheckIfShareExists()
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
                //LogError(LogErrorLevel.A, "Could not find EAImages share. Exception:" + ex.Message);
            }

            return false;
        }

        public int ImportXMLFile(string xmlPath)
        {
            if (!File.Exists(xmlPath))
            {
                LogError(LogErrorLevel.A, "Could not load XML document. File " + xmlPath + " does not exist.");
                return 1;
            }

            try
            {
                XMLData = File.ReadAllText(xmlPath);
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not read XML file.");
                return 1;
            }

            return ImportXMLFile();
        }

        public override int ImportXMLFile()
        {
            //m_strImportFilePath = GetTempFolder();
            //m_SharePath = m_strImportFilePath;
            //m_SharePath = @"\\127.0.0.1\EAImages";
            m_Repository.BatchAppend = true;

            int iErrorCount = 0;

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
                StringReader strXMLSource = new StringReader(XMLData);
                xdoc = XDocument.Load(strXMLSource);
                if (xdoc == null)
                {
                    LogError(LogErrorLevel.A, "Error loading xml file");
                    return 1;
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
                return 1;
            }

            ReadLocations(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/Locations", namespaces));

            try
            {
                if (AddIKBModel() != 0)
                {
                    m_Repository.BatchAppend = false;
                    return 1;
                }
            }
            catch (EpriException ex)
            {
                handleException(ex);
                return 1;
            }
            catch
            {
                LogError(LogErrorLevel.A, "Error creating package (UseCaseRepository).");
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
                    iErrorCount += m_Domains.Import(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/DomainLibrary", namespaces));

                    // Actors can be imported indepenant of all other components
                    iErrorCount += m_Actors.Import(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/ActorLibrary", namespaces));

                    // Information classes can be imported indepenant of all other components
                    iErrorCount += m_InfoModels.Import(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/InformationModelLibrary", namespaces), m_Packages["UseCaseRepository/InformationModelLibrary"]);

                    // Import Use Case
                    // ??? TBD: should we handle importing multiple use cases?
                    iErrorCount += m_ImportUseCase.ProcessUseCase(xdoc.Root.XPathSelectElement("/UC:UseCaseRepository/UseCaseLibrary/UseCase", namespaces));
                }
                catch (EpriException ex)
                {
                    handleException(ex);
                    iErrorCount++;
                }
                catch
                {
                    LogError(LogErrorLevel.A, "Error occured while importing.");
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

            CleanupTempFolder();

            m_Packages = null;
            m_InfoModels = null;
            m_Actors = null;
            m_ImportUseCase = null;
            m_Domains = null;
            m_Requirements = null;

            m_Repository.BatchAppend = false;

            return iErrorCount;

        }
    }
}
