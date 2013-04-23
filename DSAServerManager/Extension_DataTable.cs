using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DSAServerManager
{
    /// <summary>
    /// 
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class Extension_DataTable
    {
        /// <summary>
        /// 處理每一個 Row。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="function"></param>
        public static void Each(this DataTable dt, Action<DataRow> function)
        {
            foreach (DataRow row in dt.Rows)
                function(row);
        }
    }
}
