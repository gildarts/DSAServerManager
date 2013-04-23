using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FISCA;
using FISCA.Data;
using System.Collections.Generic;
using FISCA.Presentation;
using FISCA.Permission;
using System.Threading;
using Campus;

namespace DSAServerManager
{
    /// <summary>
    /// 增加各項基本重要功能的資料項目。
    /// </summary>
    public partial class DetailContentImproved : FISCA.Presentation.DetailContent
    {
        private DetailContentImprovedMsg MsgPanel { get; set; }

        /// <summary>
        /// 取得是否有 PrimaryKey 等待處理。
        /// </summary>
        private bool PrimaryKeyPadding { get; set; }

        /// <summary>
        /// 背景工作執行緒。
        /// </summary>
        private Task PrimaryKeyTask { get; set; }

        /// <summary>
        /// 提供自動處理 Save、Cancel 出現時機。
        /// </summary>
        private ChangeListen ChangeListen;

        /// <summary>
        /// 取得 UI 排程器，用於排程相關工作於 UI 執行緒執行。
        /// </summary>
        protected TaskScheduler UISyncContext { get; private set; }

        /// <summary>
        /// 直接查詢。 
        /// </summary>
        protected static QueryHelper Backend { get; private set; }

        /// <summary>
        /// 提供畫面錯誤提示。
        /// </summary>
        protected ErrorTip ErrorTip { get; private set; }

        /// <summary>
        /// 指示是否已經初始化過了。
        /// </summary>
        private bool Inited = false;
        /// <summary>
        /// 用於同步。
        /// </summary>
        private object InitedSyncObj = new object();

        /// <summary>
        /// 
        /// </summary>
        public DetailContentImproved()
        {
            InitializeComponent();

            PrimaryKeyPadding = false;
            PrimaryKeyTask = null;
            MsgPanel = null;

            ChangeListen = new ChangeListen();
            ChangeListen.StatusChanged += (sender, e) =>
            {
                OnDirtyStatusChanged(e);
            };
        }

        private void DetailContentImproved_Load(object sender, EventArgs e)
        {
            try
            {
                UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();
                ErrorTip = new ErrorTip();

                if (Backend == null)
                    Backend = new FISCA.Data.QueryHelper();
            }
            catch
            {
            }
        }

        #region Change Manage
        /// <summary>
        /// 新增變更接聽。
        /// </summary>
        /// <param name="source"></param>
        protected void WatchChange(IChangeSource source)
        {
            ChangeListen.Add(source);
        }

        /// <summary>
        /// 指示正在改變控制項的資料，這將使變更接聽暫停運作。
        /// </summary>
        protected void BeginChangeControlData()
        {
            ChangeListen.SuspendListen();
        }

        /// <summary>
        /// 指示控制項的資料變更已經完成，這將使變更接聽開始運作。
        /// </summary>
        protected void EndChangeControlData()
        {
            ChangeListen.ResumeListen();
        }

        /// <summary>
        /// 重設資料修改狀態。
        /// </summary>
        protected void ResetDirtyStatus()
        {
            ChangeListen.Reset();
            ChangeListen.ResumeListen();
        }
        #endregion

        /// <summary>
        /// 覆寫此方法後，如果沒有呼叫 base.OnPrimaryKeyChanged 的話，將不會引發 OnPrimaryKeyChangedProcessing 方法。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            base.OnPrimaryKeyChanged(e);

            Loading = true;

            if (!Inited)
            {
                Task InitTask = Task.Factory.StartNew(() => OnInitializeAsync(), new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
                InitTask.ContinueWith(x =>
                {
                    InvokeComplete(x, OnInitializeComplete);
                    Inited = true;

                    if (!x.IsFaulted)
                        OnPrimaryKeyChanged(EventArgs.Empty);

                }, UISyncContext);

                return;
            }

            if (PrimaryKeyTask == null)
            {
                PrimaryKeyTask = Task.Factory.StartNew(() =>
                {
                    OnPrimaryKeyChangedAsync();
                }, new System.Threading.CancellationToken(), TaskCreationOptions.PreferFairness, TaskScheduler.Default);

                PrimaryKeyTask.ContinueWith(x =>
                {
                    Exception ex = x.Exception;//防止 .Net 內部以為沒處理 Exception。

                    PrimaryKeyTask = null;
                    if (PrimaryKeyPadding)
                    {
                        PrimaryKeyPadding = false;
                        OnPrimaryKeyChanged(EventArgs.Empty);
                    }
                    else
                        InvokeComplete(x, OnPrimaryKeyChangedComplete);
                }, UISyncContext);
            }
            else
                PrimaryKeyPadding = true;
        }

