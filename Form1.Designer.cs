namespace InjectorInspector
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.inspector1 = new Inspector.Inspector();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.btn_DeviceCreate = new System.Windows.Forms.Button();
            this.btn_AlarmRST = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1236, 791);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.inspector1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1228, 758);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1062, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 83);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // inspector1
            // 
            this.inspector1.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.inspector1.Location = new System.Drawing.Point(58, 7);
            this.inspector1.Margin = new System.Windows.Forms.Padding(5);
            this.inspector1.Name = "inspector1";
            this.inspector1.Size = new System.Drawing.Size(1112, 744);
            this.inspector1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_AlarmRST);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btn_Connect);
            this.tabPage2.Controls.Add(this.btn_DeviceCreate);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1228, 758);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(229, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(33, 88);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(165, 39);
            this.btn_Connect.TabIndex = 1;
            this.btn_Connect.Text = "btn_Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // btn_DeviceCreate
            // 
            this.btn_DeviceCreate.Location = new System.Drawing.Point(33, 27);
            this.btn_DeviceCreate.Name = "btn_DeviceCreate";
            this.btn_DeviceCreate.Size = new System.Drawing.Size(165, 39);
            this.btn_DeviceCreate.TabIndex = 0;
            this.btn_DeviceCreate.Text = "btn_DeviceCreate";
            this.btn_DeviceCreate.UseVisualStyleBackColor = true;
            this.btn_DeviceCreate.Click += new System.EventHandler(this.btn_DeviceCreate_Click);
            // 
            // btn_AlarmRST
            // 
            this.btn_AlarmRST.Location = new System.Drawing.Point(33, 150);
            this.btn_AlarmRST.Name = "btn_AlarmRST";
            this.btn_AlarmRST.Size = new System.Drawing.Size(165, 39);
            this.btn_AlarmRST.TabIndex = 3;
            this.btn_AlarmRST.Text = "btn_AlarmRST";
            this.btn_AlarmRST.UseVisualStyleBackColor = true;
            this.btn_AlarmRST.Click += new System.EventHandler(this.btn_AlarmRST_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1260, 815);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button1;
        private Inspector.Inspector inspector1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Button btn_DeviceCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_AlarmRST;
    }
}

