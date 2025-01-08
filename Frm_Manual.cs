using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InjectorInspector;

namespace NeedleManual
{
    public partial class Frm_Manual : Form
    {
        public Frm_Manual()
        {
            InitializeComponent();
            this.TopLevel = true;
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (txt_Distance.Text == "" )
            {
                MessageBox.Show("請輸入距離");
                return;
            }
            else if (cmb_Nozzle.SelectedIndex == -1)
            {
                MessageBox.Show("請選擇移動軸");
                return;
            }
            dgv_ManualList.Rows.Add(cmb_Nozzle.Text, Convert.ToDouble(txt_Distance.Text), txt_Comment.Text);
            txt_Distance.Text = "";
            txt_Comment.Text = "";
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                dgv_ManualList.Rows.Remove(dgv_ManualList.CurrentRow);
            }
            catch (Exception)
            {
                MessageBox.Show("請選擇要刪除的項目");
            }
        }

        private void txt_Distance_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 檢查是否為數字、小數點或控制鍵（如Backspace）
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true; // 阻止非數字和小數點的輸入
            }

            // 確保小數點只能輸入一次
            if (e.KeyChar == '.' && txt_Distance.Text.Contains("."))
            {
                e.Handled = true; // 阻止重複輸入小數點  這邊不用寫這麼聰明沒關係 因為只有自己用
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();

            switch (dgv_ManualList.CurrentRow.Cells[0].Value.ToString())
            {
                case "NozzleX":
                    form1.dbapiNozzleX(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 50);
                    break;

                case "NozzleY":
                    form1.dbapiNozzleY(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 50);
                    break;

                case "NozzleZ":
                    form1.dbapiNozzleZ(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 50);
                    break;

                case "NozzleR":
                    form1.dbapiNozzleR(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 360);
                    break;

                case "CarrierX":
                    form1.dbapiCarrierX(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 50);
                    break;

                case "CarrierY":
                    form1.dbapiCarrierY(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 50);
                    break;

                case "SetZ":
                    form1.dbapiSetZ(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 33);
                    break;

                case "SetR":
                    form1.dbapiSetR(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 360);
                    break;

                case "Gate":
                    form1.dbapiGate(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value), 100);
                    break;

                case "IAI":
                    form1.dbapiIAI(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value));
                    break;

                case "3D掃描":
                    form1.dbapJoDell3D掃描(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value));
                    break;

                case "吸針嘴":
                    form1.dbapJoDell吸針嘴(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value));
                    break;

                case "植針嘴":
                    form1.dbapJoDell植針嘴(Convert.ToDouble(dgv_ManualList.CurrentRow.Cells[1].Value));
                    break;
            }
        }

        private void Frm_Manual_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();  // 隱藏 Form2 而不是銷毀它
        }

        private void Frm_Manual_Load(object sender, EventArgs e)
        {
            dgv_ManualList.Rows.Add("SetR", 178.08, "植針位");
            dgv_ManualList.Rows.Add("SetR", 268.08, "放料位");
            dgv_ManualList.Rows.Add("植針嘴", 41, "載盤高度");
            dgv_ManualList.Rows.Add("IAI", 22, "載盤高度");
        }
    }
}
