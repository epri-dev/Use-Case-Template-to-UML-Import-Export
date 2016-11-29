/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportInfoModels.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 5 $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportInfoModels.cs 5     11/05/12 11:31a Ronp $

$History: ImportInfoModels.cs $
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

using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections;

using System.Windows.Forms;
using System.Text.RegularExpressions; // used for Application.DoEvents -- may need to remove.

namespace UCRepoClassLibrary
{
    internal class ImportInfoModels
    {
        private Hashtable m_InfoClasses = new Hashtable();

        public EA.Element Get(string strInfoClassName)
        {
            return (EA.Element)m_InfoClasses[strInfoClassName];
        }
        /// <summary>
        /// AddInformationClass
        /// </summary>
        /// <param name="strPackageName"></param>
        /// <param name="strInfoClassName"></param>
        /// <param name="strInfoClassDescription"></param>
        private int AddInformationClass(EA.Package eaPackage, string strInfoClassName, string strInfoClassDescription, string strRequirements)
        {
            string strLogInfo = "Information Class: " + strInfoClassName;

            int iErrorCount = 0;

            EA.Element element1 = null;
            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            if (eaPackage == null)
            {
                throw new EpriException("cannot add infomation class- package does not exist " + strInfoClassName, "");
                //EAImporter.LogError(EAImporter.LogErrorLevel.A, "cannot add infomation class- package does not exist " + strInfoClassName);
                iErrorCount++;
                return iErrorCount;
            }

            if (m_InfoClasses[strInfoClassName] != null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return iErrorCount;
            }

            try
            {
                element1 = eaPackage.Elements.GetByName(strInfoClassName);
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
                // EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, m_Repository.Stereotypes.GetLastError() + " : " + ex.Message);
            }

            if (element1 == null)
            {
                element1 = eaPackage.Elements.AddNew(strInfoClassName, "Class");
                element1.Notes = strInfoClassDescription;
                element1.Update();

                EA.TaggedValue eaTgVal = element1.TaggedValues.AddNew("requirements", "TaggedValue");
                eaTgVal.Value = strRequirements;
                eaTgVal.Update();

                eaPackage.Elements.Refresh();
                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);

                m_InfoClasses[strInfoClassName] = element1;
            }
            else
            {
                m_InfoClasses[strInfoClassName] = element1;

                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
            }

            return iErrorCount;
        }

        /// <summary>
        /// ProcessInformationClasses
        /// </summary>
        /// <param name="el"></param>
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


                string strRequirements = "";

                EA.Package oRequirements = EAImporter.m_Packages.Add("UseCaseRepository", "RequirementLibrary");

                IEnumerable<XElement> elRequirements = elInfoClass.Elements("referencedRequirement");
                foreach (XElement elRequirement in elRequirements)
                {
                    string strID = elRequirement.Element("id").Value;

                    strRequirements = strRequirements + "," + strID;

                    EAImporter.m_Requirements.AddInformationClass(oRequirements, strID, "");

                }

                if (strRequirements != "")
                {
                    strRequirements = strRequirements.Remove(0, 1);
                }

                int iErrCnt = oFieldsValidator.ParseAndValidateFields("Information Class");

                if (iErrCnt == 0)
                {
                    oFieldsValidator.ReplaceImageLinks("UseCaseRepository/InformationLibrary/" + oFieldsValidator["name"].Value);
                    //oFieldsValidator.ReplaceImageLinks("UCRepo/InfoLib/" + Utils.GetShortText(name));

                    iErrorCount = iErrorCount + AddInformationClass(eaInfoClassPackage, oFieldsValidator["name"].Value, oFieldsValidator["description"].Value, strRequirements);
                }
                else
                {
                    iErrorCount = iErrorCount + iErrCnt;
                }
            }

            return iErrorCount;
        }
    }
}
