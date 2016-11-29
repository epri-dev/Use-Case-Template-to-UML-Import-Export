/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: HelpForm.cs $  

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp/HelpForm.cs 5     11/05/12 11:31a Ronp $

$History: HelpForm.cs $
 * 
 * *****************  Version 5  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * Update keyword expansion
 * 
 * *****************  Version 4  *****************
 * User: Marty        Date: 11/05/12   Time: 10:35a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UCRepoClientApp
{
    public partial class HelpForm : Form
    {
        string m_strHelpFile;

        public HelpForm(string strHelpFile)
        {
            m_strHelpFile = strHelpFile;
            InitializeComponent();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            richTextBox1.LoadFile(m_strHelpFile, RichTextBoxStreamType.RichText);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
