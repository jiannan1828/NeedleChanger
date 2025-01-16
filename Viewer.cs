using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using netDxf;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;
using System.Drawing;

namespace InjectorInspector
{
    internal static class Viewer
    {
        private static OpenFileDialog OpenDxfFileDialog = new OpenFileDialog();
        private static SaveFileDialog SaveJsonFileDialog = new SaveFileDialog();

        public static DxfDocument DxfDoc = new DxfDocument();
        public static JSON Json = new JSON(); // 底下 JSON 不寫成靜態, HighlightedCircle, FocusedCircle會用到

        public static JSON.Circle HighlightedCircle = null;
        public static JSON.Circle FocusedCircle = null;
        public static List<JSON.Circle> SelectedCircles = new List<JSON.Circle>();
        public static List<JSON.Circle> PlacedCircles = new List<JSON.Circle>();

        public static readonly Color DefaltCircleColor = Color.ForestGreen;
        public static readonly Color HiddenCirclesColor = Color.FromArgb(64, Color.ForestGreen);
        public static readonly Color FocusedCircleColor = Color.Goldenrod;
        public static readonly Color SelectedCirclesColor = Color.MediumVioletRed;
        public static readonly Color PlaceCirclesColor = Color.Red;

        public static Boundary Json_Boundary = new Boundary();
        public static Boundary Drag_Boundary = new Boundary();

        public const float ScaleFactor = 10;
        public static float ZoomFactor = 1;

        public static PointF Offset = new PointF(0, 0);
        public static PointF PrevMousePos = new PointF(0, 0);
        public static PointF RealMousePos = new PointF(0, 0);
        public static PointF RealMousePosBeforeZoom = new PointF(0, 0);
        public static PointF RealMousePosAfterZoom = new PointF(0, 0);

        public static double Mouse2CircleDistance;
        public static bool IsMouseinCircle;

        public static bool IsDrag = false;
        public static PointF Drag_Start = new PointF(0, 0);
        public static PointF Drag_End = new PointF(0, 0);

        public static string[] 跑馬燈文字 = {
            "待機",
            "運行中",
            "例外狀況"
        };
        public static int 跑馬燈文字Index = 0;
        public static int 跑馬燈X座標 = 0;

        /// <summary>
        /// 讀取 DXF 後儲存到這個物件, 後面會存成 JSON 檔
        /// </summary>
        public class JSON
        {
            public List<Circle> Circles { get; set; }

            public class Circle
            {
                public int    Index    { get; set; }
                public string Name     { get; set; }
                public string Id       { get; set; }
                public double X        { get; set; }
                public double Y        { get; set; }
                public double Diameter { get; set; }
                public bool   Place    { get; set; }
                public bool   Remove   { get; set; }
                public bool   Replace  { get; set; }
                public bool   Display  { get; set; }
                public bool   Enable   { get; set; }
                public bool   Reserve1 { get; set; }
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
        /// 所有圓形的邊界資訊
        /// </summary>
        /// 
        public class Boundary
        {
            public float minX { get; set; }
            public float maxX { get; set; }
            public float minY { get; set; }
            public float maxY { get; set; }
            public float width { get; set; }
            public float height { get; set; }
        }

        /// <summary>
        /// 計算 dxf 檔所有圓的邊界
        /// </summary>
        /// <param name="Json">已讀取的 JSON 資料</param>
        public static void find_Json_Boundary(JSON Json)
        {
            // 初始化邊界
            Json_Boundary.minX = float.MaxValue;
            Json_Boundary.minY = float.MaxValue;
            Json_Boundary.maxX = float.MinValue;
            Json_Boundary.maxY = float.MinValue;

            // 遍歷所有圓，更新邊界
            foreach (var circle in Json.Circles)
            {
                Json_Boundary.minX = (float)Math.Min(Json_Boundary.minX, circle.X - circle.Diameter / 2 - 1);
                Json_Boundary.minY = (float)Math.Min(Json_Boundary.minY, circle.Y - circle.Diameter / 2 - 1);
                Json_Boundary.maxX = (float)Math.Max(Json_Boundary.maxX, circle.X + circle.Diameter / 2 + 1);
                Json_Boundary.maxY = (float)Math.Max(Json_Boundary.maxY, circle.Y + circle.Diameter / 2 + 1);
            }

            Json_Boundary.width = Json_Boundary.maxX - Json_Boundary.minX;
            Json_Boundary.height = Json_Boundary.maxY - Json_Boundary.minY;
        }

