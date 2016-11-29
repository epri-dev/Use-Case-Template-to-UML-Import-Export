/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: EAExporter.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 30 $

	Date last modified: $Modtime: 11/28/12 10:30a $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/EAExporter.cs 30    12/10/12 3:00p Ronp $

$History: EAExporter.cs $
 * 
 * *****************  Version 30  *****************
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
 * *****************  Version 29  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
 * Update keyword expansion

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;

using System.Text.RegularExpressions;

using System.Drawing;

namespace UCRepoClassLibrary
{
    public class EAExporter
    {
        public delegate void LogMsgCallbackType(string strType, string strMsg);

        #region Properties
        //public static EA.Repository m_Repository;
        protected EA.Repository m_Repository;

        protected Hashtable m_Packages = new Hashtable();

        protected Boolean m_bCloseEAFileOnExit = false;
        protected Boolean m_bCloseEAOnExit = false;

        protected static LogMsgCallbackType m_LogMsgCallback = null;
        protected static LogMsgCallbackType m_ErrorMsgCallback = null;

        internal static int m_iRefUseCaseClsID = 0;
        internal static int m_ScenarioBaseClsID = 0;

        internal static string m_strImageFileFullPath = null;
        internal static string m_strImageFileRelPath = null;

        internal string XSLTPath { get; set; }
        #endregion

        #region Constructors
        public EAExporter(LogMsgCallbackType myLogMsgCallback, LogMsgCallbackType myErrorMsgCallback)
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

        /// <summary>
        /// Open
        /// </summary>
        public int Open(string strRepository)
        {
            if (strRepository == "")
            {
                LogError(LogErrorLevel.A, "Could not load repository. File name must be supplied.");
                return 1;
            }

            strRepository = System.IO.Path.GetFullPath(strRepository);

            if (!System.IO.File.Exists(strRepository))
            {
                LogError(LogErrorLevel.A, "Could not load repository. File " + strRepository + " does not exist.");
                return 1;
            }

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

                LogMsg(LogMsgType.Info, "EA not running : " + ex.Message);
            }

            if (bEANotRunning)
            {
                // EA not running... start it up
                LogMsg(LogMsgType.Info, "Could not connect to running to EA ... start new instance");

                m_Repository = new EA.Repository();

                LogMsg(LogMsgType.Info, "Connected to EA");

                m_bCloseEAOnExit = true;

            }

            if (m_Repository.ConnectionString == strRepository)
            {
                LogMsg(LogMsgType.Info, "model already loaded.");
                m_bCloseEAFileOnExit = false;
            }
            else
            {
                LogMsg(LogMsgType.Info, "Opening model");
                try
                {
                    m_Repository.OpenFile(strRepository);
                }
                catch (Exception ex)
                {
                    LogMsg(LogMsgType.MiscExceptions, "Could not load repository. Error:" + ex.Message);
                    return 1;
                }


                m_bCloseEAFileOnExit = true;
                LogMsg(LogMsgType.Info, "Opened model");
            }

            m_iRefUseCaseClsID = m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Ref_UseCase").ElementID;
            m_ScenarioBaseClsID = m_Repository.Models.GetByName("ExchangeProfile").Packages.GetByName("ExchangedProfile").Elements.GetByName("Scenario").ElementID;


            return 0;
        }

        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            // close the repository and tidy up
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

