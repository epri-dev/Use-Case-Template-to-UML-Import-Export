/*
--------------------------------------------------------------------------
**********   <<UCReoClassLibrary >> 							 *********
--------------------------------------------------------------------------

	$Workfile: AboutBox1.cs $  

	Original Author: Ronald J. Pasquarelli
	$Author: Ronp $ 
	$Revision: 5 $

	Date last modified: $Modtime: 12/10/12 6:25p $

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
$Header: /_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp/AboutBox1.cs 5     12/10/12 6:30p Ronp $

$History: AboutBox1.cs $
 * 
 * *****************  Version 5  *****************
 * User: Ronp         Date: 12/10/12   Time: 6:30p
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * IUCTEMPLATE-15: Correct Help/About information.
 * 
 * *****************  Version 4  *****************
 * User: Ronp         Date: 11/05/12   Time: 11:31a
 * Updated in $/_project/EPRI/C18322-EPRIUseCaseRepository/Source/UCRepoClientApp/UCRepoClientApp
 * Update keyword expansion

*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace UCRepoClientApp
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;


            this.labelVersion.Text = Application.ProductVersion;


            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                this.labelVersion.Text = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Major + "." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor + "." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Build + "." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision;
            }

            //this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                //object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                //if (attributes.Length == 0)
                //{
                //    return "";
                //}
                //return ((AssemblyDescriptionAttribute)attributes[0]).Description;

                string strVersion = Application.ProductVersion;


                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    strVersion = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Major + "." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Minor + "." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Build + "." + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.Revision;
                }

                string strResult = @"
Software:	    UCRepoClientApp version " + strVersion + @" 
                                                   
Developed for:	
                Electric Power Research Institute (EPRI)                                             
                3420 Hillview Ave.                                           
                Palo Alto, CA 94304 
                                                                                                        
Support	        
                EPRI Customer Assistance Center
                Phone: 800-313-3774
                Email: askepri@epri.com

Copyright	    
                Copyright ©2012 Electric Power Research Institute, Inc. 

                Permission to use, copy, modify, and distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.

Developed by:	
                Ronald J. Pasquarelli, Martin J. Burns
                Hypertek, Inc.
                14624 Country Creek Lane
                No. Potomac, MD  20878

Ordering Information:	

                The embodiments of this Program and supporting materials may be ordered from

                Electric Power Software Center (EPSC)
                9625 Research Drive
                Charlotte, NC 28262
                Phone  	1-800-313-3774
                Email	askepri@epri.com
	

Disclaimer:	    

THIS NOTICE MAY NOT BE REMOVED FROM THE PROGRAM BY ANY USER THEREOF.
 
THE SOFTWARE IS PROVIDED ""AS IS"" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

";
                return strResult;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AboutBox1_Load(object sender, EventArgs e)
        {
            
        }

        private void labelCompanyName_Click(object sender, EventArgs e)
        {

        }
    }
}