        private void InvokeComplete(Task x, Action<Exception> callback)
        {
            try
            {
                RemoveErrorPanel();

                Loading = false;
                callback(x.Exception);
            }
            catch (Exception ex)
            {
                ShowErrorPanel(ex);
            }
        }

        private Dictionary<Control, bool> ControlsVisible = new Dictionary<Control, bool>();

        private void RemoveErrorPanel()
        {
            if (MsgPanel != null && Controls.Contains(MsgPanel))
                Controls.Remove(MsgPanel);

            MsgPanel = null;

            foreach (Control ctl in Controls)
            {
                if (ControlsVisible.ContainsKey(ctl))
                    ctl.Visible = ControlsVisible[ctl];
            }
        }

        private void ShowErrorPanel(Exception ex)
        {
            ControlsVisible = new Dictionary<Control, bool>();
            foreach (Control ctl in Controls)
                ControlsVisible.Add(ctl, ctl.Visible);
            foreach (Control ctl in Controls)
                ctl.Visible = false;

            string help = "很抱歉，目前資料項目已經炸掉！訊息如下：\r\n";
            string msg = ErrorReport.Generate(ex);
            MsgPanel = new DetailContentImprovedMsg();
            Controls.Add(MsgPanel);
            Controls.SetChildIndex(MsgPanel, 0);
            MsgPanel.Dock = DockStyle.Fill;
            MsgPanel.Message = help + msg;
        }

        /// <summary>
        /// 取得是否已經初始化完成。
        /// </summary>
        protected bool IsInitialized { get { return Inited; } }

        /// <summary>
        /// 取得目前資料項目的權限，需標示 AccessControlAttribute 屬性(Attribute)此屬性(Property)才會有作用。
        /// </summary>
        protected FeatureAce Permission
        {
            get
            {
                AccessControlAttribute aca = Attribute.GetCustomAttribute(GetType(), typeof(AccessControlAttribute)) as AccessControlAttribute;
                return UserAcl.Current[aca.Code];
            }
        }

        /// <summary>
        /// 指示重新初始化相關資料。
        /// </summary>
        protected virtual void ReInitialize()
        {
            Inited = false;
        }

        /// <summary>
        /// 當資料項目第一次載入資料前，並且在背景執行。
        /// </summary>
        protected virtual void OnInitializeAsync()
        {
        }

        /// <summary>
        /// OnInitializeAsync 完成之後執行，在前景執行。
        /// </summary>
        /// <param name="error">如果處理過程有發生錯誤，則不為 Null。</param>
        protected virtual void OnInitializeComplete(Exception error)
        {
        }

        /// <summary>
        /// 當 PrimaryKey 變更時，此方法會在背景呼叫。
        /// </summary>
        protected virtual void OnPrimaryKeyChangedAsync()
        {
            throw new NotImplementedException("您應該要實作 OnPrimaryKeyChangedAsync 方法。");
        }

