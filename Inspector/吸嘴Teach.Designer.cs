namespace Inspector
{
    partial class 吸嘴Teach
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hWin = new HalconDotNet.HWindowControl();
            this.btn_建立特徵 = new System.Windows.Forms.Button();
            this.btn_分析 = new System.Windows.Forms.Button();
            this.btn_Accept = new System.Windows.Forms.Button();
            this.ck_特徵為針頭 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // hWin
            // 
            this.hWin.BackColor = System.Drawing.Color.Black;
            this.hWin.BorderColor = System.Drawing.Color.Black;
            this.hWin.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWin.Location = new System.Drawing.Point(12, 12);
            this.hWin.Name = "hWin";
            this.hWin.Size = new System.Drawing.Size(640, 459);
            this.hWin.TabIndex = 8;
            this.hWin.WindowSize = new System.Drawing.Size(640, 459);
            // 
            // btn_建立特徵
            // 
            this.btn_建立特徵.Location = new System.Drawing.Point(12, 477);
            this.btn_建立特徵.Name = "btn_建立特徵";
            this.btn_建立特徵.Size = new System.Drawing.Size(118, 51);
            this.btn_建立特徵.TabIndex = 9;
            this.btn_建立特徵.Text = "建立特徵";
            this.btn_建立特徵.UseVisualStyleBackColor = true;
            this.btn_建立特徵.Click += new System.EventHandler(this.btn_建立特徵_Click);
            // 
            // btn_分析
            // 
            this.btn_分析.Location = new System.Drawing.Point(136, 477);
            this.btn_分析.Name = "btn_分析";
            this.btn_分析.Size = new System.Drawing.Size(118, 51);
            this.btn_分析.TabIndex = 10;
            this.btn_分析.Text = "分析";
            this.btn_分析.UseVisualStyleBackColor = true;
            this.btn_分析.Click += new System.EventHandler(this.btn_分析_Click);
            // 
            // btn_Accept
            // 
            this.btn_Accept.Location = new System.Drawing.Point(532, 477);
            this.btn_Accept.Name = "btn_Accept";
            this.btn_Accept.Size = new System.Drawing.Size(118, 51);
            this.btn_Accept.TabIndex = 11;
            this.btn_Accept.Text = "Accept";
            this.btn_Accept.UseVisualStyleBackColor = true;
            this.btn_Accept.Click += new System.EventHandler(this.btn_Accept_Click);
            // 
            // ck_特徵為針頭
            // 
            this.ck_特徵為針頭.AutoSize = true;
            this.ck_特徵為針頭.Location = new System.Drawing.Point(260, 477);
            this.ck_特徵為針頭.Name = "ck_特徵為針頭";
            this.ck_特徵為針頭.Size = new System.Drawing.Size(123, 23);
            this.ck_特徵為針頭.TabIndex = 12;
            this.ck_特徵為針頭.Text = "特徵為針頭";
            this.ck_特徵為針頭.UseVisualStyleBackColor = true;
            // 
            // 吸嘴Teach
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(662, 554);
            this.Controls.Add(this.ck_特徵為針頭);
            this.Controls.Add(this.btn_Accept);
            this.Controls.Add(this.btn_分析);
            this.Controls.Add(this.btn_建立特徵);
            this.Controls.Add(this.hWin);
            this.Font = new System.Drawing.Font("新細明體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "吸嘴Teach";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "吸嘴Teach";
            this.Load += new System.EventHandler(this.吸嘴Teach_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HalconDotNet.HWindowControl hWin;
        private System.Windows.Forms.Button btn_建立特徵;
        private System.Windows.Forms.Button btn_分析;
        private System.Windows.Forms.Button btn_Accept;
        private System.Windows.Forms.CheckBox ck_特徵為針頭;
    }
}