using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using WMX3ApiCLR;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using static WMX3ApiCLR.AdvMotion;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;

using System.IO;
using System.Text.Json;

namespace InjectorInspector
{
    public partial class Form1 : Form
    {
        //JSON
        public List<JsonContent> jsonContentList;
        public string filePath;

        //WMX3
        WMX3Api wmx = new WMX3Api();
        Io io = new Io();
        CoreMotion motion              = new CoreMotion();
        CoreMotionStatus CmStatus      = new CoreMotionStatus();
        EngineStatus EnStatus          = new EngineStatus();
        Config.HomeParam AxisHomeParam = new Config.HomeParam();
        Stopwatch stopWatch            = new Stopwatch();
        AdvancedMotion advmon          = new AdvancedMotion();

        /// <summary>
        /// ServoMotor WMX3 Control API
        /// </summary>
        /// 
        public void WMX3_Initial()
        {
            //建立裝置
            wmx.CreateDevice("C:\\Program Files\\SoftServo\\WMX3", DeviceType.DeviceTypeNormal, 10000);  

            //設定裝置名稱
            wmx.SetDeviceName("DLF");

            //設置齒輪比
            int A = motion.Config.SetGearRatio(0, 1048576, 10000);  
            int B = motion.Config.SetGearRatio(1, 1048576, 10000);

        }  //end of public void WMX3_Initial()

        public int WMX3_establish_Commu()
        {
            int rslt = 0;

            int ret = wmx.StartCommunication();
            if (ret != 0) {
                string str = WMX3Api.ErrorToString(ret);
                MessageBox.Show(str);
            }

            return rslt;
        }  //end of public void WMX3_establish_Commu()

        public void WMX3_destroy_Commu()
        {
            wmx.StopCommunication();
        }  //end of public void WMX3_destroy_Commu()

        public int WMX3_check_Commu()
        {
            int rslt = 0;

            //讀取當前通訊狀態
            motion.GetStatus(ref CmStatus);

            switch (CmStatus.EngineState) {
                case EngineState.Running:
                    rslt = 0;
                    break;

                case EngineState.Communicating:
                    rslt = 1;
                    break;
            }

            return rslt;
        }  //end of public int WMX3_check_Commu()

        public void WMX3_ServoOnOff(int axis, int bOn)
        {
            int newStatus = bOn;

            //啟動伺服
            int ret = motion.AxisControl.SetServoOn(axis, newStatus);

            if (ret != 0) {
                string ers = CoreMotion.ErrorToString(ret);
                MessageBox.Show($"{ers}");
            }
        }  //end of public void WMX3_ServoOn(int axis)

        public int WMX3_check_ServoOnOff(int axis, ref string position, ref string speed)
        {
            int rslt = 0;

            //讀取SV ON狀態
            CoreMotionAxisStatus cmAxis = CmStatus.AxesStatus[axis];
            if (cmAxis.ServoOn == true) {
                rslt = 1;
            } else {
                rslt = 0;
            }

            //讀取目前位置
            string Profile = cmAxis.ActualPos.ToString();

            //strtok info
            position = Profile.Substring(0, Math.Min(Profile.Length, 6));
            speed    = cmAxis.ActualVelocity.ToString();
            //AcTqr0.Text = cmAxis.ActualTorque.ToString();

            return rslt;
        }  //end of int WMX3_check_ServoOnOff(int axis)

        public int WMX3_Pivot(int axis, int pivot, int speed, int accel, int daccel)
        {
            int rslt = 0;

            //位置控制設定
            int ret1 = motion.AxisControl.SetAxisCommandMode(0, AxisCommandMode.Position);

            //POS參數設置
            Motion.PosCommand pos = new Motion.PosCommand();

            pos.Profile.Type     = ProfileType.Trapezoidal;  //運動模式
            pos.Axis             = axis;    //軸
            pos.Target           = pivot;   //指定位置
            pos.Profile.Velocity = speed;   //速度
            pos.Profile.Acc      = accel;   //加速度
            pos.Profile.Dec      = daccel;  //減速度

            //啟動POS運轉
            rslt = motion.Motion.StartPos(pos);  

            if (rslt != 0) {
                string ers = CoreMotion.ErrorToString(rslt);  //如果無法通訊則報錯誤給使用者
                //   textBox12.Text += "軸" + textBox3.Text + ":" + ers + "\r\n";
            }

            return rslt;
        }  //end of public void WMX3_Pivot(int pivot, int speed, int accel, int daccel)

