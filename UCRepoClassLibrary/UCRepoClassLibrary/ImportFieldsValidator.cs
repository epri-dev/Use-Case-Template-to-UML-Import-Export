/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportFieldsValidator.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 18 $

	Date last modified: $Modtime: 11/21/12 2:06p $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportFieldsValidator.cs 18    12/10/12 3:00p Ronp $

$History: ImportFieldsValidator.cs $
 * 
 * *****************  Version 18  *****************
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
 * *****************  Version 17  *****************
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

using System.Text.RegularExpressions;
using System.IO;

namespace UCRepoClassLibrary
{
    internal class FieldInfo
    {
        private XElement m_XElement = null;
        private string m_strXElementValueName = "";
        private string m_strExtractedValue = "";
        private bool m_bRequired = true;
        private string m_strLocalName = "";
        private bool m_bIsID = false;
        private bool m_bIsNote = false;
        private bool m_bIsArray = false;

        List<string> m_strValuesList = new List<string>();

        public FieldInfo(string strLocalName, string strValue)
        {
            m_XElement = null;

            m_bIsID = bIsID;
            m_bIsNote = false;

            m_strLocalName = strLocalName;
            m_bRequired = false;
            m_strExtractedValue = strValue;

            m_bIsArray = false;
        }

        public FieldInfo(string strLocalName, XElement XElement, string strXElementValueName = "", bool bRequired = true, bool bIsID = false, bool bIsNotes = false, bool bIsArray = false)
        {
            m_XElement = XElement;
            if (strXElementValueName == "")
            {
                m_strXElementValueName = strLocalName;
            }
            else
            {
                m_strXElementValueName = strXElementValueName;
            }

            m_bIsID = bIsID;
            m_bIsNote = bIsNotes;

            m_strLocalName = strLocalName;
            m_bRequired = bRequired;
            m_strExtractedValue = "";

            m_bIsArray = bIsArray;

        }
        public bool Required
        {
            get { return m_bRequired; }
            set { m_bRequired = value; }
        }
        public bool bIsID
        {
            get { return m_bIsID; }
            set { m_bIsID = value; }
        }
        public bool bIsNote
        {
            get { return m_bIsNote; }
            set { m_bIsNote = value; }
        }
        public string GetRunStateString()
        {
            string strRunState = "";

            if (m_bIsArray)
            {
                if (m_strValuesList.Count > 1)
                {
                    int i = 0;
                    foreach (string strValue in m_strValuesList)
                    {
                        strRunState = strRunState + "@VAR;Variable=" + m_strXElementValueName + "[" + i.ToString("0") + "]" + ";Value=" + strValue + ";Op==;@ENDVAR;";
                        i++;
                    }
                }
                else if (m_strValuesList.Count==0)
                {
                    strRunState = "";
                }
                else
                {
                    strRunState = strRunState + "@VAR;Variable=" + m_strXElementValueName + ";Value=" + m_strValuesList[0] + ";Op==;@ENDVAR;";
                }
            }
            else
            {
                strRunState = "@VAR;Variable=" + m_strXElementValueName + ";Value=" + m_strExtractedValue + ";Op==;@ENDVAR;";
            }

            return strRunState;
        }
        public string Value
        {
            get { return m_strExtractedValue; }
            set { m_strExtractedValue = value; }
        }
        public void AddArrayValue(string strValue)
        {
            m_strValuesList.Add(strValue);
        }
        public string ValueName
        {
            get { return m_strXElementValueName; }
            set { m_strXElementValueName = value; }
        }
        public string LocalName
        {
            get { return m_strLocalName; }
            set { m_strLocalName = value; }
        }
        public XElement XElement
        {
            get { return m_XElement; }
        }
        public bool ReplaceImageLinks(string strImagePackagePath)
        {
            if (m_bIsNote)
            {
                while (Value.IndexOf("<img ") > 0)
                {
                    Regex rgx1 = new Regex(@"<img.*?>");
                    MatchCollection matches1 = rgx1.Matches(Value);

                    foreach (Match match1 in matches1)
                    {
                        Regex rgxSrc = new Regex(@"(?<=src="").*?(?="")");
                        MatchCollection matchesSrc = rgxSrc.Matches(match1.Value);

                        Regex rgxWidth = new Regex(@"(?<=width="").*?(?="")");
                        MatchCollection matchesWidth = rgxWidth.Matches(match1.Value);

                        Regex rgxHeight = new Regex(@"(?<=height="").*?(?="")");
                        MatchCollection matchesHeight = rgxHeight.Matches(match1.Value);

                        Regex rgxId = new Regex(@"(?<=id="").*?(?="")");
                        MatchCollection matchesId = rgxId.Matches(match1.Value);

                        Regex rgxAlign = new Regex(@"(?<=align="").*?(?="")");
                        MatchCollection matchesAlign = rgxAlign.Matches(match1.Value);

                        Regex rgxStyle = new Regex(@"(?<=style="").*?(?="")");
                        MatchCollection matchesStyle = rgxStyle.Matches(match1.Value);

                        string strAlign = "";
                        if (matchesAlign.Count > 0)
                        {
                            strAlign = matchesAlign[0].Value;
                        }

                        string strStyle = "";
                        if (matchesStyle.Count > 0)
                        {
                            strStyle = matchesStyle[0].Value;
                        }

                        strStyle = strStyle.Replace(':', '#');

                        string strTargetPath = System.IO.Path.Combine(@"\\127.0.0.1\EAImages", strImagePackagePath);

                        string sourceFile = System.IO.Path.Combine(EAImporter.m_strImportFilePath, matchesSrc[0].Value);
                        string strNewFileName = "W" + matchesWidth[0].Value + "_H" + matchesHeight[0].Value + "_ID" + matchesId[0].Value + "_AL" + strAlign + "_ST" + strStyle + "_" + matchesSrc[0].Value.Split('/')[1];
                        string destFile = System.IO.Path.Combine(strTargetPath, strNewFileName);

                        try
                        {
                            if (!Directory.Exists(strTargetPath))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(strTargetPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            EAImporter.LogError(EAImporter.LogErrorLevel.A, "Could not create directory for images: " + ex.Message);
                            return false;
                        }

                        sourceFile = sourceFile.Replace("%20", " ");
                        System.IO.File.Copy(sourceFile, destFile, true);

                        string strReplacement = "&#13;&#10;<a href=\"" + destFile + "\"><font color=\"#0000ff\"><u>" + "<Ctl Left Click with Mouse to View Image>" + "</u></font></a>&#13;&#10;";

                        Value = Value.Replace(match1.Value, strReplacement);

                        EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Replacing image ref: " + match1.Value);
                        EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Replacing image ref with: " + strReplacement);
                    }
                }
            }

            return true;
        }


        public bool ParseAndValidateField()
        {
            if (m_XElement == null)
            {
                return true;
            }

            bool bSuccess = false;
            try
            {
                bool bFoundDoc = false;
                Value = "";

                if (!bFoundDoc)
                {
                    if (m_bIsArray)
                    {
                        IEnumerable<XElement> elValues = m_XElement.Elements(ValueName);

                        bSuccess = true;

                        foreach (XElement elValue in elValues)
                        {
                            AddArrayValue(elValue.Value);

                            if (elValue.Value == "")
                            {
                                bSuccess = false;
                            }
                        }
                    }
                    else
                    {
                        Value = m_XElement.Element(ValueName).Value;
                    }
                }

                if (Value != "")
                {
                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
            }

            if ((!bSuccess) && (!m_bRequired))
            {
                bSuccess = true;
            }

            return bSuccess;
        }
    }

    internal class ImportFieldsValidator
    {
        private Hashtable m_Fields = new Hashtable();
        private int m_ImportErrCnt = 0;

        public ImportFieldsValidator()
        {
        }

        public Hashtable GetFields()
        {
            return m_Fields;
        }

        public int ErrorCount
        {
            get { return m_ImportErrCnt; }
        }

        

        internal FieldInfo this[string strFieldName]
        {
            set
            {
                m_Fields[strFieldName] = value;
            }
            get
            {
                return (FieldInfo)m_Fields[strFieldName];
            }
        }

        internal void Add(FieldInfo oFieldInfo)
        {
            m_Fields[oFieldInfo.LocalName] = oFieldInfo;
        }

        public string GetIDStrName()
        {
            foreach (DictionaryEntry deFieldIndo in m_Fields)
            {
                FieldInfo oFieldInfo = (FieldInfo)deFieldIndo.Value;

                if (oFieldInfo.bIsID)
                {
                    return oFieldInfo.ValueName;
                }
            }

            return "";
        }

        public string GetNotesStrName()
        {
            foreach (DictionaryEntry deFieldIndo in m_Fields)
            {
                FieldInfo oFieldInfo = (FieldInfo)deFieldIndo.Value;

                if (oFieldInfo.bIsNote)
                {
                    return oFieldInfo.ValueName;
                }
            }

            return "";
        }

        public int ParseAndValidateFields(string strParsingElementNote)
        {
            int iErrorCount = 0;

            foreach (DictionaryEntry deFieldIndo in m_Fields)
            {
                FieldInfo oFieldInfo = (FieldInfo)deFieldIndo.Value;
                //string strErrorInfo = strParsingElementNote + " field: " + oFieldInfo.ValueName;
                string strCurrElement = "";

                if (!oFieldInfo.ParseAndValidateField())
                {
                    if (oFieldInfo.XElement != null)
                    {
                        strCurrElement = " (" + EAImporter.Recurse(oFieldInfo.XElement) + ")";
                    }
                    else
                    {
                        //strErrorInfo = strErrorInfo + " in: EMPTY";
                    }

                    iErrorCount++;

                    EAImporter.LogError(EAImporter.LogErrorLevel.A, "\"" + oFieldInfo.ValueName + "\" missing or empty from " + strParsingElementNote + strCurrElement);
                }
            }

            m_ImportErrCnt = iErrorCount;
            return m_ImportErrCnt;
        }

        public int ReplaceImageLinks(string strImagePackagePath)
        {
            int iErrorCount = 0;

            foreach (DictionaryEntry deFieldIndo in m_Fields)
            {
                FieldInfo oFieldInfo = (FieldInfo)deFieldIndo.Value;
                string strErrorInfo = "Field: " + oFieldInfo.ValueName;

                if (!oFieldInfo.ReplaceImageLinks(strImagePackagePath))
                {
                    //strErrorInfo = strErrorInfo + " in: " + EAImporter.Recurse(oFieldInfo.XElement);
                    //EAImporter.LogError(EAImporter.LogErrorLevel.A, "Field missing or empty: " + strErrorInfo); 
                    iErrorCount++;
                }
            }

            m_ImportErrCnt = iErrorCount;
            return m_ImportErrCnt;
        }
    }
}
