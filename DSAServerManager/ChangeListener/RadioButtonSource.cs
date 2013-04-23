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
    public class RadioButtonSource : ChangeSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controls"></param>
        public RadioButtonSource(params RadioButton[] controls)
        {
            Controls = new Dictionary<RadioButton, bool>();
            foreach (RadioButton control in controls)
            {
                Controls.Add(control, control.Checked);
                control.CheckedChanged += new EventHandler(Control_CheckedChanged);
            }
        }

        private void Control_CheckedChanged(object sender, EventArgs e)
        {
            bool changed = false;
            foreach (RadioButton control in Controls.Keys)
            {
                if (control.Checked != Controls[control])
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
        protected Dictionary<RadioButton, bool> Controls { get; private set; }

        /// <summary>
        /// 重設狀態為「Clean」。
        /// </summary>
        public override void Reset()
        {
            foreach (RadioButton control in new Dictionary<RadioButton, bool>(Controls).Keys)
                Controls[control] = control.Checked;
        }
    }
}
