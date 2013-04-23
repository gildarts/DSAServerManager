using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Data;
using FISCA.Presentation.Controls;

namespace DSAServerManager.Forms
{
    public partial class AddNewForm : BaseForm
    {
        public AddNewForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DSAServer s = new DSAServer();
                s.Title = txtTitle.Text;
                s.DSNS = txtDSNS.Text;
                s.Group = cboGroup.Text;
                s.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void AddNewForm_Load(object sender, EventArgs e)
        {
            try
            {
                QueryHelper qh = new QueryHelper();
                DataTable dt = qh.Select("select \"group\" from $dsaserver group by \"group\" order by \"group\"");

                foreach (DataRow row in dt.Rows)
                {
                    string gname = row["group"] + "";
                    cboGroup.Items.Add(gname);
                }
            }
            catch { }
        }
    }
}
