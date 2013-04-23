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
    public class ComboBoxSource : ChangeSource
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ListenAttribute
        {
            /// <summary>
            /// 
            /// </summary>
            SelectedIndex, 
            /// <summary>
            /// 
            /// </summary>
            Text
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="target"></param>
        public ComboBoxSource(ComboBox control, ListenAttribute target)
        {
            ListenOnValidated = false;
            ListenTarget = target;
            Control = control;
            SubscribeControlEvents();
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="target"></param>
        /// <param name="listenOnValidated"></param>
        public ComboBoxSource(ComboBox control, ListenAttribute target, bool listenOnValidated)
        {
            ListenOnValidated = listenOnValidated;
            ListenTarget = target;
            Control = control;
            SubscribeControlEvents();
            Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SubscribeControlEvents()
        {
            Control.TextChanged += new EventHandler(Control_TextChanged);
            Control.SelectedIndexChanged += new EventHandler(Control_SelectedIndexChanged);
            Control.Validated += new EventHandler(Control_Validated);
        }

        /// <summary>
        /// 
        /// </summary>
        protected ComboBox Control { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected string OriginText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected int OriginSelectedIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ListenAttribute ListenTarget { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ListenOnValidated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            OriginText = Control.Text;
            OriginSelectedIndex = Control.SelectedIndex;
        }

        private void Control_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListenTarget != ListenAttribute.SelectedIndex) return;
            RaiseSelectedIndexChanged();
        }

        private void Control_TextChanged(object sender, EventArgs e)
        {
            if (ListenTarget != ListenAttribute.Text) return;
            RaiseTextChanged();
        }

        private void Control_Validated(object sender, EventArgs e)
        {
            if (ListenTarget == ListenAttribute.SelectedIndex)
                RaiseSelectedIndexChanged();

            if (ListenTarget == ListenAttribute.Text)
                RaiseTextChanged();
        }

        private void RaiseSelectedIndexChanged()
        {
            if (Control.SelectedIndex != OriginSelectedIndex)
                RaiseStatusChanged(ValueStatus.Dirty);
            else
                RaiseStatusChanged(ValueStatus.Clean);
        }

        private void RaiseTextChanged()
        {
            if (Control.Text != OriginText)
                RaiseStatusChanged(ValueStatus.Dirty);
            else
                RaiseStatusChanged(ValueStatus.Clean);
        }
    }
}
