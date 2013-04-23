using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSAServerManager.Fields
{
    internal class GroupField : ListPaneFieldImproved
    {
        public GroupField()
            : base("分類", "Group")
        {
        }

        protected override IEnumerable<ListPaneFieldImproved.Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select uid,\"group\" from $dsaserver where uid in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["uid"] + "";
                string title = row["group"] + "";
                results.Add(new Value(id, title, string.Empty));
            });

            return results;
        }
    }
}
