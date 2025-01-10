using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using netDxf;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;

namespace InjectorInspector
{
    internal static class Viewer
    {
        private static OpenFileDialog OpenDxfFileDialog = new OpenFileDialog();
        private static SaveFileDialog SaveJsonFileDialog = new SaveFileDialog();

        public static DxfDocument DxfDoc = new DxfDocument();
        public static JSON Json = new JSON(); // 底下 JSON 不寫成靜態, HighlightedCircle, FocusedCircle會用到

        /// <summary>
        /// 讀取 DXF 後儲存到這個物件, 後面會存成 JSON 檔
        /// </summary>
        public class JSON
        {
            public List<Circle> Circles { get; set; }

            public class Circle
            {
                public int Index { get; set; }
                public string Name { get; set; }
                public string Id { get; set; }
                public double X { get; set; }
                public double Y { get; set; }
                public double Diameter { get; set; }
                public bool Place { get; set; }
                public bool Remove { get; set; }
                public bool Replace { get; set; }
                public bool Display { get; set; }
                public bool Enable { get; set; }
                public string Reserve1 { get; set; }
                public string Reserve2 { get; set; }
                public string Reserve3 { get; set; }
                public string Reserve4 { get; set; }
                public string Reserve5 { get; set; }
            }

            public JSON()
            {
                Circles = new List<Circle>();
            }
        }

        /// <summary>
        /// 計算 dxf 檔所有圓的邊界
        /// </summary>
        /// <param name="dxfJson">已讀取的 JSON 資料</param>
        /// <param name="minX">左邊界</param>
        /// <param name="minY">上邊界</param>
        /// <param name="maxX">右邊界</param>
        /// <param name="maxY">下邊界</param>
        /// <param name="width">邊界寬度</param>
        /// <param name="height">邊界高度</param>
        public static void find_Boundary(JSON dxfJson, out double minX, out double minY, out double maxX, out double maxY, out double width, out double height)
        {
            // 初始化邊界
            minX = double.MaxValue;
            minY = double.MaxValue;
            maxX = double.MinValue;
            maxY = double.MinValue;

            // 遍歷所有圓，更新邊界
            foreach (var circle in dxfJson.Circles)
            {
                minX = Math.Min(minX, circle.X - circle.Diameter / 2);
                minY = Math.Min(minY, circle.Y - circle.Diameter / 2);
                maxX = Math.Max(maxX, circle.X + circle.Diameter / 2);
                maxY = Math.Max(maxY, circle.Y + circle.Diameter / 2);
            }

            width = maxX - minX;
            height = maxY - minY;
        }

        /// <summary>
        /// 將 DXF 檔中的資料轉成 JSON
        /// </summary>
        /// <param name="DxfDoc">要顯示的 DXF 檔案</param>
        /// <param name="json">已轉換的 JSON 文件</param>
        public static void TransformDxf2Json(DxfDocument DxfDoc, ref JSON json)
        {
            int index = 0;

            json.Circles.Clear(); // 清除上一次 load 的資料

            foreach (var circle in DxfDoc.Entities.Circles)
            {
                json.Circles.Add(new JSON.Circle
                {
                    Index = index,
                    X = circle.Center.X,
                    Y = circle.Center.Y,
                    Diameter = circle.Radius * 2,

                    Place = false,
                    Remove = false,
                    Replace = false,
                    Display = true, // 顯示預設為 true
                    Enable = false
                });

                index++;
            }
        }

        /// <summary>
        /// 從新排序座標由左至右、由上至下
        /// </summary>
        /// <param name="json">已讀取的 JSON 資料</param>
        public static void ResortPosition(ref JSON json)
        {
            JSON resortedJson = new JSON();

            var resortedIndex = new (double XaddY, int fakeIndex)[json.Circles.Count];

            for (int i = 0; i < json.Circles.Count; i++)
            {
                resortedIndex[i] = (json.Circles[i].X - json.Circles[i].Y * 10000, json.Circles[i].Index);
            }

            Array.Sort(resortedIndex, (prev, next) => prev.XaddY.CompareTo(next.XaddY)); // 由小排到大

            for (int i = 0; i < json.Circles.Count; i++)
            {
                resortedJson.Circles.Add(json.Circles[resortedIndex[i].fakeIndex]);
                resortedJson.Circles[i].Index = i;
            }

            json = resortedJson;
        }

        /// <summary>
        /// 打開 DXF 或者 JSON 檔案
        /// </summary>
        public static bool OpenFile()
        {
            OpenDxfFileDialog.Filter = "JSON Files (*.json)|*.json|DXF Files (*.dxf)|*.dxf";

            if (OpenDxfFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (OpenDxfFileDialog.FilterIndex == 2) // 如果選擇 .dxf
                {
                    try
                    {
                        DxfDoc = DxfDocument.Load(OpenDxfFileDialog.FileName);

                        if (DxfDoc.Entities.Circles.Count() > 0)
                        {
                            MessageBox.Show($"檔案 {OpenDxfFileDialog.FileName} 成功讀取！");

                            TransformDxf2Json(DxfDoc, ref Json);
                            ResortPosition(ref Json);
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("此 DXF 檔案沒有圓形！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"讀取 DXF 檔時發生錯誤: {ex.Message}");
                        return false;
                    }
                }
                else if (OpenDxfFileDialog.FilterIndex == 1) // 如果選擇 .json
                {
                    try
                    {
                        Json = JsonConvert.DeserializeObject<JSON>(File.ReadAllText(OpenDxfFileDialog.FileName));
                        MessageBox.Show($"檔案 {OpenDxfFileDialog.FileName} 成功讀取！");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"讀取 Json 檔時發生錯誤: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 儲存 JSON 檔案
        /// </summary>
        public static void SaveFile()
        {
            SaveJsonFileDialog.Filter = "Json Files (*.json)|*.json";

            if (SaveJsonFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 使用 Newtonsoft.Json 進行物件序列化，並設定格式化輸出（會縮排顯示）
                string json = JsonConvert.SerializeObject(Json, Newtonsoft.Json.Formatting.Indented);

                // 使用 StreamWriter 儲存 Json 到選定的檔案
                using (StreamWriter writer = new StreamWriter(SaveJsonFileDialog.FileName))
                {
                    writer.Write(json);
                }

                MessageBox.Show("檔案儲存成功！", "儲存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            
                            case "chk_Display":
                                checkBox.Checked = focusedCircle.Display;
                                break;
                            case "chk_Enable":
                                checkBox.Checked = focusedCircle.Enable;
                                break;

                        }

                        break;

                    case RadioButton radioButton:

                        switch (radioButton.Name)
                        {
                            case "rad_Place":
                                radioButton.Checked = focusedCircle.Place;
                                break;
                            case "rad_Remove":
                                radioButton.Checked = focusedCircle.Remove;
                                break;
                            case "rad_Replace":
                                radioButton.Checked = focusedCircle.Replace;
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

                    case RadioButton radioButton:
                        radioButton.Checked = false;
                        break;
                }
            }
        }
    }
}
