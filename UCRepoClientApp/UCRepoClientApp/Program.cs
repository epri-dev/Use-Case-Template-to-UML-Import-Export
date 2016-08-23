/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: Program.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 7 $

	Date last modified: $Modtime: 11/05/12 11:28a $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp/Program.cs 7     11/05/12 11:31a Ronp $

$History: Program.cs $
 * 
 * *****************  Version 7  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * Update keyword expansion

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;


namespace UCRepoClientApp
{
    static class Program
    {
        static string _strUserResourcesFolder = "";
        static string _strBaseRsourcesFolder = "";

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                Application.Run(new MainForm(1));
                return;
            }


            AttachConsole(-1); // Attaching the current process to parent console.



            if (System.Diagnostics.Debugger.IsAttached)
            {
                //In Debugging mode  
                _strBaseRsourcesFolder = Path.GetFullPath(Application.StartupPath) + @"\..\..\..\..";
                _strUserResourcesFolder = _strBaseRsourcesFolder;

            }
            else
            {
                //In Published mode  
                _strBaseRsourcesFolder = Path.GetFullPath(Application.StartupPath) + @"\Resources";
                _strUserResourcesFolder = Properties.Settings.Default.ResourceFolder;
            }

            string strRepositoryFullPath = "";
            string strOutputFileName = "";
            string strUseCasePackage = "";
            string strPathToFile = "";
            string strFileName = "";


            int iArgCnt = 1;
            for (int i = 1; i < args.Length; i++)
            {
                iArgCnt++;


                if (args[i] == "")
                {
                    iArgCnt--;
                }
                else
                {

                    string[] strParams = args[i].Split('=');

                    if (strParams.Length != 2)
                    {
                        Console.WriteLine("Invalid command line arguments specified.");
                        dumpHelp();
                        return;
                    }

                    if ((strParams[0] == "") || (strParams[1] == ""))
                    {
                        Console.WriteLine("Invalid command line arguments specified.");
                        dumpHelp();
                        return;
                    }

                    if (strParams[0] == "repo")
                    {
                        strRepositoryFullPath = strParams[1];
                    }
                    else if (strParams[0] == "out")
                    {
                        strOutputFileName = strParams[1];
                    }
                    else if (strParams[0] == "ucase")
                    {
                        strUseCasePackage = strParams[1];
                    }
                    else if (strParams[0] == "inpath")
                    {
                        strPathToFile = strParams[1];
                    }
                    else if (strParams[0] == "infile")
                    {
                        strFileName = strParams[1];
                    }
                }
            }

