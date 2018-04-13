namespace LockStock
{
    partial class frmLockStock
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.gvDDXX = new System.Windows.Forms.DataGridView();
            this.gvSKXX = new System.Windows.Forms.DataGridView();
            this.btLock = new System.Windows.Forms.Button();
            this.btUnLock = new System.Windows.Forms.Button();
            this.btExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvDDXX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSKXX)).BeginInit();
            this.SuspendLayout();
            // 
            // gvDDXX
            // 
            this.gvDDXX.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvDDXX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvDDXX.Location = new System.Drawing.Point(12, 12);
            this.gvDDXX.Name = "gvDDXX";
            this.gvDDXX.RowTemplate.Height = 23;
            this.gvDDXX.Size = new System.Drawing.Size(811, 231);
            this.gvDDXX.TabIndex = 0;
            this.gvDDXX.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GVddxx_CellClick);
            // 
            // gvSKXX
            // 
            this.gvSKXX.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvSKXX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSKXX.Location = new System.Drawing.Point(12, 249);
            this.gvSKXX.Name = "gvSKXX";
            this.gvSKXX.RowTemplate.Height = 23;
            this.gvSKXX.Size = new System.Drawing.Size(905, 261);
            this.gvSKXX.TabIndex = 1;
            this.gvSKXX.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.GVskxx_CellValueChanged);
            // 
            // btLock
            // 
            this.btLock.Location = new System.Drawing.Point(842, 31);
            this.btLock.Name = "btLock";
            this.btLock.Size = new System.Drawing.Size(75, 39);
            this.btLock.TabIndex = 2;
            this.btLock.Text = "锁库";
            this.btLock.UseVisualStyleBackColor = true;
            this.btLock.Click += new System.EventHandler(this.BTLock_Click);
            // 
            // btUnLock
            // 
            this.btUnLock.Location = new System.Drawing.Point(842, 101);
            this.btUnLock.Name = "btUnLock";
            this.btUnLock.Size = new System.Drawing.Size(75, 39);
            this.btUnLock.TabIndex = 3;
            this.btUnLock.Text = "解锁";
            this.btUnLock.UseVisualStyleBackColor = true;
            this.btUnLock.Click += new System.EventHandler(this.BTUnLock_Click);
            // 
            // btExit
            // 
            this.btExit.Location = new System.Drawing.Point(842, 171);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(75, 39);
            this.btExit.TabIndex = 4;
            this.btExit.Text = "退出";
            this.btExit.UseVisualStyleBackColor = true;
            this.btExit.Click += new System.EventHandler(this.BTExit_Click);
            // 
            // frmLockStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 522);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.btUnLock);
            this.Controls.Add(this.btLock);
            this.Controls.Add(this.gvSKXX);
            this.Controls.Add(this.gvDDXX);
            this.Name = "frmLockStock";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LockStock";
            ((System.ComponentModel.ISupportInitialize)(this.gvDDXX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSKXX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvDDXX;
        private System.Windows.Forms.DataGridView gvSKXX;
        private System.Windows.Forms.Button btLock;
        private System.Windows.Forms.Button btUnLock;
        private System.Windows.Forms.Button btExit;
    }
}

