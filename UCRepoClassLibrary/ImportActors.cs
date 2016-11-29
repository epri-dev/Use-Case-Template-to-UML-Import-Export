/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportActors.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 5 $

	Date last modified: $Modtime: 11/05/12 11:26a $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportActors.cs 5     11/05/12 11:31a Ronp $

$History: ImportActors.cs $
 * 
 * *****************  Version 5  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
 * Update keyword expansion

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

using System.Windows.Forms;
using System.Text.RegularExpressions; // used for Application.DoEvents -- may need to remove.

namespace UCRepoClassLibrary
{

    internal class ImportActors
    {
        class ActorInformation
        {
            public ImportFieldsValidator m_oFieldsValidator = null;
            public EA.Element m_eaEl = null;
            public bool m_bUseCaseRef = false;
        }


        private Hashtable m_ActorsStruct = new Hashtable();

        /// <summary>
        /// AddStereotype
        /// </summary>
        /// <param name="strName"></param>
        private void AddStereotype(string strName)
        {
            string strLogInfo = "Stereotype: " + strName;

            EA.Stereotype Stereotype;

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            try
            {
                // cannot lookup stereotypes by name so iterate through list
                for (short i = 0; i < EAImporter.m_Repository.Stereotypes.Count; i++)
                {
                    Application.DoEvents();
                    Stereotype = EAImporter.m_Repository.Stereotypes.GetAt(i);
                    if (Stereotype.Name == strName)
                    {
                        EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, EAImporter.m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
            }

            try
            {
                Stereotype = EAImporter.m_Repository.Stereotypes.AddNew(strName, "actor");
                Stereotype.Notes = "UseCaseRepository";
                Stereotype.Update();
            }
            catch (Exception ex)
            {
                string strTemp = ex.Message;
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return;
            }

            EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
        }

        /// <summary>
        /// GetActor
        /// </summary>
        /// <param name="strActorName"></param>
        /// <returns>EA.Element</returns>
        public EA.Element GetActor(string strActorName)
        {
            if (m_ActorsStruct[strActorName] == null)
            {
                return null;
            }

            return ((ActorInformation)m_ActorsStruct[strActorName]).m_eaEl;
        }

        public EA.Element GetActorInterface(string strActorName)
        {
            if (m_ActorsStruct[strActorName] == null)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Can't get Actor interface- Actor does not exist " + strActorName);
                return null;
            }

            EA.Element eTemp = ((ActorInformation)m_ActorsStruct[strActorName]).m_eaEl;
            if (eTemp == null)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Can't get Actor interface- Actor does not exist " + strActorName);
                return null;
            }

            try
            {
                EA.Element eIntfc = eTemp.Elements.GetByName(strActorName);
                return eIntfc;
            }
            catch (Exception ex)
            {
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Can't get Actor interface- Actor interface does not exist " + strActorName + ex.Message);
            }

            return null;
        }

        public void MarkActorUseCaseRef(string strName)
        {
            if (m_ActorsStruct[strName] == null)
            {
                return;
            }

            ((ActorInformation)m_ActorsStruct[strName]).m_bUseCaseRef = true;
        }

        public Hashtable GetActorUseCaseRefs()
        {
            Hashtable UCRefActors = new Hashtable();

            foreach (DictionaryEntry deActor in m_ActorsStruct)
            {
                if (((ActorInformation)deActor.Value).m_bUseCaseRef == true)
                {
                    UCRefActors[deActor.Key] = ((ActorInformation)deActor.Value).m_oFieldsValidator;
                }
            }

            return UCRefActors;
        }

        private int AddActor(EA.Package eaPackage, ImportFieldsValidator oFieldsValidator)
        {
            string strName = oFieldsValidator["name"].Value;
            string strStereotype = oFieldsValidator["type"].Value;
            string strNotes = oFieldsValidator["description"].Value;

            string strLogInfo = "Actor: " + strName;

            int iErrorCount = 0;

            EA.Element element1 = null;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            if (m_ActorsStruct[strName] != null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return 0;
            }

            if (eaPackage == null)
            {
                throw new EpriException("cannot add actor- package does not exist " + strName, oFieldsValidator["name"].XElement.getLocation());
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "cannot add actor- package does not exist " + strName);
                return 1;
            }