            if (args[0] == "export")
            {
                if (iArgCnt != 4)
                {
                    Console.WriteLine("Invalid command line arguments specified.");
                    dumpHelp();
                    return;
                }

                Console.Write("Processing started..");
                Export(strRepositoryFullPath, _strBaseRsourcesFolder + @"\Translators\IntermediateToXHTML.xslt", strOutputFileName, strUseCasePackage); 
            }
            else if (args[0] == "importword")
            {
                if (iArgCnt != 4)
                {
                    Console.WriteLine("Invalid command line arguments specified.");
                    dumpHelp();
                    return;
                }

                Console.Write("Processing started..");
                ImportWordDoc(strRepositoryFullPath, strPathToFile, strFileName, _strBaseRsourcesFolder + @"\Translators\EUToIntermediate.xslt");
            }
            else if (args[0] == "importxml")
            {
                if (iArgCnt != 3)
                {
                    Console.WriteLine("Invalid command line arguments specified.");
                    dumpHelp();
                    return;
                }


                Console.Write("Processing started..");
                ImportXML(strRepositoryFullPath, strPathToFile, strFileName);
            }
            else
            {
                Console.WriteLine("Invalid command line arguments specified.");
                dumpHelp();
                return;
            }
        }

        static public void FormLogMsg(string strType, string strMsg)
        {
            //Console.WriteLine(strType + " : " + strMsg);
            Console.Write(".");
        }

        static public void FormLogError(string strType, string strMsg)
        {
            Console.WriteLine(" ");
            Console.WriteLine("Error: " + strType + " : " + strMsg);
        }

        static internal void Export(string strRepositoryFullPath, string strXSLTFullPath, string strOutputFileName, string strUseCasePackage)
        {
            UCRepoClassLibrary.EAExporter theEAExporter;

            UCRepoClassLibrary.EAExporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAExporter.LogMsgCallbackType(Program.FormLogMsg);
            UCRepoClassLibrary.EAExporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAExporter.LogMsgCallbackType(Program.FormLogError);

            theEAExporter = new UCRepoClassLibrary.EAExporter(myLogMsgCallback, myErrorMsgCallback);

            Console.WriteLine(".");

            if (theEAExporter.Open(strRepositoryFullPath) != 0)
            {
                Console.WriteLine("Export failed.");
                return;
            }

            int iErrorCount = theEAExporter.Export(strUseCasePackage, strXSLTFullPath, strOutputFileName);

            if (iErrorCount == 0)
            {
                Console.WriteLine("Export completed successfully.");
            }
            else
            {
                Console.WriteLine("Export failed.");
            }

            theEAExporter.Close();
        }

        static internal void ImportWordDoc(string strRepositoryFullPath, string strPathToFile, string strFileName, string strXSLTFullPath)
        {
            UCRepoClassLibrary.EAImporter theEAImporter;

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(Program.FormLogMsg);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(Program.FormLogError);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);

            if (theEAImporter.Open(strRepositoryFullPath) != 0)
            {
                Console.WriteLine(".");
                Console.WriteLine("Import failed.");
                return;
            }

            int iErrorCount = theEAImporter.ImportWordDoc(strPathToFile, strFileName, strXSLTFullPath);
            Console.WriteLine(".");

            if (iErrorCount == 0)
            {
                Console.WriteLine("Import completed successfully");
            }
            else
            {
                Console.WriteLine("Import ABORTED. Errors found. Error count=" + iErrorCount.ToString());
            }

            theEAImporter.Close();
        }

        static internal void ImportXML(string strRepositoryFullPath, string strPathToFile, string strFileName)
        {
            UCRepoClassLibrary.EAImporter theEAImporter;

            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myLogMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(Program.FormLogMsg);
            UCRepoClassLibrary.EAImporter.LogMsgCallbackType myErrorMsgCallback = new UCRepoClassLibrary.EAImporter.LogMsgCallbackType(Program.FormLogError);

            theEAImporter = new UCRepoClassLibrary.EAImporter(myLogMsgCallback, myErrorMsgCallback);

            if (theEAImporter.Open(strRepositoryFullPath) != 0)
            {
                Console.WriteLine(".");
                Console.WriteLine("Import failed.");
                return;
            }

            int iErrorCount = theEAImporter.ImportXMLFile(strPathToFile, strFileName);
            Console.WriteLine(".");

            if (iErrorCount == 0)
            {
                Console.WriteLine("Import completed successfully");
            }
            else
            {
                Console.WriteLine("Import ABORTED. Errors found. Error count=" + iErrorCount.ToString());
            }

            theEAImporter.Close();
        }

        static internal void dumpHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Command line usage:");
            Console.WriteLine("");
            Console.WriteLine("For export:");
            Console.WriteLine("");
            Console.WriteLine("UCRepoClientApp export repo=\"path/To/filename.eap\" out=\"path/to/filename.html\" ucase=\"/UseCaseRepository/UseCaseLibrary/ImportedUseCase/name of use case\"");
            Console.WriteLine("");
            Console.WriteLine("For import:");
            Console.WriteLine("");
            Console.WriteLine("UCRepoClientApp importword repo=\"path/To/filename.eap\" inpath=\"path/to/word/doc\" infile=\"filename.docx\"");
            Console.WriteLine("");
            Console.WriteLine("UCRepoClientApp importxml repo=\"path/To/filename.eap\" inpath=\"path/to/word/doc\" infile=\"filename.docx\"");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("UCRepoClientApp export repo=\"E:\\DriveE\\_project\\EPRI\\C18322-EPRIUseCaseRepository\\Source\\EAResources\\UCRepo.eap\" out=\"E:\\DriveE\\_project\\EPRI\\C18322-EPRIUseCaseRepository\\Source\\FilledInTemplates\\aout.html\" ucase=\"/UseCaseRepository/UseCaseLibrary/ImportedUseCase/Distribution/D-11.1 Power Quality Contracts\"");
            Console.WriteLine("");
            Console.WriteLine("UCRepoClientApp importword repo=\"E:\\DriveE\\_project\\EPRI\\C18322-EPRIUseCaseRepository\\Source\\EAResources\\UCRepo.eap\" inpath=\"E:\\DriveE\\_project\\EPRI\\C18322-EPRIUseCaseRepository\\Source\\FilledInTemplates\" infile=\"IEC_UseCaseTemplateGOOD.docx\"");
            Console.WriteLine("");
            Console.WriteLine("UCRepoClientApp importxml repo=\"E:\\DriveE\\_project\\EPRI\\C18322-EPRIUseCaseRepository\\Source\\EAResources\\UCRepo.eap\" inpath=\"E:\\DriveE\\_project\\EPRI\\C18322-EPRIUseCaseRepository\\Source\\FilledInTemplates\" infile=\"IEC_UseCaseTemplateGOOD_MODEL.xml\"");
            Console.WriteLine("");
        }
    }
}


