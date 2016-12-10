using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EPRi
{
    public partial class HelpBox : Form
    {
        private string resourceFolder;

        public HelpBox(string strResourceFolder)
        {
            this.resourceFolder = strResourceFolder;
            InitializeComponent();
        }

        private void HelpBox_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("You can find the sample files in: " + resourceFolder);
            sb.AppendLine("\nTo import use cases, import the Exchange Profile provided in above directory as a root node.");
            sb.AppendLine("\nDirections to import Exchange Profile:");
            sb.AppendLine("\t1. Right Click on root node");
            sb.AppendLine("\t2. Click on Import Model from XML.");
            sb.AppendLine("\t3. Select ExchangeProfile.xml from the sample folder.");
            sb.AppendLine("\t4. Import the xml as Root Model.");
            sb.AppendLine("\t5. Re-open the Project file.");

            textBoxDescription.Text = sb.ToString();

        }
    }
}