        /// <summary>
        /// 將拖曳框的參數帶入 Drag_Boundary
        /// </summary>
        /// <param name="Drag_Start">拖曳起始位置</param>
        /// <param name="Drag_End">拖曳結束位置</param>
        public static void find_Drag_Boundary()
        {
            Drag_Boundary.minX = Math.Min(Drag_Start.X, Drag_End.X);
            Drag_Boundary.minY = Math.Min(Drag_Start.Y, Drag_End.Y); 
            Drag_Boundary.maxX = Math.Max(Drag_Start.X, Drag_End.X);
            Drag_Boundary.maxY = Math.Max(Drag_Start.Y, Drag_End.Y); 
            Drag_Boundary.width = Math.Abs(Drag_End.X - Drag_Start.X);
            Drag_Boundary.height = Math.Abs(Drag_End.Y - Drag_Start.Y);
        }

        /// <summary>
        /// 拖曳框找出選重的圓
        /// </summary>
        public static void find_Selected_Circles()
        {
            SelectedCircles.Clear();

            foreach (var circle in Json.Circles)
            {
                if (Drag_Boundary.minX < (circle.X - circle.Diameter / 2) * ScaleFactor &&
                    Drag_Boundary.minY < (circle.Y - circle.Diameter / 2) * ScaleFactor &&
                    Drag_Boundary.maxX > (circle.X + circle.Diameter / 2) * ScaleFactor &&
                    Drag_Boundary.maxY > (circle.Y + circle.Diameter / 2) * ScaleFactor)
                {
                    SelectedCircles.Add(circle);
                }
            }
        }

        /// <summary>
        /// 找出需要植針的圓
        /// </summary>
        public static int find_Placed_Circles()
        {
            int irsltCount = 0;

            PlacedCircles.Clear();

            foreach (var circle in Json.Circles)
            {
                if (circle.Place == true)
                {
                    PlacedCircles.Add(circle);
                }
            }

            return irsltCount = PlacedCircles.Count();
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
        public static string strFileName = "";
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
                        strFileName = OpenDxfFileDialog.FileName;

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

            SaveJsonFileDialog.FileName = Path.GetFileNameWithoutExtension(strFileName);

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
        public static void show_grp_NeedleInfo(GroupBox grp_NeedleInfo)
        {
            foreach (Control control in grp_NeedleInfo.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:

                        switch (textBox.Name)
                        {
                            case "txt_Index":
                                textBox.Text = (FocusedCircle.Index).ToString();
                                break;
                            case "txt_Name":
                                textBox.Text = FocusedCircle.Name;
                                break;
                            case "txt_Id":
                                textBox.Text = FocusedCircle.Id;
                                break;
                            case "txt_PosX":
                                textBox.Text = (FocusedCircle.X).ToString("F3");
                                break;
                            case "txt_PosY":
                                textBox.Text = (FocusedCircle.Y).ToString("F3");
                                break;
                            case "txt_Diameter":
                                textBox.Text = (FocusedCircle.Diameter).ToString("F3");
                                break;
                        }

                        break;

                    case CheckBox checkBox:

                        switch (checkBox.Name)
                        {
                            
                            case "chk_Display":
                                checkBox.Checked = FocusedCircle.Display;
                                break;
                            case "chk_Enable":
                                checkBox.Checked = FocusedCircle.Enable;
                                break;
                            case "chk_Reserve1":
                                checkBox.Checked = FocusedCircle.Reserve1;
                                break;
                        }

                        break;

                    case RadioButton radioButton:

                        switch (radioButton.Name)
                        {
                            case "rad_Place":
                                radioButton.Checked = FocusedCircle.Place;
                                break;
                            case "rad_Remove":
                                radioButton.Checked = FocusedCircle.Remove;
                                break;
                            case "rad_Replace":
                                radioButton.Checked = FocusedCircle.Replace;
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

        /// <summary>
        /// 填寫流水號, 名稱, ID 搜尋植針
        /// </summary>
        /// <param name="needleInfo">流水號名稱ID按下Enter傳進來的 Textbox.Name</param>
        /// <param name="focusedCircle">按下Enter要查詢的圓</param>
        /// <returns>無回傳值</returns>
        public static void search_grp_NeedleInfo(string textBoxType, string textBoxText)
        {
            switch (textBoxType)
            {
                case "txt_Index":
                    foreach (var circle in Json.Circles)
                    {
                        if (circle.Index.ToString() == textBoxText)
                        {
                            FocusedCircle = circle;
                        }
                    }
                    break;

                case "txt_Name":
                    foreach (var circle in Json.Circles)
                    {
                        if (circle.Name == textBoxText)
                        {
                            FocusedCircle = circle;
                        }
                    }
                    break;

                case "txt_Id":
                    foreach (var circle in Json.Circles)
                    {
                        if (circle.Id == textBoxText)
                        {
                            FocusedCircle = circle;
                        }
                    }
                    break;
            }
        }
    }
}
