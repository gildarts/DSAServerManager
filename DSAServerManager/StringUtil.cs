using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSAServerManager
{
    public static class StringUtil
    {
        /// <summary>
        /// 提供功能將變數取代為 Id 清單，例：'1','2','3'。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="paramName"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string ReplaceList(this string src, string paramName, IEnumerable<string> idList, string quote)
        {
            string sep = quote + "," + quote;
            return src.Replace(paramName, quote + string.Join(sep, idList) + quote);
        }

        /// <summary>
        /// 提供功能將變數取代為 Id 清單，quote 預設為「'」，例：'1','2','3'。
        /// </summary>
        /// <param name="src"></param>
        /// <param name="paramName"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string ReplaceList(this string src, string paramName, IEnumerable<string> idList)
        {
            return src.ReplaceList(paramName, idList, "'");
        }
    }
}
