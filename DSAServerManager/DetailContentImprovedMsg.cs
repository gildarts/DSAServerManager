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
    [ToolboxItemAttribute(false)]
    internal partial class DetailContentImprovedMsg : UserControl
    {
        public DetailContentImprovedMsg()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get { return txtMsg.Text; } set { txtMsg.Text = value; } }
    }
}