        /// <summary>
        /// 當 OnPrimaryKeyChangedAsync 完成時，會在前景呼叫此方法。
        /// </summary>
        /// <param name="error">如果處理過程有發生錯誤，則不為 Null。</param>
        protected virtual void OnPrimaryKeyChangedComplete(Exception error)
        {
            throw new NotImplementedException("您應該要實作 OnPrimaryKeyChangedComplete 方法。");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSaveButtonClick(EventArgs e)
        {
            base.OnSaveButtonClick(e);

            ErrorTip.Clear();
            Dictionary<Control, string> errors = new Dictionary<Control, string>();

            try
            {
                RemoveErrorPanel();
                OnValidateData(errors);
            }
            catch (Exception ex)
            {
                ShowErrorPanel(ex);
                return;
            }

            if (errors.Count > 0)
            {
                foreach (KeyValuePair<Control, string> error in errors)
                    ErrorTip.SetError(error.Key, error.Value);
                return;
            }

            try
            {
                RemoveErrorPanel();
                OnSaveData();
            }
            catch (Exception ex)
            {
                ShowErrorPanel(ex);
            }
        }

        /// <summary>
        /// 儲存前驗證資料。
        /// </summary>
        /// <param name="errors">錯誤資訊。</param>
        protected virtual void OnValidateData(Dictionary<Control, string> errors)
        {
        }

        /// <summary>
        /// 當要儲存資料時。
        /// </summary>
        protected virtual void OnSaveData()
        {
        }

        /// <summary>
        /// 當資料有被變更異動時發生(需配合 WatchChange 方法)。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        /// <summary>
        /// 當使用者按下儲存按鈕時。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCancelButtonClick(EventArgs e)
        {
            base.OnCancelButtonClick(e);
            OnPrimaryKeyChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DetailItem_Extend
    {
        /// <summary>
        /// 已使用的 FeatureCode 清單，以防重覆 Register。
        /// </summary>
        private static HashSet<string> FeatureCodes = new HashSet<string>();

        /// <summary>
        /// 記錄每一個型別的權限屬性資訊。
        /// </summary>
        private static Dictionary<Type, AccessControlAttribute> ACL = new Dictionary<Type, AccessControlAttribute>();

        /// <summary>
        /// 註冊資料項目，並且一併處理權限問題。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panel"></param>
        public static void RegisterDetailContent<T>(this NLDPanel panel)
            where T : FISCA.Presentation.DetailContent, new()
        {
            AccessControlAttribute access = Attribute.GetCustomAttribute(typeof(T), typeof(AccessControlAttribute)) as AccessControlAttribute;

            if (access != null)
            {
                if (!ACL.ContainsKey(typeof(T))) //指定的型別沒有被註冊過才進行註冊。
                {
                    ACL.Add(typeof(T), access);

                    Catalog cg = GetCatalog(access.Path);

                    if (!FeatureCodes.Contains(access.Code)) //重覆的代碼也不進行註冊。
                    {
                        FeatureCodes.Add(access.Code);
                        cg.Add(new DetailItemFeature(typeof(T)));
                    }
                }

                if (!UserAcl.Current[access.Code].Viewable)
                    return;
            }

            DetailBulider<T> builder = new DetailBulider<T>();
            builder.ContentBulided += new EventHandler<ContentBulidedEventArgs<T>>(builder_ContentBulided);

            panel.AddDetailBulider(builder);
        }

        private static void builder_ContentBulided<T>(object sender, ContentBulidedEventArgs<T> e)
        {
            if (ACL.ContainsKey(e.Content.GetType()))
            {
                DetailContent dc = e.Content as DetailContent;
                dc.SaveButtonVisibleChanged += new EventHandler(dc_SaveButtonVisibleChanged);
            }
        }

        /// <summary>
        /// 防止 SaveButtonVisibleChanged 事件重覆發生。
        /// </summary>
        private static bool VisibleChanging = false;

        private static void dc_SaveButtonVisibleChanged(object sender, EventArgs e)
        {
            if (VisibleChanging) return;

            DetailContent content = sender as DetailContent;
            AccessControlAttribute ac = ACL[sender.GetType()];

            if (!UserAcl.Current[sender.GetType()].Editable)
            {
                VisibleChanging = true;
                content.SaveButtonVisible = false;
                VisibleChanging = false;
            }
        }

        private static Catalog GetCatalog(string path)
        {
            string[] parts = path.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);

            Catalog result = RoleAclSource.Instance.Root;
            foreach (string name in parts)
                result = result[name];

            return result;
        }
    }

    /// <summary>
    /// 提供權限項目的預設分類，例：學生>資料項目，權限將會放在「學生」分類下的「資料項目」分類。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AccessControlAttribute : FeatureCodeAttribute
    {
        /// <summary>
        /// 提供權限項目的預設分類，例：學生>資料項目。
        /// </summary>
        /// <param name="path">分類路徑，例：學生>資料項目。</param>
        /// <param name="code"></param>
        /// <param name="title"></param>
        public AccessControlAttribute(string code, string title, string path)
            : base(code, title)
        {
            Path = path;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
    }
}
