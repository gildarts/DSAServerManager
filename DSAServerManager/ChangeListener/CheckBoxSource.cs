using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSAServerManager
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckBoxSource : ChangeSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controls"></param>
        public CheckBoxSource(params CheckBox[] controls)
        {
            Controls = new Dictionary<CheckBox, CheckState>();
            foreach (CheckBox control in controls)
            {
                Controls.Add(control, control.CheckState);
                control.CheckStateChanged += new EventHandler(Control_CheckStateChanged);
            }
        }

        private void Control_CheckStateChanged(object sender, EventArgs e)
        {
            bool changed = false;
            foreach (CheckBox control in Controls.Keys)
            {
                if (control.CheckState != Controls[control])
                {
                    changed = true;
                    break;
                }
            }
            if (changed)
                RaiseStatusChanged(ValueStatus.Dirty);
            else
                RaiseStatusChanged(ValueStatus.Clean);
        }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<CheckBox, CheckState> Controls { get; private set; }

        /// <summary>
        /// 重設狀態為「Clean」。
        /// </summary>
        public override void Reset()
        {
            foreach (CheckBox control in new Dictionary<CheckBox, CheckState>(Controls).Keys)
                Controls[control] = control.CheckState;
        }
    }
}
