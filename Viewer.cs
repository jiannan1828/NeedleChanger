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
        public static OpenFileDialog OpenDxfFileDialog = new OpenFileDialog();
        public static SaveFileDialog SaveJsonFileDialog = new SaveFileDialog();

        public static DxfDocument DxfDoc = new DxfDocument();
        public static JSON Json = new JSON(); // 底下 JSON 不寫成靜態, HighlightedNeedle, FocusedNeedle會用到

        public static JSON.NeedleInfo HighlightedNeedle = null;
        public static JSON.NeedleInfo FocusedNeedle = null;
        public static List<JSON.NeedleInfo> SelectedNeedles = new List<JSON.NeedleInfo>();
        public static List<JSON.NeedleInfo> PlaceNeedles = new List<JSON.NeedleInfo>();

        public static readonly Color DefaltNeedleColor = Color.ForestGreen;
        public static readonly Color HiddenNeedlesColor = Color.FromArgb(64, Color.ForestGreen);
        public static readonly Color FocusedNeedleColor = Color.Goldenrod;
        public static readonly Color SelectedNeedlesColor = Color.MediumVioletRed;
        public static readonly Color PlaceNeedlesColor = Color.Red;

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

        /// <summary>
        /// 讀取 DXF 後儲存到這個物件, 後面會存成 JSON 檔
        /// </summary>
        public class JSON
        {
            public BarcodeInfo Barcode { get; set; }
            public List<NeedleInfo> Needles { get; set; }

            public class BarcodeInfo
            {
                public string Barcode { get; set; }
                public string 短編號 { get; set; }
                public string 客戶 { get; set; }
                public string 型號 { get; set; }
                public string 板全號 { get; set; }
                public string 儲位 { get; set; }
            }

            public class NeedleInfo
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
                Barcode = new BarcodeInfo();
                Needles = new List<NeedleInfo>();
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
        public static void find_Json_Boundary(JSON Json, int pic_Width, int pic_Height)
        {
            // 初始化邊界
            Json_Boundary.minX = float.MaxValue;
            Json_Boundary.minY = float.MaxValue;
            Json_Boundary.maxX = float.MinValue;
            Json_Boundary.maxY = float.MinValue;

            // 遍歷所有圓，更新邊界
            foreach (var circle in Json.Needles)
            {
                Json_Boundary.minX = (float)Math.Min(Json_Boundary.minX, circle.X - circle.Diameter / 2 - 1);
                Json_Boundary.minY = (float)Math.Min(Json_Boundary.minY, circle.Y - circle.Diameter / 2 - 1);
                Json_Boundary.maxX = (float)Math.Max(Json_Boundary.maxX, circle.X + circle.Diameter / 2 + 1);
                Json_Boundary.maxY = (float)Math.Max(Json_Boundary.maxY, circle.Y + circle.Diameter / 2 + 1);
            }

            Json_Boundary.width = Json_Boundary.maxX - Json_Boundary.minX;
            Json_Boundary.height = Json_Boundary.maxY - Json_Boundary.minY;

            ZoomFactor = Math.Min(pic_Width / ScaleFactor / Json_Boundary.width, pic_Height / ScaleFactor / Json_Boundary.height);

            Offset.X = -Json_Boundary.minX * ScaleFactor * ZoomFactor;
            Offset.Y = -Json_Boundary.maxY * ScaleFactor * -ZoomFactor;
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
        public static void find_Selected_Needles()
        {
            SelectedNeedles.Clear();

            foreach (var circle in Json.Needles)
            {
                if (Drag_Boundary.minX < (circle.X - circle.Diameter / 2) * ScaleFactor &&
                    Drag_Boundary.minY < (circle.Y - circle.Diameter / 2) * ScaleFactor &&
                    Drag_Boundary.maxX > (circle.X + circle.Diameter / 2) * ScaleFactor &&
                    Drag_Boundary.maxY > (circle.Y + circle.Diameter / 2) * ScaleFactor)
                {
                    SelectedNeedles.Add(circle);
                }
            }
        }

        /// <summary>
        /// 找出需要植針的圓
        /// </summary>
        public static int find_PlaceNeedles()
        {
            int irsltCount = 0;

            PlaceNeedles.Clear();

            foreach (var circle in Json.Needles)
            {
                if (circle.Place == true)
                {
                    PlaceNeedles.Add(circle);
                }
            }

            return irsltCount = PlaceNeedles.Count();
        }

        /// <summary>
        /// 找出需要植針圓的座標
        /// </summary>
        public static void find_PlaceNeedle_Position(int iIndex, ref double dbX, ref double dbY)
        {
            search_grp_NeedleInfo("txt_Index", iIndex.ToString());
            

            double dbTargetX = FocusedNeedle.X * (-1);  //-396.62823254488018
            double dbTargetY = FocusedNeedle.Y * (-1);  //-107.4742719089583

            //Socket1, point0, x=136.816
            //Socket1, point0, y=602.420
            const double OffsetX = -533.4442325448801;  //136.816
            const double OffsetY = -709.8942719089583;  //602.420

            dbX = dbTargetX - OffsetX;  //136.816 = -396.62823254488018 - OffsetX, OffsetX = 533.4442325448801
            dbY = dbTargetY - OffsetY;  //602.420 = -107.4742719089583  - OffsetY, OffsetY = 709.8942719089583
        }

        /// <summary>
        /// 將 DXF 檔中的資料轉成 JSON
        /// </summary>
        /// <param name="DxfDoc">要顯示的 DXF 檔案</param>
        /// <param name="json">已轉換的 JSON 文件</param>
        public static void TransformDxf2Json(DxfDocument DxfDoc, ref JSON json)
        {
            int index = 0;

            json.Needles.Clear(); // 清除上一次 load 的資料

            foreach (var circle in DxfDoc.Entities.Circles)
            {
                json.Needles.Add(new JSON.NeedleInfo
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

            var resortedIndex = new (double XaddY, int fakeIndex)[json.Needles.Count];

            for (int i = 0; i < json.Needles.Count; i++)
            {
                resortedIndex[i] = (json.Needles[i].X - json.Needles[i].Y * 10000, json.Needles[i].Index);
            }

            Array.Sort(resortedIndex, (prev, next) => prev.XaddY.CompareTo(next.XaddY)); // 由小排到大

            for (int i = 0; i < json.Needles.Count; i++)
            {
                resortedJson.Needles.Add(json.Needles[resortedIndex[i].fakeIndex]);
                resortedJson.Needles[i].Index = i;
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
                            //MessageBox.Show($"檔案 {OpenDxfFileDialog.FileName} 成功讀取！");

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
                        strFileName = OpenDxfFileDialog.FileName;
                        //MessageBox.Show($"檔案 {OpenDxfFileDialog.FileName} 成功讀取！");
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

                //MessageBox.Show("檔案儲存成功！", "儲存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 在 groupbox 中顯示植針資訊
        /// </summary>
        /// <param name="grp_NeedleInfo">植針資訊的 Groupbox</param>
        /// <param name="FocusedNeedle">在 picturebox 上按下的圓</param>
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
                                textBox.Text = (FocusedNeedle.Index).ToString();
                                break;
                            case "txt_Name":
                                textBox.Text = FocusedNeedle.Name;
                                break;
                            case "txt_Id":
                                textBox.Text = FocusedNeedle.Id;
                                break;
                            case "txt_PosX":
                                textBox.Text = (FocusedNeedle.X).ToString("F3");
                                break;
                            case "txt_PosY":
                                textBox.Text = (FocusedNeedle.Y).ToString("F3");
                                break;
                            case "txt_Diameter":
                                textBox.Text = (FocusedNeedle.Diameter).ToString("F3");
                                break;
                        }

                        break;

                    case CheckBox checkBox:

                        switch (checkBox.Name)
                        {
                            
                            case "chk_Display":
                                checkBox.Checked = FocusedNeedle.Display;
                                break;
                            case "chk_Enable":
                                checkBox.Checked = FocusedNeedle.Enable;
                                break;
                            case "chk_Reserve1":
                                checkBox.Checked = FocusedNeedle.Reserve1;
                                break;
                        }

                        break;

                    case RadioButton radioButton:

                        switch (radioButton.Name)
                        {
                            case "rad_Place":
                                radioButton.Checked = FocusedNeedle.Place;
                                break;
                            case "rad_Remove":
                                radioButton.Checked = FocusedNeedle.Remove;
                                break;
                            case "rad_Replace":
                                radioButton.Checked = FocusedNeedle.Replace;
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
        /// <param name="FocusedNeedle">按下Enter要查詢的圓</param>
        /// <returns>無回傳值</returns>
        public static void search_grp_NeedleInfo(string textBoxType, string textBoxText)
        {
            switch (textBoxType)
            {
                case "txt_Index":
                    foreach (var circle in Json.Needles)
                    {
                        if (circle.Index.ToString() == textBoxText)
                        {
                            FocusedNeedle = circle;
                        }
                    }
                    break;

                case "txt_Name":
                    foreach (var circle in Json.Needles)
                    {
                        if (circle.Name == textBoxText)
                        {
                            FocusedNeedle = circle;
                        }
                    }
                    break;

                case "txt_Id":
                    foreach (var circle in Json.Needles)
                    {
                        if (circle.Id == textBoxText)
                        {
                            FocusedNeedle = circle;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 在 groupbox 中顯示植針資訊
        /// </summary>
        /// <param name="grp_NeedleInfo">植針資訊的 Groupbox</param>
        /// <param name="FocusedNeedle">在 picturebox 上按下的圓</param>
        /// <returns>無回傳值</returns>
        public static void show_grp_BarcodeInfo(GroupBox grp_BarcodeInfo)
        {
            foreach (Control control in grp_BarcodeInfo.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:

                        switch (textBox.Name)
                        {
                            case "txt_Barcode":
                                textBox.Text = Json.Barcode.Barcode;
                                break;
                            case "txt_短編號":
                                textBox.Text = Json.Barcode.短編號;
                                break;
                            case "txt_客戶":
                                textBox.Text = Json.Barcode.客戶;
                                break;
                            case "txt_型號":
                                textBox.Text = Json.Barcode.型號;
                                break;
                            case "txt_板全號":
                                textBox.Text = Json.Barcode.板全號;
                                break;
                            case "txt_儲位":
                                textBox.Text = Json.Barcode.儲位;
                                break;
                        }

                        break;

                    
                }
            }
        }

        /// <summary>
        /// 在 richtextbox 顯示當前資訊
        /// </summary>
        /// <param name="message">目標資訊</param>
        /// <returns>無回傳值</returns>
        public static void rtb_Status_AppendMessage(RichTextBox rtb_Status, string message)
        {
            // 獲取當前時間
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 構造帶有當前時間和訊息的字串
            string textToAdd = $"[{currentTime}] {message}";

            // 更新RichTextBox內容，保持換行
            rtb_Status.AppendText(textToAdd + Environment.NewLine);

            // 滾動到最後一行
            rtb_Status.ScrollToCaret();
        }
    }
}
