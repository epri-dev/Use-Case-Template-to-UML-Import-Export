using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace UCRepoClassLibrary
{
    public class EAPluginExporter : EAExporter
    {
        public EAPluginExporter(LogMsgCallbackType myLogMsgCallback, LogMsgCallbackType myErrorMsgCallback) :
            base(myLogMsgCallback, myErrorMsgCallback) { }

        public int SetupRepo(EA.Repository Repository)
        {
            m_Repository = Repository;
            m_bCloseEAOnExit = false;
            m_bCloseEAFileOnExit = false;

            try
            {
                m_iRefUseCaseClsID = m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Ref_UseCase").ElementID;
                m_ScenarioBaseClsID = m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Scenario").ElementID;
            }
            catch
            {
                LogError(LogErrorLevel.A, "Unable to locate the Exchange Profile package. You can find the package in ExchangeProfileXML.xml copied with sample files. Once added to this project please retry import.");
                return 1;
            }
            return 0;
        }

        public override int Export(string strUseCasePackage, string strFileName)
        {
            string outputFile = Path.GetFullPath(strFileName);
            string outputFilePath = Path.GetDirectoryName(outputFile) + @"\" + Path.GetFileNameWithoutExtension(outputFile);
            string strXMLDataFile = outputFilePath + "_MODEL.xml";
            m_strImageFileFullPath = outputFilePath + "_files";

            m_strImageFileRelPath = Path.GetFileNameWithoutExtension(outputFile) + "_files";


            try
            {
                if (!Directory.Exists(m_strImageFileFullPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(m_strImageFileFullPath);

                }
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not create directory for images: " + ex.Message);
                return 1;
            }

            m_Repository.Models.Refresh();

            try
            {
                m_Packages["UseCaseRepository"] = m_Repository.Models.GetByName("UseCaseRepository");
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not find model/package " + m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
                return 1;
            }

            if (m_Packages["UseCaseRepository"] == null)
            {
                LogError(LogErrorLevel.A, "Could not find model/package");
                return 1;
            }

            StringBuilder sb = new StringBuilder();
            try
            {

                string strUseCaseName = strUseCasePackage.Substring(strUseCasePackage.LastIndexOf("/") + 1);

                XElement XActorLibrary = GetActorLibraryForUseCase(GetPackage(strUseCasePackage + "/Computation"), GetPackage(strUseCasePackage + "/Enterprise"), strUseCaseName);

                XElement XDomainLibrary = GetDomainLibraryForUseCase(GetPackage(strUseCasePackage + "/Computation"), GetPackage(strUseCasePackage + "/Enterprise"));

                XElement XInformationModelLibrary = GetInfoExchangedForUseCase(GetPackage(strUseCasePackage + "/Computation"));

                if ((XActorLibrary == null) || (XDomainLibrary == null) || (XInformationModelLibrary == null))
                {
                    LogError(LogErrorLevel.A, "Could not find requisite package(s).");
                    return 1;
                }

                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

                sb.AppendLine("<UC:UseCaseRepository xsi:schemaLocation=\"http://www.TC8.org/IEC62559/UseCaseTemplate/V01 IEC62559UseCaseTemplate_V01.xsd\" xmlns:UC=\"http://www.TC8.org/IEC62559/UseCaseTemplate/V01\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

                sb.AppendLine(XActorLibrary.ToString());
                if (XDomainLibrary != null)
                {
                    sb.AppendLine(XDomainLibrary.ToString());
                }

                sb.AppendLine(XInformationModelLibrary.ToString());

                XElement XUseCaseLibrary = new XElement("UseCaseLibrary");
                XElement XUseCase = GetUseCase(strUseCasePackage);
                XUseCaseLibrary.Add(XUseCase);
                sb.AppendLine(XUseCaseLibrary.ToString());

                sb.AppendLine("</UC:UseCaseRepository>");

                //File.WriteAllText(strXMLDataFile, sb.ToString());
            }
            catch (EpriException ex)
            {
                LogError(LogErrorLevel.A, ex.Message);
                return 1;
            }

            //string strXMLOutput = System.IO.File.ReadAllText(strXMLDataFile);

            // transform the XML to "MODEL" xml file
            XslCompiledTransform xslt = new XslCompiledTransform();
            using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("UCRepoClassLibrary.Translators.IntermediateToXHTML.xslt"))
            {
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    xslt.Load(reader);
                }
            }

            // Create the writer.
            XmlWriterSettings XWriterSettings = new XmlWriterSettings();
            XWriterSettings.Indent = true;
            XWriterSettings.IndentChars = "\t";
            XWriterSettings.ConformanceLevel = ConformanceLevel.Auto;

            XmlWriter writer = XmlWriter.Create(outputFile, XWriterSettings);

            StringReader strXMLSource = new StringReader(sb.ToString());
            XDocument xmlTree = XDocument.Load(strXMLSource);

            xslt.Transform(xmlTree.CreateReader(), writer);

            writer.Close();

            return 0;
        }
    }
}