        protected EA.Package GetPackage(string strPackageFullPathName)
        {
            EA.Package ePackage = null;

            LogMsg(LogMsgType.Info, "Lookup package: " + strPackageFullPathName);

            string[] strPackageNames = strPackageFullPathName.Split('/');

            string strBldFullPackageName = "";

            foreach (string strPackage in strPackageNames)
            {
                if (strPackage != "")
                {
                    if (m_Packages[strBldFullPackageName + strPackage] != null)
                    {
                        ePackage = (EA.Package)m_Packages[strBldFullPackageName + strPackage];
                        strBldFullPackageName = strBldFullPackageName + strPackage + "/";
                    }
                    else
                    {
                        string strParentPackageName = strBldFullPackageName.Substring(0, strBldFullPackageName.Length - 1);

                        try
                        {
                            ePackage = ((EA.Package)m_Packages[strParentPackageName]).Packages.GetByName(strPackage);
                        }
                        catch (Exception ex)
                        {
                            throw new EpriException("Lookup package failed: " + strPackageFullPathName + strPackage + "error: " + ex.Message, "");
                            LogError(LogErrorLevel.A, "Lookup package failed: " + strPackageFullPathName + strPackage + "error: " + ex.Message);
                            return null;
                        }

                        if (ePackage != null)
                        {
                            m_Packages[strBldFullPackageName + strPackage] = ePackage;
                            strBldFullPackageName = strBldFullPackageName + strPackage + "/";
                        }
                        else
                        {
                            throw new EpriException("Lookup package failed: " + strPackageFullPathName + strPackage, "");
                            LogError(LogErrorLevel.A, "Lookup package failed: " + strPackageFullPathName + strPackage);
                            return null;
                        }
                    }
                }
            }

            LogMsg(LogMsgType.Info, "Lookup package found: " + strPackageFullPathName);

            return ePackage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strActorName"></param>
        /// <returns></returns>
        private EA.Element GetActor(string strActorName)
        {
            LogMsg(LogMsgType.Info, "Lookup Actor: " + strActorName);

            EA.Collection eColElements = m_Repository.GetElementsByQuery("Simple", strActorName);

            foreach (EA.Element elAnElement in eColElements)
            {
                if ((elAnElement.Type == "Actor") && (elAnElement.Name == strActorName))
                {
                    LogMsg(LogMsgType.Info, "found Actor: " + strActorName);
                    return elAnElement;
                }
            }

            throw new EpriException("could not find Actor: " + strActorName, "");
            LogError(LogErrorLevel.A, "could not find Actor: " + strActorName);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfoClassName"></param>
        /// <returns></returns>
        private EA.Element GetInformationClass(string strInfoClassName)
        {
            LogMsg(LogMsgType.Info, "Lookup InformationClass: " + strInfoClassName);

            EA.Collection eColElements = m_Repository.GetElementsByQuery("Simple", strInfoClassName);

            foreach (EA.Element elAnElement in eColElements)
            {
                if ((elAnElement.Type == "Class") && (elAnElement.Name == strInfoClassName))
                {
                    LogMsg(LogMsgType.Info, "found InfoClass: " + strInfoClassName);
                    return elAnElement;
                }
            }

            throw new EpriException("could not find Information Class: " + strInfoClassName, "");
            LogError(LogErrorLevel.A, "could not find Infor Class: " + strInfoClassName);
            return null;
        }


        private EA.Collection FindActorLinkInUCDiagram(EA.Package eaPackageEnterprise, EA.Element eaActor, string strUseCaseName)
        {
            EA.Element eaUseCaseEl1 = eaPackageEnterprise.Elements.GetByName(strUseCaseName);

            foreach (EA.Connector eaConnector in eaUseCaseEl1.Connectors)
            {
                if (eaConnector.SupplierID == eaActor.ElementID)
                {
                    LogMsg(LogMsgType.Info, "found UC link to actor: " + eaActor.Name);

                    return eaConnector.TaggedValues;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eaPackageComputation"></param>
        /// <returns></returns>
        protected XElement GetDomainLibraryForUseCase(EA.Package eaPackageComputation, EA.Package eaPackageEnterprise)
        {
            if (eaPackageComputation == null)
            {
                LogError(LogErrorLevel.A, "Could find Computation package");
                return null;
            }

            if (eaPackageEnterprise == null)
            {
                LogError(LogErrorLevel.A, "Could find Enterprise package");
                return null;
            }

            XElement Xel_Groups = new XElement("DomainLibrary");

            foreach (EA.Package eaScenario in eaPackageComputation.Packages)
            {
                EA.Package eaPackageSequence = eaScenario.Packages.GetByName("Sequence");

                foreach (EA.Element eaElement in eaPackageSequence.Elements)
                {
                    // each element is an object or instance of an actor: go look up the actor:
                    if (eaElement.Type == "Object")
                    {
                        EA.Element eaActor = GetActor(eaElement.Name);
                        if (eaActor != null)
                        {
                            // now find package group the actor is in
                            EA.Package eaActorParentPackage = m_Repository.GetPackageByID(eaActor.PackageID);

                            if (eaActorParentPackage != null)
                            {
                                string strDomainName = eaActorParentPackage.Name;
                                // find domain package

                                EA.Package eaPackageDomain = GetPackage("UseCaseRepository/DomainLibrary/" + strDomainName);

                                if (eaPackageDomain != null)
                                {
                                    XElement XelExistingGroup = Xel_Groups.XPathSelectElement("Domain[name='" + eaActorParentPackage.Name + "']");

                                    if (XelExistingGroup == null)
                                    {
                                        // does not exist - create it
                                        XElement Xel_Group = new XElement("Domain");
                                        string strTemp = eaPackageDomain.Notes;
                                        int iRes = EAExporter.CheckForAndReplaceLinks(ref strTemp);

                                        Xel_Group.Add(new XElement("description", strTemp));

                                        strTemp = eaPackageDomain.Name;
                                        iRes = EAExporter.CheckForAndReplaceLinks(ref strTemp);
                                        Xel_Group.Add(new XElement("name", strTemp));
                                        Xel_Groups.Add(Xel_Group);

                                    }
                                    else
                                    {
                                        // Group already exists
                                    }
                                }
                                else
                                {
                                    throw new EpriException("Could not find domain library package for domain: " + strDomainName, "");
                                    LogError(LogErrorLevel.A, "Could not find domain library package for domain: " + strDomainName);
                                    return null;
                                }
                            }
                        }
                    }
                }
            }

            return Xel_Groups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eaPackageComputation"></param>
        /// <returns></returns>
        protected XElement GetActorLibraryForUseCase(EA.Package eaPackageComputation, EA.Package eaPackageEnterprise, string strUseCaseName)
        {
            if (eaPackageComputation == null)
            {
                throw new EpriException("Could not find Computation package.", "");
                LogError(LogErrorLevel.A, "Could find Computation package");
                return null;
            }

            if (eaPackageEnterprise == null)
            {
                throw new EpriException("Could not find Enterprise package.", "");
                LogError(LogErrorLevel.A, "Could find Enterprise package");
                return null;
            }

            XElement Xel_Groups = new XElement("ActorLibrary");

            foreach (EA.Package eaScenario in eaPackageComputation.Packages)
            {
                EA.Package eaPackageSequence = eaScenario.Packages.GetByName("Sequence");

                foreach (EA.Element eaElement in eaPackageSequence.Elements)
                {
                    // each element is an object or instance of an actor: go look up the actor:
                    if (eaElement.Type == "Object")
                    {
                        EA.Element eaActor = GetActor(eaElement.Name);
                        if (eaActor != null)
                        {
                            // now find package group the actor is in
                            EA.Package eaActorParentPackage = m_Repository.GetPackageByID(eaActor.PackageID);
                            if (eaActorParentPackage != null)
                            {
                                EA.Collection eaCollection = FindActorLinkInUCDiagram(eaPackageEnterprise, eaActor, strUseCaseName);

                                XElement Xel_Actor = new XElement("Actor");
                                Xel_Actor.Add(new XElement("description", eaActor.Notes));

                                if (eaCollection != null)
                                {
                                    foreach (EA.ConnectorTag eaTgVal in eaCollection)
                                    {
                                        if (eaTgVal.Name == "furtherInformation")
                                        {
                                            Xel_Actor.Add(new XElement("furtherInformation", eaTgVal.Value));
                                            break;
                                        }
                                    }
                                }

                                Xel_Actor.Add(new XElement("name", eaActor.Name));
                                Xel_Actor.Add(new XElement("type", eaActor.Stereotype));

                                XElement XelExistingGroup = Xel_Groups.XPathSelectElement("Domain[name='" + eaActorParentPackage.Name + "']");

                                if (XelExistingGroup == null)
                                {
                                    // does not exist - create it
                                    XElement Xel_Group = new XElement("Domain");
                                    Xel_Group.Add(new XElement("name", eaActorParentPackage.Name));
                                    Xel_Groups.Add(Xel_Group);

                                    Xel_Group.Add(Xel_Actor);
                                }
                                else
                                {
                                    // Group already exists
                                    //See if we already have this actor in this group
                                    if (XelExistingGroup.XPathSelectElement("Actor[name='" + eaActor.Name + "']") == null)
                                    {
                                        XelExistingGroup.Add(Xel_Actor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Xel_Groups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eaPackageComputation"></param>
        /// <returns></returns>
        protected XElement GetInfoExchangedForUseCase(EA.Package eaPackageComputation)
        {
            if (eaPackageComputation == null)
            {
                throw new EpriException("Could not find Computation package.", "");
                LogError(LogErrorLevel.A, "Could find Computation package");
                return null;
            }

            XElement Xel_InfoClasses = new XElement("InformationModelLibrary");

            foreach (EA.Package eaScenario in eaPackageComputation.Packages)
            {
                EA.Package eaPackageSequence = eaScenario.Packages.GetByName("Sequence");

                foreach (EA.Element eaElement in eaPackageSequence.Elements)
                {
                    foreach (EA.Connector eaConnector in eaElement.Connectors)
                    {
                        string strInformationClassName = eaConnector.Name;
                        strInformationClassName = strInformationClassName.Replace("CREATE", "");
                        strInformationClassName = strInformationClassName.Replace("READ", "");
                        strInformationClassName = strInformationClassName.Replace("UPDATE", "");
                        strInformationClassName = strInformationClassName.Replace("DELETE", "");
                        strInformationClassName = strInformationClassName.Replace("REPORT", "");
                        strInformationClassName = strInformationClassName.Replace("TIMER", "");
                        strInformationClassName = strInformationClassName.Replace("REPEAT", "");
                        strInformationClassName = strInformationClassName.Replace("REPLY", "");
                        strInformationClassName = strInformationClassName.Replace("EXECUTE", "");
                        strInformationClassName = strInformationClassName.Replace("GET", "");
                        strInformationClassName = strInformationClassName.Replace("(", "");
                        strInformationClassName = strInformationClassName.Replace(")", "");

                        EA.Element eaInfoClass = GetInformationClass(strInformationClassName);

                        if (eaInfoClass != null)
                        {
                            // see if we already emitted this info class
                            if (Xel_InfoClasses.XPathSelectElement("InformationModel[name='" + strInformationClassName + "']") == null)
                            {
                                XElement XelInfoClass = new XElement("InformationModel");

                                string strTemp = eaInfoClass.Notes;
                                int iRes = EAExporter.CheckForAndReplaceLinks(ref strTemp);
                                XelInfoClass.Add(new XElement("description", strTemp));

                                strTemp = strInformationClassName;
                                iRes = EAExporter.CheckForAndReplaceLinks(ref strTemp);
                                XelInfoClass.Add(new XElement("name", strTemp));

                                string strRequirements = eaInfoClass.TaggedValues.GetByName("requirements").Value();
                                if (strRequirements != "")
                                {
                                    string[] strRequirement = strRequirements.Split(',');

                                    foreach (string strReq in strRequirement)
                                    {
                                        XelInfoClass.Add(new XElement("referencedRequirement", new XElement("id", strReq)));
                                    }
                                }

                                Xel_InfoClasses.Add(XelInfoClass);
                            }
                        }
                    }
                }
            }

            return Xel_InfoClasses;
        }

        class InteractionFragmentInfo
        {
            public EA.Element m_eaElement = null;
            public EA.DiagramObject m_eaDiagramObject = null;
            public int m_iLoopStart = 1000;
            public int m_iLoopEnd = -1;
        }

        private class ScenarioSortHelper : IComparable<ScenarioSortHelper>
        {
            public string _strNoForSort;
            public EA.Package _eaScenario;

            public ScenarioSortHelper(string strSecNumber, EA.Package eaScenario)
            {
                string strNoForSort = "";

                string[] strSectionPeices = strSecNumber.Split('.');

                for (int i = 0; i < strSectionPeices.Length; i++)
                {
                    try
                    {
                        int iVal = Convert.ToInt16(strSectionPeices[i]);

                        strNoForSort = strNoForSort + "." + iVal.ToString("0000");
                    }
                    catch (FormatException e)
                    {
                        throw e;
                    }
                }

                _strNoForSort = strNoForSort;
                _eaScenario = eaScenario;
            }

            public int CompareTo(ScenarioSortHelper obj)
            {
                return _strNoForSort.CompareTo(obj._strNoForSort);
            }
        }

        private XElement GetScenariosForUseCase(EA.Package eaPackageComputation)
        {
            XElement XScenarios = new XElement("Scenarios");

            List<ScenarioSortHelper> myObjectList = new List<ScenarioSortHelper>();

            foreach (EA.Package eaScenario in eaPackageComputation.Packages)
            {
                XElement XScenario = new XElement("Scenario");

                int iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_Scenario(m_ScenarioBaseClsID), null, eaScenario, XScenario);

                try
                {
                    myObjectList.Add(new ScenarioSortHelper(XScenario.Element("number").Value, eaScenario));
                }
                catch (FormatException ex)
                {
                    throw new EpriException("Bad scenario number.", "");
                    //LogError(LogErrorLevel.A, "Bad scenario number : " + ex.Message);
                    return null;
                }
            }

            myObjectList.Sort();

            foreach (ScenarioSortHelper obj in myObjectList)
            {
                EA.Package eaScenario = obj._eaScenario;

                EA.Package eaPackageActivity = eaScenario.Packages.GetByName("Activity");

                XElement XScenario = new XElement("Scenario");

                int iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_Scenario(m_ScenarioBaseClsID), null, eaScenario, XScenario);
                iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_PrimaryActor(), "PrimaryActor", eaScenario, XScenario);


                EA.Element eaElActivityInitial = eaPackageActivity.Elements.GetByName("ActivityInitial");

                EA.Connector eaConn = eaElActivityInitial.Connectors.GetAt(0);

                EA.Element eaActivityCurrent = m_Repository.GetElementByID(eaConn.SupplierID);

                bool bDoLoop = true;

                while (bDoLoop)
                {
                    XElement XStep = new XElement("Step");

                    XStep.Add(new XElement("description", eaActivityCurrent.Notes));

                    XStep.Add(new XElement("event", eaActivityCurrent.TaggedValues.GetByName("event").Value()));

                    if (eaActivityCurrent.Name.StartsWith("Wait till:"))
                    {
                        string strTemp = eaActivityCurrent.Name.Substring(eaActivityCurrent.Name.IndexOf("name=") + 5);
                        XStep.Add(new XElement("name", strTemp));
                    }
                    else if (eaActivityCurrent.Name.StartsWith("Repeat till:"))
                    {

                    }
                    else
                    {
                        XStep.Add(new XElement("name", eaActivityCurrent.Name));
                    }

                    XStep.Add(new XElement("number", eaActivityCurrent.TaggedValues.GetByName("number").Value()));


                    XElement XInformationModel = null;
                    EA.Element eaObjectFlow = null;
                    EA.Element eaNextActivity1 = null;
                    EA.Element eaNextActivity2 = null;
                    EA.Connector eaConnOperation = null;
                    string strProducer = null;
                    string strReceiver = null;

                    foreach (EA.Connector eaConnector in eaActivityCurrent.Connectors)
                    {
                        if (eaConnector.Type == "ControlFlow")
                        {
                            // check that this is not connector back to previous activity
                            if (eaConnector.SupplierID != eaActivityCurrent.ElementID)
                            {
                                eaNextActivity1 = m_Repository.GetElementByID(eaConnector.SupplierID);
                            }
                        }

                        if (eaConnector.Type == "ObjectFlow")
                        {

                            if (eaConnector.SupplierID == eaActivityCurrent.ElementID)
                            {
                                // this is object flow to self- must be read

                                eaObjectFlow = m_Repository.GetElementByID(eaConnector.ClientID);
                                strProducer = eaObjectFlow.TaggedValues.GetByName("ActorName").Value;
                                strReceiver = m_Repository.GetElementByID(eaConnector.SupplierID).TaggedValues.GetByName("ActorName").Value;
                                eaConnOperation = eaConnector;
                            }
                            else
                            {
                                eaObjectFlow = m_Repository.GetElementByID(eaConnector.SupplierID);
                                eaConnOperation = eaConnector;

                                strReceiver = eaObjectFlow.TaggedValues.GetByName("ActorName").Value;
                                strProducer = m_Repository.GetElementByID(eaConnector.ClientID).TaggedValues.GetByName("ActorName").Value;
                            }
                        }
                    }

                    if (eaNextActivity1 == null)
                    {
                        foreach (EA.Connector eaConnector in eaObjectFlow.Connectors)
                        {
                            if (eaConnector.Type == "ControlFlow")
                            {
                                // check that this is not connector back to previous activity
                                if (eaConnector.SupplierID != eaActivityCurrent.ElementID)
                                {
                                    eaNextActivity2 = m_Repository.GetElementByID(eaConnector.SupplierID);
                                }
                            }

                            if (eaConnector.Type == "ObjectFlow")
                            {
                                // check that this is not connector back to previous activity
                                if (eaConnector.SupplierID != eaActivityCurrent.ElementID)
                                {
                                    eaNextActivity2 = m_Repository.GetElementByID(eaConnector.SupplierID);
                                }
                            }
                        }

                        eaNextActivity1 = eaNextActivity2;
                    }

                    if (eaConnOperation != null)
                    {
                        string strOperation = eaConnOperation.Name;
                        strOperation = strOperation.Remove(strOperation.IndexOf("("));
                        XStep.Add(new XElement("service", strOperation));
                    }

                    if (eaActivityCurrent.Name.StartsWith("Wait till:"))
                    {
                        XStep.Add(new XElement("service", "TIMER"));

                        XStep.Add(new XElement("InformationReceiver", new XElement("name", eaActivityCurrent.TaggedValues.GetByName("ActorName").Value)));
                        XStep.Add(new XElement("InformationProducer", new XElement("name", eaActivityCurrent.TaggedValues.GetByName("ActorName").Value)));

                        string strTemp = eaActivityCurrent.Name.Substring("Wait till: ".Length, eaActivityCurrent.Name.IndexOf("name=") - "Wait till: ".Length - 1);

                        XInformationModel = new XElement("InformationModel");
                        XInformationModel.Add(new XElement("name", strTemp));
                        XStep.Add(XInformationModel);
                    }

                    if (eaActivityCurrent.Name.StartsWith("Repeat till:"))
                    {

                        string strService = "REPEAT";
                        int iFirstStep = 1000;
                        int iLastStep = 0;
                        int iNumber = 0;

                        foreach (EA.Connector eaConnector in eaActivityCurrent.Connectors)
                        {
                            if (eaConnector.Type == "ControlFlow")
                            {
                                // check that this is not connector back to previous activity
                                if (eaConnector.SupplierID != eaActivityCurrent.ElementID)
                                {
                                    string strNumber = m_Repository.GetElementByID(eaConnector.SupplierID).TaggedValues.GetByName("number").Value;

                                    iNumber = Convert.ToInt32(strNumber);

                                    if (iNumber < iFirstStep)
                                    {
                                        iFirstStep = iNumber;
                                    }
                                }
                                else
                                {
                                    string strNumber = m_Repository.GetElementByID(eaConnector.ClientID).TaggedValues.GetByName("number").Value;

                                    iNumber = Convert.ToInt32(strNumber);

                                    iLastStep = iNumber;
                                }
                            }
                        }

                        strService = strService + "(" + iFirstStep + "-" + iLastStep + ")";

                        XStep.Add(new XElement("service", strService));
                    }

                    if (strReceiver != null)
                    {
                        XStep.Add(new XElement("InformationReceiver", new XElement("name", strReceiver)));
                    }

                    if (strProducer != null)
                    {
                        XStep.Add(new XElement("InformationProducer", new XElement("name", strProducer)));
                    }

                    if (eaObjectFlow != null)
                    {

                        XInformationModel = new XElement("InformationModel");
                        XInformationModel.Add(new XElement("name", eaObjectFlow.Name));
                        XStep.Add(XInformationModel);
                    }


                    //XElement XRequirement = new XElement("Requirement");
                    //XRequirement.Add(new XElement("id", eaActivityCurrent.TaggedValues.GetByName("requirements").Value()));
                    //XStep.Add(XRequirement);

                    string strRequirements = eaActivityCurrent.TaggedValues.GetByName("requirements").Value();
                    if (strRequirements != "")
                    {
                        string[] strRequirement = strRequirements.Split(',');

                        foreach (string strReq in strRequirement)
                        {
                            XStep.Add(new XElement("Requirement", new XElement("id", strReq)));
                        }
                    }


                    XScenario.Add(XStep);

                    eaActivityCurrent = eaNextActivity1;

                    if (eaActivityCurrent.Name == "ActivityFinal")
                    {
                        bDoLoop = false;
                    }
                }

                XScenarios.Add(XScenario);
            }

            return XScenarios;
        }


        static internal Hashtable ParseRunStateString(string strRunState)
        {
            Hashtable hTaggedValues = new Hashtable();

            string pattern1 = @"\@VAR;Variable=.*?;Value=.*?;Op==.*?;\@ENDVAR;";
            string patternName = @"(?<=Variable=).*?(?=\;)";
            string patternValue = @"(?<=Value=).*?(?=\;)";

            Regex rgx1 = new Regex(pattern1);
            MatchCollection matches1 = rgx1.Matches(strRunState);

            foreach (Match match1 in matches1)
            {
                Regex rgxName = new Regex(patternName);
                MatchCollection matchesName = rgxName.Matches(match1.Value);
                Regex rgxValue = new Regex(patternValue);
                MatchCollection matchesValue = rgxValue.Matches(match1.Value);

                hTaggedValues[matchesName[0].Value] = matchesValue[0].Value;

            }

            return hTaggedValues;
        }


        internal static int CheckForAndReplaceLinks(ref string strValue)
        {
            while (strValue.Contains("<a href=\""))
            {
                int iStart = strValue.IndexOf("<a href=\"");
                int iPosEndInfo = strValue.IndexOf("</u></font></a>");
                int iLen = iPosEndInfo - iStart + "</u></font></a>".Length;

                string strLink = strValue.Substring(iStart, iLen);

                string strOldFullPath = strLink.Substring("<a href=\"".Length, strLink.IndexOf("\">") - "<a href=\"".Length);
                string strFileName = Path.GetFileName(strOldFullPath);
                string strOldPath = strOldFullPath.Replace(strFileName, "");


                string strTemp = strFileName;
                string strWidth = strTemp.Substring(strTemp.IndexOf("W") + 1, strTemp.IndexOf("_") - 1);
                strTemp = strTemp.Remove(0, strWidth.Length + 2);
                string strHeight = strTemp.Substring(strTemp.IndexOf("H") + 1, strTemp.IndexOf("_") - 1);
                strTemp = strTemp.Remove(0, strHeight.Length + 2);
                string strId = strTemp.Substring(strTemp.IndexOf("ID") + 2, strTemp.IndexOf("_") - 2);
                strTemp = strTemp.Remove(0, strId.Length + 3);

                string strAlign = "";

                if ((strTemp.IndexOf("AL") + 2) == (strTemp.IndexOf("_") - 2))
                {

                }
                else
                {
                    strAlign = strTemp.Substring(strTemp.IndexOf("AL") + 2, strTemp.IndexOf("_") - 2);
                }

                strTemp = strTemp.Remove(0, strAlign.Length + 3);

                string strStyle = "";

                //if (strTemp.IndexOf("?") < 0)
                //{
                //}
                //else
                //{
                //strStyle = strTemp.Substring(strTemp.IndexOf("?"));
                //strTemp = strTemp.Remove(strTemp.IndexOf("?"));

                ////strStyle = strTemp.Substring(strTemp.IndexOf("ST") + 2, strTemp.IndexOf("_") - 2);
                //}

                //strStyle = strStyle.TrimStart('?');
                //strStyle = strStyle.Replace('#', ':');

                //strTemp = strTemp.Remove(0, strStyle.Length + 3);
                string strNewFileName = strTemp;

                string strReplace = "&lt;img src=\"" + EAExporter.m_strImageFileRelPath + "\\" + strNewFileName + "\" width=\"" + strWidth + "\" height=\"" + strHeight + "\" id=\"" + strId + "\" align=\"" + strAlign + "\" style=\"" + strStyle + "\"&gt;";

                strValue = strValue.Replace("&#13;&#10;" + strLink + "&#13;&#10;", strReplace);

                System.IO.File.Copy(strOldFullPath, EAExporter.m_strImageFileFullPath + "\\" + strNewFileName, true);

            }

            return 0;
        }

        private int GetNewDimensions(string strFileName, ref int iWidth, ref int iHeight)
        {
            Image image = System.Drawing.Image.FromFile(strFileName);

            double resizeMaxWidth = 500;
            double resizeMaxHeight = 200;
            double resizeWidth = 0;
            double resizeHeight = 0;

            double maxAspect = (double)resizeMaxWidth / (double)resizeMaxHeight;
            double aspect = (double)image.Width / (double)image.Height;

            if (maxAspect > aspect && image.Width > resizeMaxWidth)
            {
                //Width is the bigger dimension relative to max bounds
                resizeWidth = resizeMaxWidth;
                resizeHeight = resizeMaxWidth / aspect;
            }
            else if (maxAspect <= aspect && image.Height > resizeMaxHeight)
            {
                //Height is the bigger dimension
                resizeHeight = resizeMaxHeight;
                resizeWidth = resizeMaxHeight * aspect;
            }

            iWidth = (int)resizeWidth;
            iHeight = (int)resizeHeight;

            image.Dispose();

            return 0;
        }

        private int GetDiagrams(string strUseCasePackage, XElement XUseCase)
        {
            int iErrCnt = 0;
            int iWidth = 0;
            int iHeight = 0;
            string strNewFileName = "";
            string strOrigFileName = "";

            EA.Package eaPackage = GetPackage(strUseCasePackage + "/Enterprise");

            EA.Diagram eaDiagram = eaPackage.Diagrams.GetByName("Use Case Diagram");

            EA.Project iProject = m_Repository.GetProjectInterface();

            strOrigFileName = "UseCaseDiagram.png";
            Boolean bResult = iProject.PutDiagramImageToFile(eaDiagram.DiagramGUID, EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, 1);

            GetNewDimensions(EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, ref iWidth, ref iHeight);
            strNewFileName = @"\W" + iWidth + "_H" + iHeight + "_" + strOrigFileName;
            System.IO.File.Delete(EAExporter.m_strImageFileFullPath + strNewFileName);
            System.IO.File.Move(EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, EAExporter.m_strImageFileFullPath + strNewFileName);

            XElement xelDiagram = new XElement("Diagram");
            xelDiagram.Add(new XElement("drawingType", "UML"));
            xelDiagram.Add(new XElement("name", "Use Case Diagram"));
            xelDiagram.Add(new XElement("URI", EAExporter.m_strImageFileRelPath + strNewFileName));

            XUseCase.Add(xelDiagram);

            EA.Package eaPackageComputation = GetPackage(strUseCasePackage + "/Computation");

            for (short sRevLoop = Convert.ToInt16(eaPackageComputation.Packages.Count - 1); sRevLoop >= 0; sRevLoop--)
            {
                EA.Package eaScenario = eaPackageComputation.Packages.GetAt(sRevLoop);

                string strScenarioName = eaScenario.Name;

                EA.Package eaPackageActivity = eaScenario.Packages.GetByName("Activity");

                eaDiagram = eaPackageActivity.Diagrams.GetByName("Activity Diagram");

                strOrigFileName = strScenarioName + "_Activity.png";
                bResult = iProject.PutDiagramImageToFile(eaDiagram.DiagramGUID, EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, 1);

                GetNewDimensions(EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, ref iWidth, ref iHeight);
                strNewFileName = @"\W" + iWidth + "_H" + iHeight + "_" + strOrigFileName;
                System.IO.File.Delete(EAExporter.m_strImageFileFullPath + strNewFileName);
                System.IO.File.Move(EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, EAExporter.m_strImageFileFullPath + strNewFileName);

                xelDiagram = new XElement("Diagram");
                xelDiagram.Add(new XElement("drawingType", "UML"));
                xelDiagram.Add(new XElement("name", strScenarioName + " Activity Diagram"));
                xelDiagram.Add(new XElement("URI", EAExporter.m_strImageFileRelPath + strNewFileName));

                XUseCase.Add(xelDiagram);

                EA.Package eaPackageSequence = eaScenario.Packages.GetByName("Sequence");

                eaDiagram = eaPackageSequence.Diagrams.GetByName("Sequence Diagram");

                strOrigFileName = strScenarioName + "_Sequence.png";
                bResult = iProject.PutDiagramImageToFile(eaDiagram.DiagramGUID, EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, 1);

                GetNewDimensions(EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, ref iWidth, ref iHeight);
                strNewFileName = @"\W" + iWidth + "_H" + iHeight + "_" + strOrigFileName;
                System.IO.File.Delete(EAExporter.m_strImageFileFullPath + strNewFileName);
                System.IO.File.Move(EAExporter.m_strImageFileFullPath + @"\" + strOrigFileName, EAExporter.m_strImageFileFullPath + strNewFileName);

                xelDiagram = new XElement("Diagram");
                xelDiagram.Add(new XElement("drawingType", "UML"));
                xelDiagram.Add(new XElement("name", strScenarioName + " Sequence Diagram"));
                xelDiagram.Add(new XElement("URI", EAExporter.m_strImageFileRelPath + strNewFileName));

                XUseCase.Add(xelDiagram);

            }


            return iErrCnt;
        }


        protected XElement GetUseCase(string strUseCasePackage)
        {
            XElement XUseCase = new XElement("UseCase");

            EA.Package eaPackageEnterprise = GetPackage(strUseCasePackage + "/Enterprise");

            // First Emit UseCase Obj Instance
            EA.Element eaUseCaseClassObj = eaPackageEnterprise.Elements.GetByName("UseCase");
            Hashtable hTaggedValues = ParseRunStateString(eaUseCaseClassObj.RunState);
            ObjInst_UseCase objUseCase = new ObjInst_UseCase();
            objUseCase.GetElements(hTaggedValues, XUseCase, eaUseCaseClassObj);


            // Next emit Business Cases
            int iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_BusinessCase(), "BusinessCase", GetPackage(strUseCasePackage + "/Enterprise/RelatedBusinessCases"), XUseCase, true);

            // Next Emit Conditions
            foreach (EA.Package eaPackageCondition in GetPackage(strUseCasePackage + "/Enterprise/Conditions").Packages)
            {
                // first create Condition and its elements
                XElement xElCondition = new XElement("Condition");
                iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_Condition(), null, eaPackageCondition, xElCondition);

                // child: precondtion
                iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_PreCondition(), "PreCondition", eaPackageCondition, xElCondition);

                // child: referencedActor
                iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_ReferencedActor(), "referencedActor", eaPackageCondition, xElCondition);

                // Add Condition to UseCase
                XUseCase.Add(xElCondition);
            }

            // Emit diagrams
            iErr = GetDiagrams(strUseCasePackage, XUseCase);

            // Next emit General Remarks
            iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_GeneralRemark(), "GeneralRemark", GetPackage(strUseCasePackage + "/Enterprise/GeneralRemarks"), XUseCase);

            // Next emit Narrative
            XElement XNarrative = new XElement("Narrative");
            EA.Element eaNarrativeClassObj = eaPackageEnterprise.Elements.GetByName("Narrative");
            hTaggedValues = ParseRunStateString(eaNarrativeClassObj.RunState);
            ObjInst_Narrative objNarrative = new ObjInst_Narrative();
            objNarrative.GetElements(hTaggedValues, XNarrative, eaNarrativeClassObj);
            XUseCase.Add(XNarrative);

            // emit Related Objectives
            iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_RelatedObjective(), "RelatedObjective", GetPackage(strUseCasePackage + "/Enterprise/RelatedObjectives"), XUseCase);

            // emit Primary Domain
            EA.Package eaPackageUseCase = GetPackage(strUseCasePackage);
            EA.Package eaPackageDomain = m_Repository.GetPackageByID(eaPackageUseCase.ParentID);

            XElement XPrimaryDomain = new XElement("PrimaryDomain");
            XPrimaryDomain.Add(new XElement("name", eaPackageDomain.Name));
            XUseCase.Add(XPrimaryDomain);

            // emit Additional Domains
            iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_AdditionalDomain(), "AdditionalDomain", GetPackage(strUseCasePackage + "/Enterprise/AdditionalDomains"), XUseCase);

            // emit References
            iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_Reference(), "Reference", GetPackage(strUseCasePackage + "/Enterprise/References"), XUseCase);

            // emit scenarios
            EA.Package ePackageComputation = GetPackage(strUseCasePackage + "/Computation");
            XElement XScenarios = GetScenariosForUseCase(ePackageComputation);
            XUseCase.Add(XScenarios.XPathSelectElements("*"));

            // emit relations
            //for (short sRevLoop = Convert.ToInt16(GetPackage(strUseCasePackage + "/Enterprise/UseCaseRelations").Packages.Count - 1); sRevLoop >= 0; sRevLoop--)
            foreach (EA.Package eaPackageRelation in GetPackage(strUseCasePackage + "/Enterprise/UseCaseRelations").Packages)
            {
                //    EA.Package eaPackageRelation = GetPackage(strUseCasePackage + "/Enterprise/UseCaseRelations").Packages.GetAt(sRevLoop);

                XElement xElRelation = new XElement("UseCaseRelation");
                iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_UseCaseRelation(), null, eaPackageRelation, xElRelation);

                // child: RelatedUseCase
                iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_RelatedUseCase(m_iRefUseCaseClsID), "RelatedUseCase", eaPackageRelation, xElRelation, true);

                // Add Condition to UseCase
                XUseCase.Add(xElRelation);
            }

            // emit Versions
            iErr = ObjInstanceElementsCollector.GetObjInstanceList(new ObjInst_VersionInformation(), "VersionInformation", GetPackage(strUseCasePackage + "/Enterprise/Versions"), XUseCase);

            return XUseCase;
        }

        public List<string> GetListOfUseCases()
        {
            List<string> theStrList = new List<string>();

            m_Repository.Models.Refresh();

            try
            {
                m_Packages["UseCaseRepository"] = m_Repository.Models.GetByName("UseCaseRepository");
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Unable to find root package called UseCaseRepository. Exporting use cases requires that all use cases exist under the UseCaseRepository root package. Please move use cases to this package and try again.");
                return null;
            }

            if (m_Packages["UseCaseRepository"] == null)
            {
                LogError(LogErrorLevel.A, "Unable to find root package called UseCaseRepository. Exporting use cases requires that all use cases exist under the UseCaseRepository root package. Please move use cases to this package and try again.");
                return null;
            }

            try
            {
                foreach (EA.Package eaPackageDomain in GetPackage("/UseCaseRepository/UseCaseLibrary/ImportedUseCase").Packages)
                {
                    string strDomainName = "/UseCaseRepository/UseCaseLibrary/ImportedUseCase/" + eaPackageDomain.Name;

                    foreach (EA.Package eaPackageUseCase in GetPackage(strDomainName).Packages)
                    {
                        string strUseCaseName = strDomainName + "/" + eaPackageUseCase.Name;

                        theStrList.Add(strUseCaseName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not find model/package: " + ex.Message);
                return null;
            }

            return theStrList;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Export(string strUseCasePackage, string strXSLTFullPath, string strFileName)
        {
            if (strFileName == "")
            {
                LogError(LogErrorLevel.A, "Output file name must be supplied.");
                return 1;
            }

            XSLTPath = Path.GetFullPath(strXSLTFullPath);

            if (!System.IO.File.Exists(strXSLTFullPath))
            {
                LogError(LogErrorLevel.A, "Could not load xslt. File " + strXSLTFullPath + " does not exist.");
                return 1;
            }

            return Export(strUseCasePackage, strFileName);
        }

        public virtual int Export(string strUseCasePackage, string strFileName)
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

            StringBuilder sbXML = new StringBuilder();

            System.IO.StreamWriter file;
            try
            {
                file = new System.IO.StreamWriter(strXMLDataFile);
            }
            catch (Exception ex)
            {
                LogError(LogErrorLevel.A, "Could not create output file. Error: " + ex.Message);
                return 1;
            }

            if (file == null)
            {
                LogError(LogErrorLevel.A, "Could not create output file.");
                return 1;
            }


            string strUseCaseName = strUseCasePackage.Substring(strUseCasePackage.LastIndexOf("/") + 1);

            try
            {
                XElement XActorLibrary = GetActorLibraryForUseCase(GetPackage(strUseCasePackage + "/Computation"), GetPackage(strUseCasePackage + "/Enterprise"), strUseCaseName);

                XElement XDomainLibrary = GetDomainLibraryForUseCase(GetPackage(strUseCasePackage + "/Computation"), GetPackage(strUseCasePackage + "/Enterprise"));

                XElement XInformationModelLibrary = GetInfoExchangedForUseCase(GetPackage(strUseCasePackage + "/Computation"));

                if ((XActorLibrary == null) || (XDomainLibrary == null) || (XInformationModelLibrary == null))
                {
                    LogError(LogErrorLevel.A, "Could not find requisite package(s).");
                    return 1;
                }

                file.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

                file.WriteLine("<UC:UseCaseRepository xsi:schemaLocation=\"http://www.TC8.org/IEC62559/UseCaseTemplate/V01 IEC62559UseCaseTemplate_V01.xsd\" xmlns:UC=\"http://www.TC8.org/IEC62559/UseCaseTemplate/V01\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

                file.WriteLine(XActorLibrary);
                if (XDomainLibrary != null)
                {
                    file.WriteLine(XDomainLibrary);
                }

                file.WriteLine(XInformationModelLibrary);

                XElement XUseCaseLibrary = new XElement("UseCaseLibrary");
                XElement XUseCase = GetUseCase(strUseCasePackage);
                XUseCaseLibrary.Add(XUseCase);
                file.WriteLine(XUseCaseLibrary);

                file.WriteLine("</UC:UseCaseRepository>");
                file.Flush();
                file.Close();
            }
            catch (EpriException ex)
            {
                LogError(LogErrorLevel.A, ex.Message);
                return 1;
            }


            string strXMLOutput = System.IO.File.ReadAllText(strXMLDataFile);

            XslCompiledTransform xslt = new XslCompiledTransform();

            xslt.Load(XSLTPath);

            // Create the writer.
            XmlWriterSettings XWriterSettings = new XmlWriterSettings();
            XWriterSettings.Indent = true;
            XWriterSettings.IndentChars = "\t";
            XWriterSettings.ConformanceLevel = ConformanceLevel.Auto;

            XmlWriter writer = XmlWriter.Create(outputFile, XWriterSettings);

            StringReader strXMLSource = new StringReader(strXMLOutput);
            XDocument xmlTree = XDocument.Load(strXMLSource);

            xslt.Transform(xmlTree.CreateReader(), writer);

            writer.Close();

            return 0;
        }
    }
}