        public int WMX3_SetHomePosition(int axis)
        {
            int rslt = 0;

            //讀取當前座標
            AxisHomeParam.HomeType = Config.HomeType.CurrentPos;

            //設置原點參數
            rslt = motion.Config.SetHomeParam(axis, AxisHomeParam);

            if (rslt != 0) {
                string ers = CoreMotion.ErrorToString(rslt);//如果無法通訊則報錯誤給使用者
                //   textBox12.Text += "軸" + textBox9.Text + "設置HOME錯誤" + ers + "\r\n";
            }

            //開始回原點
            rslt = motion.Home.StartHome(axis); 

            return rslt;
        }  //end of public int WMX3_SetHomePosition(int axis)






        /// <summary>
        /// Project Code implement
        /// </summary>
        /// 
        public Form1()
        {
            //C# project code component initialize
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WMX3_Initial();

            //先跳到第2頁
            tabControl1.SelectedTab = tabControl1.TabPages[2-1];

            this.Text = "2024/08/20 17:43";
        }

        private void strCommunication_Click_1(object sender, EventArgs e)
        {
            WMX3_establish_Commu();
        }
        private void stoCommunication_Click_1(object sender, EventArgs e)
        {
            WMX3_destroy_Commu();
        }
        private void svOn0_Click(object sender, EventArgs e)
        {
            int axis = 0;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void svOn1_Click_1(object sender, EventArgs e)
        {
            int axis = 1;
            int isOn = 1;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void svOff0_Click_1(object sender, EventArgs e)
        {
            int axis = 0;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }
        private void svOff1_Click(object sender, EventArgs e)
        {
            int axis = 1;
            int isOn = 0;
            WMX3_ServoOnOff(axis, isOn);
        }

        private void Home0_Click(object sender, EventArgs e)
        {
            int axis;

            axis = 0;
            WMX3_SetHomePosition(axis);

            axis = 1;
            WMX3_SetHomePosition(axis);
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //WMX3通訊狀態
            int getCommuStatus = WMX3_check_Commu();
            if (getCommuStatus == 1) {
                label12.Text = "連線中";
                label12.ForeColor = Color.Red;
            } else {
                label12.Text = "尚未連線";
                label12.ForeColor = Color.Black;
            }

            //region 讀取軸狀態
            int rslt = 0;
            int axis = 0;
            string position = "";
            string speed    = "";

            axis = 0;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if(rslt == 1) {
                svOn0.BackColor = Color.Red;
            } else {
                svOn0.BackColor = Color.Green;
            }
            AcPos0.Text = position;
            AcSpd0.Text = speed;

            axis = 1;
            rslt = WMX3_check_ServoOnOff(axis, ref position, ref speed);
            if (rslt == 1) {
                svOn1.BackColor = Color.Red;
            } else {
                svOn1.BackColor = Color.Green;
            }
            AcPos1.Text = position;
            AcSpd1.Text = speed;
        }

        private void stPos1_Click_1(object sender, EventArgs e)
        {
            //單點動

            int axis;

            int position = int.Parse(textBox2.Text);
            int speed    = int.Parse(textBox1.Text);
            int accel    = 100000 * 10;
            int daccel   = 100000 * 10;

            axis = 0;
            WMX3_Pivot(axis, position, speed, accel, daccel);

            axis = 1;
            WMX3_Pivot(axis, position * 10, speed * 10, accel * 10, daccel * 10);
        }

        private void 緊急停止_Click(object sender, EventArgs e)
        {
            int isOn = 0;
            int axis = 0;

            axis = 0;
            WMX3_ServoOnOff(axis, isOn);

            axis = 1;
            WMX3_ServoOnOff(axis, isOn);
        }






        /// <summary>
        /// JSON function
        /// </summary>
        /// 
        public void InitialJsonFile()
        {
            // 初始化列表和文件路徑
            jsonContentList = new List<JsonContent>();
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "received_data.json");

            // 如果文件存在，讀取現有數據
            if (File.Exists(filePath)) {
                try {
                    string jsonString = File.ReadAllText(filePath);
                    jsonContentList = JsonSerializer.Deserialize<List<JsonContent>>(jsonString) ?? new List<JsonContent>();
                    Console.WriteLine("成功初始化 JSON 文件。");
                } catch (Exception ex) {
                    Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
                }
            } else {
                Console.WriteLine("文件不存在，初始化為空列表。");
            }
        }
        public void AddJsonContent(JsonContent newContent)
        {
            jsonContentList.Add(newContent);
            WriteDataToJsonFile();
            Console.WriteLine("成功添加新的 JSON 內容並更新文件。");
        }
        public void RemoveJsonContentByIndex(uint u32Index)
        {
            // 使用 RemoveAll 方法來刪除所有符合條件的項目
            int removedCount = jsonContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0) {
                // 如果有項目被刪除，則寫入更新後的列表到文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功刪除 {removedCount} 個 u32Index 為 {u32Index} 的項目。");
            } else {
                // 如果未找到匹配的項目，顯示提示
                Console.WriteLine($"未找到 u32Index 為 {u32Index} 的項目。");
            }
        }
        public void SortAndRemoveDuplicates()
        {
            // 使用 LINQ 去重並排序
            jsonContentList = jsonContentList
                .GroupBy(p => p.u32Index)
                .Select(g => g.First())    // 取每個組的第一個項目
                .OrderBy(p => p.u32Index)  // 按 u32Index 排序
                .ToList();

            Console.WriteLine("排序並去重完成。");
        }
        public void WriteDataToJsonFile()
        {
            try {
                // 先排序並去重
                SortAndRemoveDuplicates();

                // 序列化列表為 JSON 字串
                string jsonString = JsonSerializer.Serialize(jsonContentList, new JsonSerializerOptions { WriteIndented = true });

                // 輸出序列化結果到控制台，幫助調試
                Console.WriteLine("更新後的 JSON 文件內容:\n" + jsonString);

                // 寫入 JSON 字串到文件
                File.WriteAllText(filePath, jsonString);

                Console.WriteLine("成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 處理錯誤，例如記錄錯誤信息
                Console.WriteLine("寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }

        public void WriteDataToJsonFile_Test()
        {
            // 使用正確的類型來創建對象
            List<JsonContent> TestReadWriteJson = new List<JsonContent>
            {
                new JsonContent { strLabel = "PinA", u32Index =  0, dblXCoordinate = 1.234, dblYCoordinate = 5.678, bReplace = true, bVisible = true },
                new JsonContent { strLabel = "SSKR", u32Index =  3, dblXCoordinate = 2.345, dblYCoordinate = 6.789, bReplace = true, bVisible = true },
                new JsonContent { strLabel = "FUCK", u32Index = 18, dblXCoordinate = 3.456, dblYCoordinate = 7.890, bReplace = true, bVisible = true },
            };

            // 序列化對象為 JSON 字串
            string jsonString = JsonSerializer.Serialize(TestReadWriteJson, new JsonSerializerOptions { WriteIndented = true });

            // 確保目錄存在，如果不存在則創建
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(folderPath, "received_data.json");

            try {
                // 寫入 JSON 字串到文件
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine("測試數據成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("測試數據寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }

        public string ReadNameFromJsonFile()
        {
            string rslt = "";

            uint u32_Index = 3;

            // 獲取當前應用程序目錄
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(folderPath, "received_data.json");

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonContent>>(jsonString);

                    // 查找 u32_Index
                    JsonContent GotNeedle = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32_Index);

                    if (GotNeedle != null) {
                        // 找到，輸出信息
                        rslt = $"{GotNeedle.strLabel} {GotNeedle.u32Index} {GotNeedle.dblXCoordinate} {GotNeedle.dblYCoordinate}";
                        Console.WriteLine("查詢結果: " + rslt);
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }
        public string ReadHeightWeightFromJsonFile()
        {
            string rslt = "";

            double max_X = 2.82;
            double min_X = 1.18;
            double max_Y = 7.2;
            double min_Y = 5.3;

            // 獲取當前應用程序目錄
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(folderPath, "received_data.json");

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonContent> Needle = JsonSerializer.Deserialize<List<JsonContent>>(jsonString);

                    // 根據X和Y範圍過濾
                    var filteredNeedle = Needle?.Where(p => p.dblXCoordinate >= min_X && p.dblXCoordinate <= max_X &&
                                                            p.dblYCoordinate >= min_Y && p.dblYCoordinate <= max_Y);

                    if (filteredNeedle != null && filteredNeedle.Any()) {
                        // 找到滿足條件的
                        rslt = string.Join(", ", filteredNeedle.Select(p => $"{p.strLabel} (u32Index: {p.u32Index}, X: {p.dblXCoordinate}, Y: {p.dblYCoordinate}) \r\n"));
                        Console.WriteLine("篩選結果:\n" + rslt);
                        label8.Text = rslt;
                    } else {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                } else {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return rslt;
        }






        /// <summary>
        /// Reserve function
        /// </summary>
        /// 
        private void button3_Click_1(object sender, EventArgs e)
        {
            //路徑動
            AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
            
            path.Axis[0] = 0;
            path.Axis[1] = 1;

            path.EnableConstProfile  = 1;

            path.Profile[0].Type = ProfileType.Trapezoidal;
            path.NumPoints = 6;
            
            path.Profile[0].Velocity = 10000;
            path.Profile[0].Acc      = 10000;
            path.Profile[0].Dec      = 10000;

            path.Type[0] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0,0] = 0;
            path.Target[1,0] = 0;

            path.Type[1] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 1] = 0;
            path.Target[1, 1] = 5000;

            path.Type[2] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 2] = 5000;
            path.Target[1, 2] = 15000;

            path.Type[3] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 3] = 25000;
            path.Target[1, 3] = 15000;

            path.Type[4] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 4] = 30000;
            path.Target[1, 4] = 5000;

            path.Type[5] = AdvMotion.PathIntplSegmentType.Linear;
            path.Target[0, 5] = 30000;
            path.Target[1, 5] = 0;

            // int ret = motion.Motion.StartPos(path);
            int a1 = advmon.AdvMotion.StartPathIntplPos(path);

            //int a = WmxLib_Adv.AdvMotion.StartCSplinePos(0, splineCommand, 9, splinePoint);
            //AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
            //static AdvancedMotion WmxLib_Adv = new AdvancedMotion(Wmx3Lib);
            //AdvMotion.PathIntplCommand path = new AdvMotion.PathIntplCommand();
        }






        /// <summary>
        /// Uunknow
        /// </summary>
        /// 
        private void Pos_Click(object sender, EventArgs e)
        {
            Config.HomeParam homeParam = new Config.HomeParam();
            motion.Config.GetHomeParam(0, ref homeParam);
            homeParam.HomeType = Config.HomeType.CurrentPos;

            motion.Home.StartHome(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Velocity.VelCommand vel = new Velocity.VelCommand();
            Torque.TrqCommand trq = new Torque.TrqCommand();

            //Set axis to velocity command mode
            int ret = motion.AxisControl.SetAxisCommandMode(0, AxisCommandMode.Velocity);
            //  err = wmxlib_cm.axisControl->SetAxisCommandMode(0, AxisCommandMode::Velocity);

            // if (err != ErrorCode::None)
            // {
            //     wmxlib_cm.ErrorToString(err, errString, sizeof(errString));
            //     printf("Failed to set axis command mode to velocity. Error=%d (%s)\n", err, errString);
            //     goto exit;
            // }

            //Set velocity command parameters
            vel.Axis = 0;
            vel.Profile.Type = ProfileType.Trapezoidal;
            vel.Profile.Velocity = 100000;
            vel.Profile.Acc = 10000;
            vel.Profile.Dec = 10000;
            //Execute a velocity command
            int ret1 = motion.Velocity.StartVel(vel);
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            int ret2 = motion.Velocity.Stop(0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //WriteDataToJsonFile();

            InitialJsonFile();

            // 創建新的 JsonContent 對象
            JsonContent newTestContent = new JsonContent
            {
                strLabel       = "GGshimida",
                u32Index       = 87,
                dblXCoordinate = 1.357,
                dblYCoordinate = 2.468,
                bReplace = false,
                bVisible = true
            };
            AddJsonContent(newTestContent);

            JsonContent newTestContent2 = new JsonContent
            {
                strLabel = "KKis87",
                u32Index = 7878,
                dblXCoordinate = 0.2468,
                dblYCoordinate = 1.3579,
                bReplace = false,
                bVisible = true
            };
            AddJsonContent(newTestContent2);

            RemoveJsonContentByIndex(18);

            ReadHeightWeightFromJsonFile();
            //this.Text = ReadNameFromJsonFile();
        }

        // Homing.
        // Config.HomeParam homeParam = new Config.HomeParam();
        // motion.Config.GetHomeParam(0, ref homeParam);
        // homeParam.HomeType = Config.HomeType.CurrentPos;
        // Motion.Config.SetHomeParam(0, homeParam);

        //Motion.Motion.Wait(0);
    }






    /// <summary>
    /// JSON function
    /// </summary>
    /// 
    public class JsonContent
    {
        public string strLabel { get; set; }
        public uint   u32Index { get; set; }
        public double dblXCoordinate { get; set; } 
        public double dblYCoordinate { get; set; } 
        public bool   bReplace { get; set; }
        public bool   bVisible { get; set; }
    }

}

