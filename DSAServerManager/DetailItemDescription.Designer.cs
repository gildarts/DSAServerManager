namespace DSAServerManager
{
    partial class DetailItemDescription
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblName = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            // 
            // 
            // 
            this.lblName.BackgroundStyle.Class = "";
            this.lblName.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblName.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblName.Location = new System.Drawing.Point(0, 0);
            this.lblName.Name = "lblName";
            this.lblName.PaddingLeft = 8;
            this.lblName.Size = new System.Drawing.Size(76, 26);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Content";
            // 
            // DetailItemDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblName);
            this.Name = "DetailItemDescription";
            this.Size = new System.Drawing.Size(385, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lblName;
    }
}
