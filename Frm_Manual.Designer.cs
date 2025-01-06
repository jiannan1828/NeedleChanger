using System.Windows.Forms;

namespace NeedleManual
{
    partial class Frm_Manual
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
            this.cmb_Nozzle = new System.Windows.Forms.ComboBox();
            this.txt_Distance = new System.Windows.Forms.TextBox();
            this.txt_Comment = new System.Windows.Forms.TextBox();
            this.btn_add = new System.Windows.Forms.Button();
            this.dgv_ManualList = new System.Windows.Forms.DataGridView();
            this.col_Nozzle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Distance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Run = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ManualList)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_Nozzle
            // 
            this.cmb_Nozzle.FormattingEnabled = true;
            this.cmb_Nozzle.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
            this.cmb_Nozzle.Location = new System.Drawing.Point(12, 12);
            this.cmb_Nozzle.Name = "cmb_Nozzle";
            this.cmb_Nozzle.Size = new System.Drawing.Size(122, 30);
            this.cmb_Nozzle.TabIndex = 0;
            // 
            // txt_Distance
            // 
            this.txt_Distance.Location = new System.Drawing.Point(140, 12);
            this.txt_Distance.Name = "txt_Distance";
            this.txt_Distance.Size = new System.Drawing.Size(172, 33);
            this.txt_Distance.TabIndex = 1;
            this.txt_Distance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_Distance_KeyPress);
            // 
            // txt_Comment
            // 
            this.txt_Comment.Location = new System.Drawing.Point(318, 12);
            this.txt_Comment.Name = "txt_Comment";
            this.txt_Comment.Size = new System.Drawing.Size(172, 33);
            this.txt_Comment.TabIndex = 2;
            // 
            // btn_add
            // 
            this.btn_add.Location = new System.Drawing.Point(496, 12);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(108, 40);
            this.btn_add.TabIndex = 3;
            this.btn_add.Text = "加入";
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // dgv_ManualList
            // 
            this.dgv_ManualList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ManualList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Nozzle,
            this.col_Distance,
            this.col_Comment});
            this.dgv_ManualList.Location = new System.Drawing.Point(12, 58);
            this.dgv_ManualList.Name = "dgv_ManualList";
            this.dgv_ManualList.RowHeadersWidth = 51;
            this.dgv_ManualList.RowTemplate.Height = 27;
            this.dgv_ManualList.Size = new System.Drawing.Size(592, 337);
            this.dgv_ManualList.TabIndex = 4;
            // 
            // col_Nozzle
            // 
            this.col_Nozzle.HeaderText = "移動軸";
            this.col_Nozzle.MinimumWidth = 6;
            this.col_Nozzle.Name = "col_Nozzle";
            this.col_Nozzle.Width = 125;
            // 
            // col_Distance
            // 
            this.col_Distance.HeaderText = "移動距離";
            this.col_Distance.MinimumWidth = 6;
            this.col_Distance.Name = "col_Distance";
            this.col_Distance.Width = 175;
            // 
            // col_Comment
            // 
            this.col_Comment.HeaderText = "備註";
            this.col_Comment.MinimumWidth = 6;
            this.col_Comment.Name = "col_Comment";
            this.col_Comment.Width = 250;
            // 
            // btn_Run
            // 
            this.btn_Run.Location = new System.Drawing.Point(12, 402);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(200, 54);
            this.btn_Run.TabIndex = 5;
            this.btn_Run.Text = "移動";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(218, 402);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 54);
            this.button1.TabIndex = 6;
            this.button1.Text = "返回上一動";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(424, 402);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(180, 54);
            this.btn_Delete.TabIndex = 7;
            this.btn_Delete.Text = "刪除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // Frm_Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 468);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Run);
            this.Controls.Add(this.dgv_ManualList);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.txt_Comment);
            this.Controls.Add(this.txt_Distance);
            this.Controls.Add(this.cmb_Nozzle);
            this.Font = new System.Drawing.Font("標楷體", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Frm_Manual";
            this.Text = "手動介面";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ManualList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private ComboBox cmb_Nozzle;
        private TextBox txt_Distance;
        private TextBox txt_Comment;
        private Button btn_add;
        private DataGridView dgv_ManualList;
        private Button btn_Run;
        private Button button1;
        private DataGridViewTextBoxColumn col_Nozzle;
        private DataGridViewTextBoxColumn col_Distance;
        private DataGridViewTextBoxColumn col_Comment;
        private Button btn_Delete;
    }
}

