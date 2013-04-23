using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSAServerManager
{
    internal class DetailItems
    {
        public DetailItems()
        {
            Program.MainPanel.RegisterDetailContent<BasicInfoItem>();
            //Program.MainPanel.RegisterDetailContent<NetworkItem>();
            //Program.MainPanel.RegisterDetailContent<UDMItem>();
            //Program.MainPanel.RegisterDetailContent<UDTItem>();
            //Program.MainPanel.RegisterDetailContent<UDSItem>();
            //Program.MainPanel.RegisterDetailContent<ModuleItem>();
            //Program.MainPanel.RegisterDetailContent<WebGadgetItem>();
        }
    }
}
