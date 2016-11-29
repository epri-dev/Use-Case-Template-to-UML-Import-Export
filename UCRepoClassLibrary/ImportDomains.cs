/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: ImportDomains.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 4 $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClassLibrary/UCRepoClassLibrary/ImportDomains.cs 4     11/05/12 11:31a Ronp $

$History: ImportDomains.cs $
 * 
 * *****************  Version 4  *****************
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
    internal class ImportDomains
    {
        /// <summary>
        /// ImportDomains
        /// </summary>
        /// <param name="el"></param>
        public int Import(XElement el)
        {
            int iErrorCount = 0;

            if (el == null)
            {
                EAImporter.LogMsg(EAImporter.LogMsgType.Info, "No domains to import");
                return 0;
            }

            IEnumerable<XElement> elDomains = el.Elements("Domain");

            foreach (XElement elDomain in elDomains)
            {
                Application.DoEvents();

                ImportFieldsValidator oFieldsValidator = new ImportFieldsValidator();

                oFieldsValidator.Add(new FieldInfo("name", elDomain, "name"));
                oFieldsValidator.Add(new FieldInfo("description", elDomain, "description", false, false, true));

                int iErrCnt = oFieldsValidator.ParseAndValidateFields("Actor Grouping/Domain");

                if (iErrCnt == 0)
                {
                    oFieldsValidator.ReplaceImageLinks("UseCaseRepository/DomainLibrary" + "/" + oFieldsValidator["name"].Value);

                    EAImporter.LogMsg(EAImporter.LogMsgType.Info, "Importing domain: " + oFieldsValidator["name"].Value);
                    if (EAImporter.m_Packages.Add("UseCaseRepository/DomainLibrary", oFieldsValidator["name"].Value, oFieldsValidator["description"].Value, elDomain) == null) { iErrorCount++; };
                    if (EAImporter.m_Packages.Add("UseCaseRepository/ActorLibrary", oFieldsValidator["name"].Value, "", elDomain) == null) { iErrorCount++; };
                }

                iErrorCount = iErrorCount + iErrCnt;
            }

            return iErrorCount;
        }
    }
}
