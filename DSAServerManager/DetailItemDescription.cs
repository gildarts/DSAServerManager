using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSAServerManager
{
    public partial class DetailItemDescription : FISCA.Presentation.DescriptionPane
    {
        public DetailItemDescription()
        {
            InitializeComponent();
            lblName.Text = string.Empty;
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            dynamic school=Program.GlobalSchoolCache[PrimaryKey];

            lblName.Text = string.Format("{0} ({1})",(string)school.Title,(string)school.DSNS);
        }
    }
}
