using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using FISCA.DSA;
using System.Threading.Tasks;
using System.Threading;

namespace DSAServerManager
{
    public partial class BasicInfoItem : DetailContentImproved
    {
        private DSAServer SchoolData { get; set; }

        private string PhysicalUrl { get; set; }

        public BasicInfoItem()
        {
            InitializeComponent();
            Group = "基本資料";
        }

        protected override void OnInitializeComplete(Exception error)
        {
            WatchChange(new TextBoxSource(txtTitle));
            WatchChange(new TextBoxSource(txtDSNS));
            WatchChange(new TextBoxSource(txtGroup));
            WatchChange(new TextBoxSource(txtComment));
        }

        protected override void OnSaveData()
        {
            if (SchoolData != null)
            {
                SchoolData.Title = txtTitle.Text;
                SchoolData.DSNS = txtDSNS.Text;
                SchoolData.Group = txtGroup.Text;
                SchoolData.Comment = txtComment.Text;
                SchoolData.Save();
                Program.RefreshFilteredSource();
                ConnectionHelper.ResetConnection(PrimaryKey);
            }
            ResetDirtyStatus();
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            AccessHelper access = new AccessHelper();
            List<DSAServer> schools = access.Select<DSAServer>(string.Format("uid='{0}'", PrimaryKey));

            if (schools.Count > 0)
                SchoolData = schools[0];
            else
                SchoolData = null;
        }

        private void ResolveUrl()
        {
            PhysicalUrl = string.Empty;
            if (SchoolData != null)
            {
                AccessPoint ap;
                if (AccessPoint.TryParse(SchoolData.DSNS, out ap))
                    PhysicalUrl = ap.Url;
            }
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (SchoolData != null)
            {
                BeginChangeControlData();
                txtTitle.Text = SchoolData.Title;
                txtDSNS.Text = SchoolData.DSNS;
                txtGroup.Text = SchoolData.Group;
                txtComment.Text = SchoolData.Comment;
                txtPhysicalUrl.Text = "解析中...";

                Task task = Task.Factory.StartNew(() =>
                {
                    ResolveUrl();
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

                task.ContinueWith(x =>
                {
                    txtPhysicalUrl.Text = PhysicalUrl;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                ResetDirtyStatus();
            }
            else
                throw new Exception("無查資料：" + PrimaryKey);
        }
    }
}
