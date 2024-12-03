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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.inspector1 = new Inspector.Inspector();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnVibrationLEDOff = new System.Windows.Forms.Button();
            this.btnVibrationLED = new System.Windows.Forms.Button();
            this.btnVibrationStop = new System.Windows.Forms.Button();
            this.btnVibrationInit = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPosition01 = new System.Windows.Forms.Button();
            this.AcSpd1 = new System.Windows.Forms.Label();
            this.AcSpd0 = new System.Windows.Forms.Label();
            this.AcPos1 = new System.Windows.Forms.Label();
            this.AcPos0 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSetHome = new System.Windows.Forms.Button();
            this.btn_Off1 = new System.Windows.Forms.Button();
            this.btn_Off0 = new System.Windows.Forms.Button();
            this.btn_On1 = new System.Windows.Forms.Button();
            this.btn_On0 = new System.Windows.Forms.Button();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_AlarmRST = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtDeg = new System.Windows.Forms.TextBox();
            this.btnChgDeg = new System.Windows.Forms.Button();
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
            this.tabPage2.Controls.Add(this.btnChgDeg);
            this.tabPage2.Controls.Add(this.txtDeg);
            this.tabPage2.Controls.Add(this.btnVibrationLEDOff);
            this.tabPage2.Controls.Add(this.btnVibrationLED);
            this.tabPage2.Controls.Add(this.btnVibrationStop);
            this.tabPage2.Controls.Add(this.btnVibrationInit);
            this.tabPage2.Controls.Add(this.btnStop);
            this.tabPage2.Controls.Add(this.btnPosition01);
            this.tabPage2.Controls.Add(this.AcSpd1);
            this.tabPage2.Controls.Add(this.AcSpd0);
            this.tabPage2.Controls.Add(this.AcPos1);
            this.tabPage2.Controls.Add(this.AcPos0);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btnSetHome);
            this.tabPage2.Controls.Add(this.btn_Off1);
            this.tabPage2.Controls.Add(this.btn_Off0);
            this.tabPage2.Controls.Add(this.btn_On1);
            this.tabPage2.Controls.Add(this.btn_On0);
            this.tabPage2.Controls.Add(this.btn_Disconnect);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.btn_AlarmRST);
            this.tabPage2.Controls.Add(this.btn_Connect);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1228, 758);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnVibrationLEDOff
            // 
            this.btnVibrationLEDOff.Location = new System.Drawing.Point(946, 173);
            this.btnVibrationLEDOff.Name = "btnVibrationLEDOff";
            this.btnVibrationLEDOff.Size = new System.Drawing.Size(172, 39);
            this.btnVibrationLEDOff.TabIndex = 25;
            this.btnVibrationLEDOff.Text = "btnVibrationLEDOff";
            this.btnVibrationLEDOff.UseVisualStyleBackColor = true;
            this.btnVibrationLEDOff.Click += new System.EventHandler(this.btnVibrationLEDOff_Click);
            // 
            // btnVibrationLED
            // 
            this.btnVibrationLED.Location = new System.Drawing.Point(750, 173);
            this.btnVibrationLED.Name = "btnVibrationLED";
            this.btnVibrationLED.Size = new System.Drawing.Size(165, 39);
            this.btnVibrationLED.TabIndex = 24;
            this.btnVibrationLED.Text = "btnVibrationLED";
            this.btnVibrationLED.UseVisualStyleBackColor = true;
            this.btnVibrationLED.Click += new System.EventHandler(this.btnVibrationLED_Click);
            // 
            // btnVibrationStop
            // 
            this.btnVibrationStop.Location = new System.Drawing.Point(946, 109);
            this.btnVibrationStop.Name = "btnVibrationStop";
            this.btnVibrationStop.Size = new System.Drawing.Size(165, 39);
            this.btnVibrationStop.TabIndex = 23;
            this.btnVibrationStop.Text = "btnVibrationStop";
            this.btnVibrationStop.UseVisualStyleBackColor = true;
            this.btnVibrationStop.Click += new System.EventHandler(this.btnVibrationStop_Click);
            // 
            // btnVibrationInit
            // 
            this.btnVibrationInit.Location = new System.Drawing.Point(750, 109);
            this.btnVibrationInit.Name = "btnVibrationInit";
            this.btnVibrationInit.Size = new System.Drawing.Size(165, 39);
            this.btnVibrationInit.TabIndex = 22;
            this.btnVibrationInit.Text = "btnVibrationInit";
            this.btnVibrationInit.UseVisualStyleBackColor = true;
            this.btnVibrationInit.Click += new System.EventHandler(this.btnVibrationInit_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(403, 211);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(165, 39);
            this.btnStop.TabIndex = 21;
            this.btnStop.Text = "btnStop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPosition01
            // 
            this.btnPosition01.Location = new System.Drawing.Point(403, 150);
            this.btnPosition01.Name = "btnPosition01";
            this.btnPosition01.Size = new System.Drawing.Size(165, 39);
            this.btnPosition01.TabIndex = 20;
            this.btnPosition01.Text = "btnPosition01";
            this.btnPosition01.UseVisualStyleBackColor = true;
            this.btnPosition01.Click += new System.EventHandler(this.btnPosition01_Click);
            // 
            // AcSpd1
            // 
            this.AcSpd1.AutoSize = true;
            this.AcSpd1.Location = new System.Drawing.Point(316, 502);
            this.AcSpd1.Name = "AcSpd1";
            this.AcSpd1.Size = new System.Drawing.Size(67, 19);
            this.AcSpd1.TabIndex = 19;
            this.AcSpd1.Text = "AcSpd1";
            // 
            // AcSpd0
            // 
            this.AcSpd0.AutoSize = true;
            this.AcSpd0.Location = new System.Drawing.Point(126, 502);
            this.AcSpd0.Name = "AcSpd0";
            this.AcSpd0.Size = new System.Drawing.Size(67, 19);
            this.AcSpd0.TabIndex = 18;
            this.AcSpd0.Text = "AcSpd0";
            // 
            // AcPos1
            // 
            this.AcPos1.AutoSize = true;
            this.AcPos1.Location = new System.Drawing.Point(316, 468);
            this.AcPos1.Name = "AcPos1";
            this.AcPos1.Size = new System.Drawing.Size(65, 19);
            this.AcPos1.TabIndex = 17;
            this.AcPos1.Text = "AcPos1";
            // 
            // AcPos0
            // 
            this.AcPos0.AutoSize = true;
            this.AcPos0.Location = new System.Drawing.Point(126, 468);
            this.AcPos0.Name = "AcPos0";
            this.AcPos0.Size = new System.Drawing.Size(65, 19);
            this.AcPos0.TabIndex = 16;
            this.AcPos0.Text = "AcPos0";
            this.AcPos0.Click += new System.EventHandler(this.AcPos0_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(515, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 19);
            this.label1.TabIndex = 15;
            this.label1.Text = "label1";
            // 
            // btnSetHome
            // 
            this.btnSetHome.Location = new System.Drawing.Point(162, 264);
            this.btnSetHome.Name = "btnSetHome";
            this.btnSetHome.Size = new System.Drawing.Size(165, 39);
            this.btnSetHome.TabIndex = 14;
            this.btnSetHome.Text = "btnSetHome";
            this.btnSetHome.UseVisualStyleBackColor = true;
            this.btnSetHome.Click += new System.EventHandler(this.btnSetHome_Click);
            // 
            // btn_Off1
            // 
            this.btn_Off1.Location = new System.Drawing.Point(262, 399);
            this.btn_Off1.Name = "btn_Off1";
            this.btn_Off1.Size = new System.Drawing.Size(165, 39);
            this.btn_Off1.TabIndex = 13;
            this.btn_Off1.Text = "btn_Off1";
            this.btn_Off1.UseVisualStyleBackColor = true;
            this.btn_Off1.Click += new System.EventHandler(this.btn_Off1_Click);
            // 
            // btn_Off0
            // 
            this.btn_Off0.Location = new System.Drawing.Point(67, 399);
            this.btn_Off0.Name = "btn_Off0";
            this.btn_Off0.Size = new System.Drawing.Size(165, 39);
            this.btn_Off0.TabIndex = 12;
            this.btn_Off0.Text = "btn_Off0";
            this.btn_Off0.UseVisualStyleBackColor = true;
            this.btn_Off0.Click += new System.EventHandler(this.btn_Off0_Click);
            // 
            // btn_On1
            // 
            this.btn_On1.Location = new System.Drawing.Point(262, 333);
            this.btn_On1.Name = "btn_On1";
            this.btn_On1.Size = new System.Drawing.Size(165, 39);
            this.btn_On1.TabIndex = 11;
            this.btn_On1.Text = "btn_On1";
            this.btn_On1.UseVisualStyleBackColor = true;
            this.btn_On1.Click += new System.EventHandler(this.btn_On1_Click);
            // 
            // btn_On0
            // 
            this.btn_On0.Location = new System.Drawing.Point(67, 333);
            this.btn_On0.Name = "btn_On0";
            this.btn_On0.Size = new System.Drawing.Size(165, 39);
            this.btn_On0.TabIndex = 10;
            this.btn_On0.Text = "btn_On0";
            this.btn_On0.UseVisualStyleBackColor = true;
            this.btn_On0.Click += new System.EventHandler(this.btn_On0_Click);
            // 
            // btn_Disconnect
            // 
            this.btn_Disconnect.Location = new System.Drawing.Point(33, 121);
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.Size = new System.Drawing.Size(165, 39);
            this.btn_Disconnect.TabIndex = 9;
            this.btn_Disconnect.Text = "btn_Disconnect";
            this.btn_Disconnect.UseVisualStyleBackColor = true;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(593, 613);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 19);
            this.label5.TabIndex = 8;
            this.label5.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(593, 579);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(596, 547);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "label3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(593, 515);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(571, 449);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(165, 38);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btn_AlarmRST
            // 
            this.btn_AlarmRST.Location = new System.Drawing.Point(232, 54);
            this.btn_AlarmRST.Name = "btn_AlarmRST";
            this.btn_AlarmRST.Size = new System.Drawing.Size(165, 39);
            this.btn_AlarmRST.TabIndex = 3;
            this.btn_AlarmRST.Text = "btn_AlarmRST";
            this.btn_AlarmRST.UseVisualStyleBackColor = true;
            this.btn_AlarmRST.Click += new System.EventHandler(this.btn_AlarmRST_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(33, 54);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(165, 39);
            this.btn_Connect.TabIndex = 1;
            this.btn_Connect.Text = "btn_Connect";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtDeg
            // 
            this.txtDeg.Location = new System.Drawing.Point(67, 612);
            this.txtDeg.Name = "txtDeg";
            this.txtDeg.Size = new System.Drawing.Size(100, 30);
            this.txtDeg.TabIndex = 26;
            this.txtDeg.Text = "-0.00";
            // 
            // btnChgDeg
            // 
            this.btnChgDeg.Location = new System.Drawing.Point(67, 559);
            this.btnChgDeg.Name = "btnChgDeg";
            this.btnChgDeg.Size = new System.Drawing.Size(165, 39);
            this.btnChgDeg.TabIndex = 27;
            this.btnChgDeg.Text = "btnChgDeg";
            this.btnChgDeg.UseVisualStyleBackColor = true;
            this.btnChgDeg.Click += new System.EventHandler(this.btnChgDeg_Click);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_AlarmRST;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.Button btn_Off1;
        private System.Windows.Forms.Button btn_Off0;
        private System.Windows.Forms.Button btn_On1;
        private System.Windows.Forms.Button btn_On0;
        private System.Windows.Forms.Button btnSetHome;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label AcPos1;
        private System.Windows.Forms.Label AcPos0;
        private System.Windows.Forms.Label AcSpd1;
        private System.Windows.Forms.Label AcSpd0;
        private System.Windows.Forms.Button btnPosition01;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnVibrationInit;
        private System.Windows.Forms.Button btnVibrationLEDOff;
        private System.Windows.Forms.Button btnVibrationLED;
        private System.Windows.Forms.Button btnVibrationStop;
        private System.Windows.Forms.Button btnChgDeg;
        private System.Windows.Forms.TextBox txtDeg;
    }
}

