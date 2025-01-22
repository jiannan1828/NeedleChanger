using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using netDxf;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;
using System.Drawing;
using static InjectorInspector.Viewer.MissionInfo;

namespace InjectorInspector
{
    internal static class Viewer
    {
        public static List<string> BarcodeList { get; set; } = new List<string>();
        public static OpenFileDialog OpenMissionFileDialog = new OpenFileDialog();
        public static SaveFileDialog SaveMissionFileDialog = new SaveFileDialog();

        public static DxfDocument DxfDoc = new DxfDocument();
        public static MissionInfo Mission = new MissionInfo(); // 底下 MissionInfo 不寫成靜態, HighlightedNeedle, FocusedNeedle會用到
        public static string OpenFileName = "";


        public static MissionInfo.NeedleInfo HighlightedNeedle = null;
        public static MissionInfo.NeedleInfo FocusedNeedle = null;
        public static List<MissionInfo.NeedleInfo> SelectedNeedles = new List<MissionInfo.NeedleInfo>();
        public static List<MissionInfo.NeedleInfo> PlaceNeedles = new List<MissionInfo.NeedleInfo>();

        public static readonly Color DefaltNeedleColor = Color.ForestGreen;
        public static readonly Color HiddenNeedlesColor = Color.FromArgb(64, Color.ForestGreen);
        public static readonly Color FocusedNeedleColor = Color.Goldenrod;
        public static readonly Color SelectedNeedlesColor = Color.MediumVioletRed;
        public static readonly Color PlaceNeedlesColor = Color.Red;

        public static Boundary Mission_Boundary = new Boundary();
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
        /// 讀取 DXF 或 JSON 後儲存到這個物件, 後面會存成 Json 檔
        /// </summary>
        public class MissionInfo
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

