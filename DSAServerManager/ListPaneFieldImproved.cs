using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using System.Threading.Tasks;
using FISCA;
using FISCA.Data;
using System.Threading;
using Campus;

namespace DSAServerManager
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ListPaneFieldImproved
    {
        private ListPaneField Field;

        private DynamicCache CurrentCache;

        private bool IsReloading = false;

        private bool IsDataLoading = false;

        private bool IsPaddingTask = false;

        private TaskScheduler UISyncContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">要顯示的標題文字。</param>
        /// <param name="fieldName">對應資料庫或是 Value Object 的欄位名稱。</param>
        public ListPaneFieldImproved(string title, string fieldName)
        {
            FieldName = fieldName;
            CacheProvider = null;
            CurrentCache = null;

            if (Backend == null)
                Backend = new QueryHelper();

            Field = new ListPaneField(title);
            Field.CompareValue += (CompareValue);
            Field.PreloadVariable += (Field_PreloadVariable);
            Field.GetVariable += (Field_GetVariable);

            UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private void Field_PreloadVariable(object sender, PreloadVariableEventArgs e)
        {
            if (IsDataLoading)
            {
                IsPaddingTask = true;
                return;
            }

            if (IsReloading) return;

            if (CacheProvider != null)
                CurrentCache = CacheProvider;
            else
                CurrentCache = new DynamicCache();

            IsDataLoading = true;
            Task task = Task.Factory.StartNew(() =>
            {
                if (CurrentCache.GetOutOfDate(e.Keys, FieldName))
                {//如果不是在最新狀態就呼叫 GetDataAsync 讀取資料，並更新到 Cache 中。

                    if (e.Keys.Count() <= 0) return; //如果沒有資料就不執行。

                    IEnumerable<Value> values = GetDataAsync(e.Keys);
                    HashSet<string> resultSet = new HashSet<string>();
                    foreach (Value v in values)
                    {
                        CurrentCache.FillProperty(v.Id, FieldName, v.Val);
                        resultSet.Add(v.Id);
                    }

                    HashSet<string> clearSet = new HashSet<string>(e.Keys);
                    clearSet.ExceptWith(resultSet);

                    //將沒有回傳的 Id 值清空。
                    foreach (string id in clearSet)
                        CurrentCache.FillProperty(id, FieldName, string.Empty);
                }
            }, new CancellationToken(), TaskCreationOptions.PreferFairness, TaskScheduler.Default);

            task.ContinueWith((x) =>
            {
                IsDataLoading = false;

                if (IsPaddingTask)
                {
                    IsPaddingTask = false;
                    Field.Reload();
                    return;
                }

                if (x.Exception != null)
                    RTOut.WriteError(x.Exception);
                else
                {
                    IsReloading = true;
                    Field.Reload();
                    IsReloading = false;
                }
                CurrentCache = null;
            }, UISyncContext);
        }

        private void Field_GetVariable(object sender, GetVariableEventArgs e)
        {
            e.Value = (string)CurrentCache[e.Key][FieldName];
        }

        /// <summary>
        /// 是供排序的邏輯。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CompareValue(object sender, CompareValueEventArgs e)
        {
            e.Result = (e.Value1 + "").CompareTo((e.Value2 + ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            throw new NotImplementedException("您應該要實作此方法。");
        }

        /// <summary>
        /// 將欄位註冊到畫面上。
        /// </summary>
        /// <param name="panel"></param>
        public void Register(NLDPanel panel)
        {
            panel.AddListPaneField(Field);
        }

        /// <summary>
        /// 重新讀取欄位資料。
        /// </summary>
        protected void Reload()
        {
            if (CacheProvider != null)
                CacheProvider.SetOutOfDate(FieldName);

            Task.Factory.StartNew(() => Field.Reload(), new CancellationToken(), TaskCreationOptions.None, UISyncContext);
        }

        /// <summary>
        /// 重新讀取欄位資料。
        /// </summary>
        /// <param name="primaryKeys"></param>
        protected void Reload(IEnumerable<string> primaryKeys)
        {
            if (CacheProvider != null)
                CacheProvider.SetOutOfDate(primaryKeys, FieldName);

            Task.Factory.StartNew(() => Field.Reload(), new CancellationToken(), TaskCreationOptions.None, UISyncContext);
        }

        /// <summary>
        /// 後端資料查詢提供者。
        /// </summary>
        protected static QueryHelper Backend { get; private set; }

        /// <summary>
        /// 資料快取提供者，如果不指定則內部則不使用快取。
        /// </summary>
        public DynamicCache CacheProvider { get; set; }

        /// <summary>
        /// 對應的資料庫欄位或是 Value Object 屬性名稱。
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// 代表欄位值。
        /// </summary>
        public struct Value
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <param name="val"></param>
            /// <param name="toolTip"></param>
            public Value(string id, string val, string toolTip)
                : this()
            {
                Id = id;
                Val = val;
                ToolTip = toolTip;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Val { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ToolTip { get; set; }
        }
    }
}