            try
            {
                element1 = eaPackage.Elements.GetByName(strName);
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
            }

            if (element1 == null)
            {
                element1 = eaPackage.Elements.AddNew(strName, "Actor");
                eaPackage.Update();
                eaPackage.Elements.Refresh();
                element1.Notes = strNotes;
                element1.StereotypeEx = strStereotype;

                //EA.Element eIntfc = element1.Elements.AddNew(strName, "Interface");
                EA.Element eIntfc = element1.Elements.AddNew(strName, "Class");
                eIntfc.Update();
                element1.Update();
                element1.Elements.Refresh();

                EA.Element source = eIntfc;
                EA.Element target = element1;

                EA.Connector con = source.Connectors.AddNew("realizes", "Realization");

                con.SupplierID = target.ElementID;
                con.Update();
                source.Connectors.Refresh();

                EA.Connector con2 = source.Connectors.AddNew("", "Generalization");

                con2.SupplierID = EAImporter.m_iActorBaseClsID;
                con2.Update();
                source.Connectors.Refresh();

                ActorInformation oActorInfo = new ActorInformation();
                oActorInfo.m_eaEl = element1;
                oActorInfo.m_oFieldsValidator = oFieldsValidator;

                m_ActorsStruct[strName] = oActorInfo;

                element1.Update();
                eIntfc.Update();

                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            }
            else
            {
                ActorInformation oActorInfo = new ActorInformation();
                oActorInfo.m_eaEl = element1;
                oActorInfo.m_oFieldsValidator = oFieldsValidator;
                m_ActorsStruct[strName] = oActorInfo;

                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
            }
            return iErrorCount;
        }

        /// <summary>
        /// Import
        /// </summary>
        /// <param name="el"></param>
        public int Import(XElement el)
        {
            int iErrorCount = 0;

            if (el == null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Info, "No actors to import ");
                return iErrorCount;
            }

            foreach (XElement elDomain in el.Elements("Domain"))
            {
                ImportFieldsValidator oFieldsValidatorDomain = new ImportFieldsValidator();
                oFieldsValidatorDomain.Add(new FieldInfo("name", elDomain));

                int iErrCnt = oFieldsValidatorDomain.ParseAndValidateFields("Actor Category");
                if (iErrCnt != 0)
                {
                    iErrorCount = iErrorCount + iErrCnt;
                }
                else
                {
                    IEnumerable<XElement> elActors = elDomain.Elements("Actor");

                    foreach (XElement elActor in elActors)
                    {
                        Application.DoEvents();

                        ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                        oFieldsValidator.Add(new FieldInfo("name", elActor));
                        oFieldsValidator.Add(new FieldInfo("type", elActor));
                        oFieldsValidator.Add(new FieldInfo("description", elActor, "", false, false, true));
                        oFieldsValidator.Add(new FieldInfo("furtherInformation", elActor, "", false));

                        iErrCnt = oFieldsValidator.ParseAndValidateFields("Actor");

                        if (iErrCnt == 0)
                        {
                            oFieldsValidator.ReplaceImageLinks("UseCaseRepository/ActorLibrary" + "/" + oFieldsValidatorDomain["name"].Value + "/" + oFieldsValidator["name"].Value);

                            AddStereotype(elActor.Element("type").Value);

                            iErrorCount = iErrorCount + AddActor(EAImporter.m_Packages["UseCaseRepository/ActorLibrary" + "/" + oFieldsValidatorDomain["name"].Value], oFieldsValidator);
                        }
                        else
                        {
                            iErrorCount = iErrorCount + iErrCnt;
                        }
                    }
                }
            }

            return iErrorCount;
        }


    }
}
