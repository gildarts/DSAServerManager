using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DSAServerManager.Forms;
using FISCA.Presentation;
using FISCA.UDT;

namespace DSAServerManager
{
    class RibbonButtons
    {
        public RibbonButtons()
        {
            Program.MainPanel.RibbonBarItems["管理"]["新增"].Image = Properties.Resources.atom_add_128;
            Program.MainPanel.RibbonBarItems["管理"]["新增"].Size = RibbonBarButton.MenuButtonSize.Large;
            Program.MainPanel.RibbonBarItems["管理"]["新增"].Click += delegate
            {
                DialogResult dr = new AddNewForm().ShowDialog();
                if (dr == DialogResult.OK)
                    Program.RefreshFilteredSource();
            };

            Program.MainPanel.RibbonBarItems["管理"]["刪除"].Image = Properties.Resources.atom_close_128;
            Program.MainPanel.RibbonBarItems["管理"]["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;
            Program.MainPanel.RibbonBarItems["管理"]["刪除"].Click += delegate
            {
                DialogResult dr = MessageBox.Show("刪除選擇的學校？", "Campus", MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                {
                    AccessHelper ah = new AccessHelper();
                    List<DSAServer> dsaservers = ah.Select<DSAServer>(Program.MainPanel.SelectedSource);

                    dsaservers.ForEach((x) => x.Deleted = true);
                    dsaservers.SaveAll();
                    Program.RefreshFilteredSource();
                }
            };

            //Program.MainPanel.RibbonBarItems["進階"]["搜尋"].Image = Properties.Resources.lamp_search_128;
            //Program.MainPanel.RibbonBarItems["進階"]["搜尋"].Size = RibbonBarButton.MenuButtonSize.Large;
            //Program.MainPanel.RibbonBarItems["進階"]["搜尋"].Click += delegate
            //{
            //    new SearchForm().ShowDialog();
            //};
        }
    }
}
