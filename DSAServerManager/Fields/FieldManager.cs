using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSAServerManager.Fields
{
    internal class FieldManager
    {
        internal static TitleField TitleField { get; private set; }

        internal static DSNSField DSNSField { get; private set; }

        internal static GroupField GroupField { get; private set; }

        public FieldManager()
        {

            TitleField = new DSAServerManager.Fields.TitleField();
            GroupField = new DSAServerManager.Fields.GroupField();
            DSNSField = new DSAServerManager.Fields.DSNSField();

            TitleField.Register(Program.MainPanel);
            DSNSField.Register(Program.MainPanel);
            GroupField.Register(Program.MainPanel);
        }
    }
}