            public MissionInfo()
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
        /// <param name="Mission">已讀取的 MissionInfo 資料</param>
        public static void find_Mission_Boundary(MissionInfo Mission, int pic_Width, int pic_Height)
        {
            // 初始化邊界
            Mission_Boundary.minX = float.MaxValue;
            Mission_Boundary.minY = float.MaxValue;
            Mission_Boundary.maxX = float.MinValue;
            Mission_Boundary.maxY = float.MinValue;

            // 遍歷所有圓，更新邊界
            foreach (var circle in Mission.Needles)
            {
                Mission_Boundary.minX = (float)Math.Min(Mission_Boundary.minX, circle.X - circle.Diameter / 2 - 1);
                Mission_Boundary.minY = (float)Math.Min(Mission_Boundary.minY, circle.Y - circle.Diameter / 2 - 1);
                Mission_Boundary.maxX = (float)Math.Max(Mission_Boundary.maxX, circle.X + circle.Diameter / 2 + 1);
                Mission_Boundary.maxY = (float)Math.Max(Mission_Boundary.maxY, circle.Y + circle.Diameter / 2 + 1);
            }

            Mission_Boundary.width = Mission_Boundary.maxX - Mission_Boundary.minX;
            Mission_Boundary.height = Mission_Boundary.maxY - Mission_Boundary.minY;

            ZoomFactor = Math.Min(pic_Width / ScaleFactor / Mission_Boundary.width, pic_Height / ScaleFactor / Mission_Boundary.height);

            Offset.X = -Mission_Boundary.minX * ScaleFactor * ZoomFactor;
            Offset.Y = -Mission_Boundary.maxY * ScaleFactor * -ZoomFactor;
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

            foreach (var circle in Mission.Needles)
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

            foreach (var circle in Mission.Needles)
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
        /// 將 DXF 檔中的資料轉成 MissionInfo
        /// </summary>
        /// <param name="DxfDoc">要顯示的 DXF 檔案</param>
        /// <param name="mission">已轉換的 MissionInfo 文件</param>
        public static void TransformDxf2Mission(DxfDocument DxfDoc, ref MissionInfo mission)
        {
            int index = 0;

            mission.Needles.Clear(); // 清除上一次 load 的資料

            foreach (var circle in DxfDoc.Entities.Circles)
            {
                mission.Needles.Add(new MissionInfo.NeedleInfo
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
        /// <param name="mission">已讀取的 MissionInfo 資料</param>
        public static void ResortPosition(ref MissionInfo mission)
        {
            MissionInfo resortedMission = new MissionInfo();

            var resortedIndex = new (double XaddY, int fakeIndex)[mission.Needles.Count];

            for (int i = 0; i < mission.Needles.Count; i++)
            {
                resortedIndex[i] = (mission.Needles[i].X - mission.Needles[i].Y * 10000, mission.Needles[i].Index);
            }

            Array.Sort(resortedIndex, (prev, next) => prev.XaddY.CompareTo(next.XaddY)); // 由小排到大

            for (int i = 0; i < mission.Needles.Count; i++)
            {
                resortedMission.Needles.Add(mission.Needles[resortedIndex[i].fakeIndex]);
                resortedMission.Needles[i].Index = i;
            }

            mission = resortedMission;
        }

        /// <summary>
        /// 打開 DXF 或者 MissionInfo 檔案
        /// </summary>
        public static bool OpenFile()
        {
            OpenMissionFileDialog.Filter = "Json Files (*.json)|*.json|DXF Files (*.dxf)|*.dxf";
            
            if (OpenMissionFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (OpenMissionFileDialog.FilterIndex == 2) // 如果選擇 .dxf
                {
                    try
                    {
                        DxfDoc = DxfDocument.Load(OpenMissionFileDialog.FileName);
                        OpenFileName = OpenMissionFileDialog.FileName;

                        if (DxfDoc.Entities.Circles.Count() > 0)
                        {
                            //MessageBox.Show($"檔案 {OpenMissionFileDialog.FileName} 成功讀取！");

                            TransformDxf2Mission(DxfDoc, ref Mission);
                            ResortPosition(ref Mission);
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
                else if (OpenMissionFileDialog.FilterIndex == 1) // 如果選擇 .mission
                {
                    try
                    {
                        Mission = JsonConvert.DeserializeObject<MissionInfo>(File.ReadAllText(OpenMissionFileDialog.FileName));
                        OpenFileName = OpenMissionFileDialog.FileName;
                        //MessageBox.Show($"檔案 {OpenMissionFileDialog.FileName} 成功讀取！");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"讀取 Mission 檔時發生錯誤: {ex.Message}");
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
        /// 儲存 MissionInfo 檔案
        /// </summary>
        public static void SaveFile()
        {
            SaveMissionFileDialog.Filter = "Json Files (*.json)|*.json";

            SaveMissionFileDialog.FileName = Path.GetFileNameWithoutExtension(OpenFileName);

            if (SaveMissionFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 使用 Newtonsoft.Mission 進行物件序列化，並設定格式化輸出（會縮排顯示）
                string mission = JsonConvert.SerializeObject(Mission, Newtonsoft.Json.Formatting.Indented);

                // 使用 StreamWriter 儲存 Mission 到選定的檔案
                using (StreamWriter writer = new StreamWriter(SaveMissionFileDialog.FileName))
                {
                    writer.Write(mission);
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
                    foreach (var circle in Mission.Needles)
                    {
                        if (circle.Index.ToString() == textBoxText)
                        {
                            FocusedNeedle = circle;
                        }
                    }
                    break;

                case "txt_Name":
                    foreach (var circle in Mission.Needles)
                    {
                        if (circle.Name == textBoxText)
                        {
                            FocusedNeedle = circle;
                        }
                    }
                    break;

                case "txt_Id":
                    foreach (var circle in Mission.Needles)
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
                                textBox.Text = Mission.Barcode.Barcode;
                                break;
                            case "txt_短編號":
                                textBox.Text = Mission.Barcode.短編號;
                                break;
                            case "txt_客戶":
                                textBox.Text = Mission.Barcode.客戶;
                                break;
                            case "txt_型號":
                                textBox.Text = Mission.Barcode.型號;
                                break;
                            case "txt_板全號":
                                textBox.Text = Mission.Barcode.板全號;
                                break;
                            case "txt_儲位":
                                textBox.Text = Mission.Barcode.儲位;
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

        /// <summary>
        /// 讀取 Barcode 清單
        /// </summary>
        public static void read_BarcodeList()
        {
            try
            {
                foreach (var line in File.ReadLines(@"Info\BarcodeList.txt"))
                {
                    BarcodeList.Add(line);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"讀取文件時發生錯誤: {ex.Message}");
            }
        }
    }
}
