/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ObjInstHelpers.cs $  

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ObjInstHelpers.cs 3     11/05/12 11:31a Ronp $

$History: ObjInstHelpers.cs $
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
    internal class ObjInstanceElementsCollector
    {
        private class FieldInfo
        {
            public string m_strFldName;
            public bool m_bIsArray;
        };

        ArrayList m_OrderedFieldList = new ArrayList();

        protected string m_FieldNameFromObjName = null;
        protected string m_FieldNameFromNotes = null;
        protected int m_iBaseType = 0;
        protected bool m_stripPrefix = false;
        protected bool m_bUseObjClassNameForLookup = false;

        protected void Add(string strFldName, bool bIsArray = false)
        {
            FieldInfo fldInfo = new FieldInfo();
            fldInfo.m_strFldName = strFldName;
            fldInfo.m_bIsArray = bIsArray;

            m_OrderedFieldList.Add(fldInfo);
        }

        public int GetBaseType()
        {
            return m_iBaseType;
        }

        public string GetUMLObjClassName()
        {
            string strTemp = this.GetType().Name;
            strTemp = strTemp.Replace("ObjInst_", "");
            return strTemp;
        }

        public bool GetUseObjClassNameForLookup()
        {
            return m_bUseObjClassNameForLookup;
        }

        public int GetElements(Hashtable hTaggedValues, XElement Xel, EA.Element eaEl)
        {
            foreach (Object objFldInfo in m_OrderedFieldList)
            {
                FieldInfo fldInfo = (FieldInfo)objFldInfo;
                string objElName = fldInfo.m_strFldName;

                string strValue = "";

                if ((string)objElName == m_FieldNameFromObjName)
                {
                    strValue = eaEl.Name;
                }
                else if ((string)objElName == m_FieldNameFromNotes)
                {
                    strValue = eaEl.Notes;
                }
                else if (fldInfo.m_bIsArray)
                {
                    // find all array elements
                    int iCnt = 0;

                    foreach (DictionaryEntry deFieldIndo in hTaggedValues)
                    {
                        string strName = deFieldIndo.Key.ToString();
                        if (strName.StartsWith(objElName))
                        {
                            iCnt++;
                        }
                    }

                    if (iCnt == 1)
                    {
                        if ((string)hTaggedValues[(string)objElName] != null)
                        {
                            strValue = (string)hTaggedValues[(string)objElName];
                        }

                        while (strValue.Contains("\r\n"))
                        {
                            strValue = strValue.Replace("\r\n", "&#13;&#10;");
                        }

                        if (m_stripPrefix)
                        {
                            strValue = strValue.Replace(GetUMLObjClassName() + "_", "");
                        }

                        if (strValue != "")
                        {
                            // check for embedded image
                            int iRes = EAExporter.CheckForAndReplaceLinks(ref strValue);

                            Xel.Add(new XElement((string)objElName, strValue));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < iCnt; i++)
                        {
                            strValue = (string)hTaggedValues[(string)objElName + "[" + i.ToString("0") + "]"];
                            // replace new paragrpahs
                            while (strValue.Contains("\r\n"))
                            {
                                strValue = strValue.Replace("\r\n", "&#13;&#10;");
                            }

                            if (m_stripPrefix)
                            {
                                strValue = strValue.Replace(GetUMLObjClassName() + "_", "");
                            }

                            if (strValue != "")
                            {
                                // check for embedded image
                                int iRes = EAExporter.CheckForAndReplaceLinks(ref strValue);

                                Xel.Add(new XElement((string)objElName, strValue));
                            }
                        }
                    }
                }
                else
                {
                    if ((string)hTaggedValues[(string)objElName] != null)
                    {
                        strValue = (string)hTaggedValues[(string)objElName];
                    }
                }

                if (!fldInfo.m_bIsArray)
                {
                    // replace new paragrpahs
                    while (strValue.Contains("\r\n"))
                    {
                        strValue = strValue.Replace("\r\n", "&#13;&#10;");
                    }

                    if (m_stripPrefix)
                    {
                        strValue = strValue.Replace(GetUMLObjClassName() + "_", "");
                    }

                    if (strValue != "")
                    {
                        // check for embedded image
                        int iRes = EAExporter.CheckForAndReplaceLinks(ref strValue);

                        Xel.Add(new XElement((string)objElName, strValue));
                    }
                }
            }

            return 0;
        }

        internal static int GetObjInstanceList(ObjInstanceElementsCollector ObjInst_, string strElementName, EA.Element eaElement, XElement Xel)
        {

            if (eaElement.Type == "Object")
            {
                bool bFound = false;

                if (ObjInst_.GetUseObjClassNameForLookup())
                {
                    if (eaElement.Name.StartsWith(ObjInst_.GetUMLObjClassName()))
                    {
                        bFound = true;
                    }
                }
                else
                {
                    bFound = true;
                }

                if (ObjInst_.GetBaseType() != 0)
                {
                    if (eaElement.ClassifierID == ObjInst_.GetBaseType())
                    {
                        bFound = true;
                    }
                    else
                    {
                        bFound = false;
                    }
                }

                if (bFound)
                {
                    Hashtable hTaggedValues = EAExporter.ParseRunStateString(eaElement.RunState);

                    if (strElementName != null)
                    {
                        XElement XParent = new XElement(strElementName);

                        int iErr = ObjInst_.GetElements(hTaggedValues, XParent, eaElement);

                        Xel.Add(XParent);
                    }
                    else
                    {
                        int iErr = ObjInst_.GetElements(hTaggedValues, Xel, eaElement);
                    }
                }
            }

            return 0;
        }

        internal static int GetObjInstanceList(ObjInstanceElementsCollector ObjInst_, string strElementName, EA.Package eaPackage, XElement Xel, bool bRevIterate = false)
        {
            // Go through each object in this package for Obj Instances
            int iErrCnt = 0;

            if (bRevIterate)
            {
                for (short sLoop = Convert.ToInt16(eaPackage.Elements.Count - 1); sLoop >= 0; sLoop--)
                {
                    EA.Element eaElement = eaPackage.Elements.GetAt(sLoop);
                    iErrCnt = iErrCnt + GetObjInstanceList(ObjInst_, strElementName, eaElement, Xel);
                }
            }
            else
            {
                foreach (EA.Element eaElement in eaPackage.Elements)
                {
                    iErrCnt = iErrCnt + GetObjInstanceList(ObjInst_, strElementName, eaElement, Xel);
                }
            }

            return iErrCnt;
        }
    }

    internal class ObjInst_UseCase : ObjInstanceElementsCollector
    {
        public ObjInst_UseCase()
        {
            // specify field name in the order to be emitted
            Add("classification");
            Add("id");
            Add("keywords");
            Add("levelOfDepth");
            Add("name");
            Add("prioritisation");
            Add("scope");
            Add("viewPoint");
        }
    }

    internal class ObjInst_BusinessCase : ObjInstanceElementsCollector
    {
        public ObjInst_BusinessCase()
        {
            // specify field name in the order to be emitted
            Add("name");
            m_FieldNameFromObjName = "name";
        }
    }

    internal class ObjInst_Condition : ObjInstanceElementsCollector
    {
        public ObjInst_Condition()
        {
            // specify field name in the order to be emitted
            Add("assumption");
            Add("trigerringEvent");
            m_bUseObjClassNameForLookup = true;
        }
    }

    internal class ObjInst_PreCondition : ObjInstanceElementsCollector
    {
        public ObjInst_PreCondition()
        {
            // specify field name in the order to be emitted
            Add("content");
            m_bUseObjClassNameForLookup = true;
        }
    }

    internal class ObjInst_ReferencedActor : ObjInstanceElementsCollector
    {
        public ObjInst_ReferencedActor()
        {
            // specify field name in the order to be emitted
            Add("name");
            m_bUseObjClassNameForLookup = true;
        }
    }

    internal class ObjInst_GeneralRemark : ObjInstanceElementsCollector
    {
        public ObjInst_GeneralRemark()
        {
            // specify field name in the order to be emitted
            Add("content");
            m_FieldNameFromNotes = "content";
        }
    }

    internal class ObjInst_Narrative : ObjInstanceElementsCollector
    {
        public ObjInst_Narrative()
        {
            // specify field name in the order to be emitted
            Add("completeDescription");
            Add("shortDescription");
            m_FieldNameFromNotes = "completeDescription";
        }
    }

    internal class ObjInst_RelatedObjective : ObjInstanceElementsCollector
    {
        public ObjInst_RelatedObjective()
        {
            // specify field name in the order to be emitted
            Add("name");
            m_FieldNameFromObjName = "name";
        }
    }

    internal class ObjInst_AdditionalDomain : ObjInstanceElementsCollector
    {
        public ObjInst_AdditionalDomain()
        {
            // specify field name in the order to be emitted
            Add("name");
            m_FieldNameFromObjName = "name";
        }
    }

    internal class ObjInst_Reference : ObjInstanceElementsCollector
    {
        public ObjInst_Reference()
        {
            // specify field name in the order to be emitted
            Add("description");
            Add("id");
            Add("impact");
            Add("originatorOrganisation");
            Add("status");
            Add("type");
            Add("URI");
            m_FieldNameFromObjName = "id";
            m_FieldNameFromNotes = "description";
        }
    }

    internal class ObjInst_VersionInformation : ObjInstanceElementsCollector
    {
        public ObjInst_VersionInformation()
        {
            // specify field name in the order to be emitted
            Add("approvalStatus");
            Add("areaOfExpertise");
            Add("changes");
            Add("date");
            Add("domainExpert");
            Add("name");
            Add("title");
            m_FieldNameFromObjName = "date";
            m_FieldNameFromNotes = "changes";
        }
    }

    internal class ObjInst_UseCaseRelation : ObjInstanceElementsCollector
    {
        public ObjInst_UseCaseRelation()
        {
            // specify field name in the order to be emitted
            Add("type");
            m_bUseObjClassNameForLookup = true;
        }
    }

    internal class ObjInst_RelatedUseCase : ObjInstanceElementsCollector
    {
        public ObjInst_RelatedUseCase(int iRefUseCaseClsID)
        {
            // specify field name in the order to be emitted
            Add("name");
            m_FieldNameFromObjName = "name";
            m_iBaseType = iRefUseCaseClsID;
            m_bUseObjClassNameForLookup = true;
        }
    }

    internal class ObjInst_Scenario : ObjInstanceElementsCollector
    {
        public ObjInst_Scenario(int ScenarioBaseClsID)
        {
            // specify field name in the order to be emitted
            Add("name");
            Add("number");
            Add("postCondition", true);
            Add("preCondition", true);
            Add("triggeringEvent");
            m_iBaseType = ScenarioBaseClsID;
        }
    }

    internal class ObjInst_PrimaryActor : ObjInstanceElementsCollector
    {
        public ObjInst_PrimaryActor()
        {
            // specify field name in the order to be emitted
            Add("name");
            m_FieldNameFromObjName = "name";
            m_stripPrefix = true;
            m_bUseObjClassNameForLookup = true;
        }
    }

}
