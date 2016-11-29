/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportRequirements.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 3 $

	Date last modified: $Modtime: 11/05/12 11:27a $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportRequirements.cs 3     11/05/12 11:31a Ronp $

$History: ImportRequirements.cs $
 * 
 * *****************  Version 3  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary
 * Update keyword expansion

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections;

using System.Windows.Forms; // used for Application.DoEvents -- may need to remove.

namespace UCRepoClassLibrary
{
    class ImportRequirements
    {

        private Hashtable m_Requirements = new Hashtable();

        public EA.Element Get(string strRequirementID)
        {
            return (EA.Element)m_Requirements[strRequirementID];
        }

        internal int AddInformationClass(EA.Package eaPackage, string strRequirementID, string strDescription)
        {
            string strLogInfo = "Requirement: " + strRequirementID;

            int iErrorCount = 0;

            EA.Element element1 = null;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            if (eaPackage == null)
            {
                throw new EpriException("cannot add requirement- package does not exist " + strRequirementID, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "cannot add requirement- package does not exist " + strRequirementID);
                iErrorCount++;
                return iErrorCount;
            }

            if (m_Requirements[strRequirementID] != null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return iErrorCount;
            }

            try
            {
                element1 = eaPackage.Elements.GetByName(strRequirementID);
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
                // EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
            }

            if (element1 == null)
            {
                element1 = eaPackage.Elements.AddNew(strRequirementID, "Class");
                element1.Notes = strDescription;
                element1.Update();
                eaPackage.Elements.Refresh();
                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

                m_Requirements[strRequirementID] = element1;
            }
            else
            {
                m_Requirements[strRequirementID] = element1;

                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
            }

            return iErrorCount;
        }

        /*
        public int Import(XElement el, EA.Package eaInfoClassPackage)
        {
            int iErrorCount = 0;

            if (el == null)
            {
                return iErrorCount;
            }

            IEnumerable<XElement> elInfoClasses = el.Elements("InformationModel");

            foreach (XElement elInfoClass in elInfoClasses)
            {
                Application.DoEvents();

                ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                oFieldsValidator.Add(new FieldInfo("name", elInfoClass));
                oFieldsValidator.Add(new FieldInfo("description", elInfoClass, "", false, false, true));

                int iErrCnt = oFieldsValidator.ParseAndValidateFields("Information Class");

                if (iErrCnt == 0)
                {
                    oFieldsValidator.ReplaceImageLinks("UseCaseRepository/InformationModelLibrary/" + oFieldsValidator["name"].Value);

                    iErrorCount = iErrorCount + AddInformationClass(eaInfoClassPackage, oFieldsValidator["name"].Value, oFieldsValidator["description"].Value);
                }
                else
                {
                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }
        */
    }
}
