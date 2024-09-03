namespace InjectorInspector
{
    partial class VisionTestFrm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.inspector1 = new Inspector.Inspector();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(1101, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 54);
            this.button1.TabIndex = 1;
            this.button1.Text = "分析吸嘴影像";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.分析吸嘴影像_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(1101, 96);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 54);
            this.button2.TabIndex = 2;
            this.button2.Text = "分析Tray盤影像";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.分析Tray盤影像_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(1100, 36);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(152, 54);
            this.button3.TabIndex = 3;
            this.button3.Text = "分析入料盤影像";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.分析入料盤影像_Click);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(1259, 96);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(152, 54);
            this.button4.TabIndex = 4;
            this.button4.Text = "Setup TrayCCD";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.SetupCCDTray_Click);
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(1259, 36);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(152, 54);
            this.button5.TabIndex = 5;
            this.button5.Text = "Setup 入料CCD";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.SetupCCD入料_Click);
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(1101, 406);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(152, 54);
            this.button6.TabIndex = 6;
            this.button6.Text = "批次分析入料盤";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.批次分析入料盤_Click);
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(1101, 466);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(152, 54);
            this.button7.TabIndex = 7;
            this.button7.Text = "批次分析Tray盤";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.批次分析Tray盤_Click);
            // 
            // button8
            // 
            this.button8.Enabled = false;
            this.button8.Location = new System.Drawing.Point(1101, 526);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(152, 54);
            this.button8.TabIndex = 8;
            this.button8.Text = "批次分析吸嘴";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.批次分析吸嘴_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(340, 331);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(670, 38);
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // inspector1
            // 
            this.inspector1.Location = new System.Drawing.Point(14, 14);
            this.inspector1.Margin = new System.Windows.Forms.Padding(5);
            this.inspector1.Name = "inspector1";
            this.inspector1.Size = new System.Drawing.Size(1078, 717);
            this.inspector1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1423, 807);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.inspector1);
            this.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Inspector.Inspector inspector1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

