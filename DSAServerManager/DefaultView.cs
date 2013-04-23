using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevComponents.AdvTree;
using FISCA.Presentation;

namespace DSAServerManager
{
    internal class DefaultView : TreeNavViewBase
    {
        public DefaultView()
        {
            InitializeComponent();

            NavText = "預設檢視";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DefaultView
            // 
            this.Name = "DefaultView";
            this.ResumeLayout(false);
        }

        protected override int KeyCatalogComparer(KeyCatalog x, KeyCatalog y)
        {
            int X, Y;

            if (!int.TryParse(x.Tag + "", out X))
                X = int.MinValue;

            if (!int.TryParse(y.Tag + "", out Y))
                Y = int.MinValue;

            return X.CompareTo(Y);
        }

        protected override void GenerateTreeStruct(KeyCatalog root)
        {
            string cmd = "select uid,\"group\" from $dsaserver where uid in(@PrimaryKeys) order by \"group\"";

            StringBuilder primarykeys = new StringBuilder();
            primarykeys.AppendFormat("{0}", "-1"); //如果沒有資料也不會爆掉。
            foreach (string key in Source)
                primarykeys.AppendFormat(",{0}", key);

            cmd = cmd.Replace("@PrimaryKeys", primarykeys.ToString());

            DataTable result = Backend.Select(cmd);

            root["未分類"].Tag = int.MaxValue;

            foreach (DataRow row in result.Rows)
            {
                string id = row["uid"].ToString();
                string group = row["group"].ToString();

                if (string.IsNullOrWhiteSpace(group))
                    group = "未分類";

                root[group].AddKey(id);
            }
        }
    }
}
