/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportPackages.cs $  

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportPackages.cs 3     11/05/12 11:31a Ronp $

$History: ImportPackages.cs $
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

namespace UCRepoClassLibrary
{

    internal class ImportPackages
    {
        private Hashtable m_Packages = new Hashtable();

        public EA.Package this[string strPackageName]
        {
            //set
            //{
            //    m_Packages[strPackageName] = value;
            //}
            get
            {
                return (EA.Package)m_Packages[strPackageName];
            }
        }

        public EA.Package Add(string strNewPackageName, EA.Package eaPackage)
        {

            if (strNewPackageName == "")
            {
                string strErrInfo = "Package name is empty.";
                EAImporter.LogError(EAImporter.LogErrorLevel.A, strErrInfo);
                return null;
            }

            if (eaPackage == null)
            {
                string strErrInfo = "Must provide package.";
                EAImporter.LogError(EAImporter.LogErrorLevel.A, strErrInfo);
                return null;
            }

            string strLogInfo = "Package: " + strNewPackageName;

            EA.Package oNewPackage = (EA.Package)m_Packages[strNewPackageName];

            if (oNewPackage != null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return oNewPackage;
            }

            m_Packages[strNewPackageName] = eaPackage;

            return eaPackage;

        }

        public EA.Package Add(string parentPackageName, string strNewPackageName, string strNotes = "", XElement CurrXEl = null)
        {
            string strLogInfo = "Package: " + strNewPackageName;

            EAImporter.LogMsg(EAImporter.LogMsgType.Adding, strLogInfo);

            if (parentPackageName == "")
            {
                string strErrInfo = "Parent package name is empty.";
                if (CurrXEl != null)
                {
                    strErrInfo = strErrInfo + "Processing element: " + EAImporter.Recurse(CurrXEl);
                }
                EAImporter.LogError(EAImporter.LogErrorLevel.A, strErrInfo);
                return null;
            }

            if (strNewPackageName == "")
            {
                string strErrInfo = "Package name is empty.";
                if (CurrXEl != null)
                {
                    strErrInfo = strErrInfo + "Processing element: " + EAImporter.Recurse(CurrXEl);
                }
                EAImporter.LogError(EAImporter.LogErrorLevel.A, strErrInfo);
                return null;
            }

            EA.Package oNewPackage = (EA.Package)m_Packages[parentPackageName + "/" + strNewPackageName];

            if (oNewPackage != null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                return oNewPackage;
            }

            if ((EA.Package)m_Packages[parentPackageName] == null)
            {
                string strErrInfo = "Add Failed- parent package does not exist";
                if (CurrXEl != null)
                {
                    strErrInfo = strErrInfo + "Processing element: " + EAImporter.Recurse(CurrXEl);
                }
                EAImporter.LogError(EAImporter.LogErrorLevel.A, strErrInfo);
                return null;
            }

            try
            {
                oNewPackage = ((EA.Package)m_Packages[parentPackageName]).Packages.GetByName(strNewPackageName);
            }
            catch (Exception ex)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.MiscExceptions, ex.Message);
            }

            if (oNewPackage == null)
            {
                oNewPackage = ((EA.Package)m_Packages[parentPackageName]).Packages.AddNew(strNewPackageName, "Nothing");
                oNewPackage.Notes = strNotes;
                oNewPackage.Update();

                m_Packages[parentPackageName + "/" + strNewPackageName] = oNewPackage;

                EAImporter.LogMsg(EAImporter.LogMsgType.Added, strLogInfo);
            }
            else
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Exists, strLogInfo);
                m_Packages[parentPackageName + "/" + strNewPackageName] = oNewPackage;
                return oNewPackage;
            }

            return oNewPackage;
        }
    }
}
