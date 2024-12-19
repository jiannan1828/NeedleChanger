using System;
using System.Windows.Forms;

using static NeedleViewer.DataManager;

namespace NeedleViewer
{
    internal static class UI
    {
        /// <summary>
        /// 在 DataGridView 顯示 DXF 資料
        /// </summary>
        /// <param name="dgv_Needles">要顯示 DXF 檔的 DataGridView</param>
        /// <param name="dxfJson">已讀取的 DXF 文件</param>
        /// <returns>無回傳值</returns>
        public static void show_dgv_Needles(DataGridView dgv_Needles, DataManager.JSON dxfJson)
        {
            dgv_Needles.Rows.Clear();

            foreach (var Circle in dxfJson.Circles)
            {
                dgv_Needles.Rows.Add(
                    Circle.Index,
                    Circle.Name,
                    Circle.Id,
                    Circle.X,
                    Circle.Y,
                    Circle.Diameter,
                    Circle.Place,
                    Circle.Remove,
                    Circle.Replace,
                    Circle.Display,
                    Circle.Enable
                );

            }
        }

        /// <summary>
        /// 在 groupbox 中顯示植針資訊
        /// </summary>
        /// <param name="grp_NeedleInfo">植針資訊的 Groupbox</param>
        /// <param name="focusedCircle">在 picturebox 上按下的圓</param>
        /// <returns>無回傳值</returns>
        public static void show_grp_NeedleInfo(GroupBox grp_NeedleInfo, JSON.Circle focusedCircle)
        {
            foreach (Control control in grp_NeedleInfo.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:

                        switch (textBox.Name)
                        {
                            case "txt_Index":
                                textBox.Text = (focusedCircle.Index).ToString();
                                break;
                            case "txt_Name":
                                textBox.Text = focusedCircle.Name;
                                break;
                            case "txt_Id":
                                textBox.Text = focusedCircle.Id;
                                break;
                            case "txt_PosX":
                                textBox.Text = (focusedCircle.X).ToString();
                                break;
                            case "txt_PosY":
                                textBox.Text = (focusedCircle.Y).ToString();
                                break;
                            case "txt_Diameter":
                                textBox.Text = (focusedCircle.Diameter).ToString();
                                break;
                        }

                        break;

                    case CheckBox checkBox:

                        switch (checkBox.Name)
                        {
                            case "chk_Place":
                                checkBox.Checked = focusedCircle.Place;
                                break;
                            case "chk_Remove":
                                checkBox.Checked = focusedCircle.Remove;
                                break;
                            case "chk_Replace":
                                checkBox.Checked = focusedCircle.Replace;
                                break;
                            case "chk_Display":
                                checkBox.Checked = focusedCircle.Display;
                                break;
                            case "chk_Enable":
                                checkBox.Checked = focusedCircle.Enable;
                                break;

                        }

                        break;
                }
            }
        }

        /// <summary>
        /// 清空 groupbox 內的資訊
        /// </summary>
        /// <param name="grp_NeedleInfo">植針資訊的 Groupbox</param>
        /// <returns>無回傳值</returns>
        public static void clear_grp_NeedleInfo(GroupBox grp_NeedleInfo)
        {
            foreach (Control control in grp_NeedleInfo.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.Clear();
                        break;

                    case CheckBox checkBox:
                        checkBox.Checked = false;
                        break;
                }
            }
        }

        /// <summary>
        /// 排序過的資料 Index 未必會按到由上到下排序, 重新遍歷他的流水號
        /// </summary>
        /// <param name="dgv_Needles">植針 DataGridView</param>
        /// <param name="Index">由 picturebox 傳進來的 Index DataGridView</param>
        /// <returns>無回傳值</returns>
        public static int find_dgv_Needles_Index(DataGridView dgv_Needles, int Index)
        {
            for (int i = 0; i < dgv_Needles.Rows.Count; i++)
            {
                if (Index == (int)dgv_Needles.Rows[i].Cells[0].Value)
                {
                    return i;
                }
            }

            return -1;
        }

        
    }
}
