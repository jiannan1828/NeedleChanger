using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TCP Server
//Vibration
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;

namespace InjectorInspector
{
    internal class Vibration
    {  // start of internal class Vibration

        //Create TCP Vibration Connection
        public bool isEstablishTCP = false;

        // 設定伺服器端的 IP 和 Port
        public IPAddress ip = IPAddress.Parse("192.168.2.124");
        public int port = 6032;
        TcpListener server;
        TcpClient client;

        //Command Header START
            public const string strCMDU15Header = "AB,1,";

            public uint u32Frequency = 0;
            public uint u32VibrationSource1_StartPhase = 0;
            public uint u32VibrationSource1_StopPhase = 0;
            public uint u32VibrationSource2_StartPhase = 0;
            public uint u32VibrationSource2_StopPhase = 0;
            public uint u32VibrationSource3_StartPhase = 0;
            public uint u32VibrationSource3_StopPhase = 0;
            public uint u32VibrationSource4_StartPhase = 0;
            public uint u32VibrationSource4_StopPhase = 0;
            public uint u32BlackDepotSource_StartPhase = 0;
            public uint u32BlackDepotSource_StopPhase = 0;
            public uint u32VibrationSource1_Power = 0;
            public uint u32VibrationSource2_Power = 0;
            public uint u32VibrationSource3_Power = 0;
            public uint u32VibrationSource4_Power = 0;
            public uint u32BlackDepotSource_Power = 0;

            public uint u32LED_Level = 0;
        //Command Header END

        public enum xe_U15_CMD
        {  // start of public enum xe_U15_CMD
            xeUC_TestMode_Parameters = 32,
            xeUC_TestMode_FunctionOn = 33,
            xeUC_SendParametersToRecipeNo = 34,
            xeUC_SendLightOnToRecipeNo = 35,
            xeUC_RunRecipeNo = 36,
            xeUC_Reserve_SendDepotsToRecipeNo = 37,
            xeUC_ReadMachineStatus = 38,
            xeUC_ReadParametersOfRecipeNo = 39,
            xeUC_TestMode_LightLevel = 40,
            xeUC_Reserve_TestMode_ParametersOfDepot = 41,
            xeUC_NullCMD_AutoReadStatus = 42,
            xeUC_Reserve_SetControlMode = 43,
            xeUC_Reserve_GetControlMode = 44,
            xeUC_RunMultiRecipeNo = 45,
            xeUC_SetRecipeGroupNo = 46,
            xeUC_GetRecipeGroupNo = 47,
            xeUC_RunRecipeGroupNo = 48,
            xeUC_Reserve_GetDIOStatus = 49,
            xeUC_ReadMachineInfo = 50,
            xeUC_Reserve01 = 51,
            xeUC_Reserve02 = 52,
            xeUC_Reserve03 = 53,
            xeUC_KillRecipeNo = 54,
            xeUC_KillRecipeGroupNo = 55,
            xeUC_Reserve_UpperLightOn = 56,

            xeUC_MachineInfo_START = 100,
            xeUC_MachineFirmwareType = 101,
            xeUC_MachineFirmwareVersion = 102,
            xeUC_MachineType = 103,
            xeUC_MachineSerialNumber = 104,
            xeUC_MachineInfo_END = 105,
        }  // end of public enum xe_U15_CMD

        public void apiEstablishTCPVibration()
        {
            if (isEstablishTCP == false)
            {
                isEstablishTCP = true;

                try
                {
                    // 建立 TcpListener 來監聽指定的 IP 和 Port
                    server = new TcpListener(ip, port);
                    server.Start();
                    Console.WriteLine("伺服器啟動中，等待客戶端連接...");

                    // 接受來自客戶端的連接
                    client = server.AcceptTcpClient();
                    Console.WriteLine("客戶端已連接！");
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException: " + ex.Message);
                }

                // 延遲0.5秒（500毫秒）
                Thread.Sleep(500);

                if (true)
                {  //Test Mode
                    SetVibration(u32Frequency, u32VibrationSource1_StartPhase,
                                               u32VibrationSource1_StopPhase,
                                               u32VibrationSource2_StartPhase,
                                               u32VibrationSource2_StopPhase,
                                               u32VibrationSource3_StartPhase,
                                               u32VibrationSource3_StopPhase,
                                               u32VibrationSource4_StartPhase,
                                               u32VibrationSource4_StopPhase,
                                               u32BlackDepotSource_StartPhase,
                                               u32BlackDepotSource_StopPhase,
                                               u32VibrationSource1_Power,
                                               u32VibrationSource2_Power,
                                               u32VibrationSource3_Power,
                                               u32VibrationSource4_Power,
                                               u32BlackDepotSource_Power);
                }  //end of if (true) {  //Test Mode

                //Vibration
                if (true)
                { //Test LED Level
                    SetVibrationLED(u32LED_Level);
                }  //end of if (true) { //Test LED Level

                // 處理客戶端連接的任務可交由不同的線程執行
                if (false)
                {
                    Thread thread = new Thread(HandleClient);
                    thread.Start(client);
                }
            }
        }

        public void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                // 持續處理客戶端的資料傳輸
                while (client != null)
                {
                    // 讀取來自客戶端的資料
                    if ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("收到: " + message);

                        // 伺服器主動回應訊息
                        byte[] response = Encoding.ASCII.GetBytes("伺服器收到: " + message);
                        stream.Write(response, 0, response.Length);
                    }

                    // 伺服器主動發送訊息給客戶端
                    string serverMessage = "這是伺服器主動發送的訊息";
                    byte[] serverResponse = Encoding.ASCII.GetBytes(serverMessage);
                    stream.Write(serverResponse, 0, serverResponse.Length);
                    Console.WriteLine("伺服器發送: " + serverMessage);

                    // 可以加一些延遲來避免過於頻繁的訊息發送
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("發生錯誤: " + e.Message);
            }
            finally
            {
                // 關閉連接
                client.Close();
                Console.WriteLine("客戶端已斷開連接。");
            }
        }

        public void Px1_SendCMD(xe_U15_CMD Px1_CMD, uint u32Px1)
        {
            switch (Px1_CMD)
            {
                case xe_U15_CMD.xeUC_TestMode_FunctionOn:
                case xe_U15_CMD.xeUC_RunRecipeNo:
                case xe_U15_CMD.xeUC_ReadParametersOfRecipeNo:
                case xe_U15_CMD.xeUC_TestMode_LightLevel:
                case xe_U15_CMD.xeUC_GetRecipeGroupNo:
                case xe_U15_CMD.xeUC_RunRecipeGroupNo:
                case xe_U15_CMD.xeUC_KillRecipeNo:
                case xe_U15_CMD.xeUC_KillRecipeGroupNo:
                    {
                        const uint u32ParaCount = 1;
                        // 組合命令字串
                        string CMD_U15 = $"{strCMDU15Header}{(uint)Px1_CMD},{(uint)u32ParaCount},{(uint)u32Px1},\r\n\0";

                        // 發送TCP數據
                        SendTCPData(client, CMD_U15);
                    }
                    break;

                default:
                    break;
            }
        }

        public void Px16_SendTestCMD(object obj, xe_U15_CMD Px16_CMD, uint u32Px1_FRQ,
                                                                      uint u32Px2_M1S,
                                                                      uint u32Px3_M1E,
                                                                      uint u32Px4_M2S,
                                                                      uint u32Px5_M2E,
                                                                      uint u32Px6_M3S,
                                                                      uint u32Px7_M3E,
                                                                      uint u32Px8_M4S,
                                                                      uint u32Px9_M4E,
                                                                      uint u32Px10_MDS,
                                                                      uint u32Px11_MDE,
                                                                      uint u32Px12_M1P,
                                                                      uint u32Px13_M2P,
                                                                      uint u32Px14_M3P,
                                                                      uint u32Px15_M4P,
                                                                      uint u32Px16_MDP)
        {
            switch (Px16_CMD)
            {
                case xe_U15_CMD.xeUC_TestMode_Parameters:
                    {
                        const uint u32ParaCount = 16;

                        // 構建指令字串
                        string CMD_U15 = $"{strCMDU15Header}{(uint)Px16_CMD},{(uint)u32ParaCount},{(uint)u32Px1_FRQ},{(uint)u32Px2_M1S},{(uint)u32Px3_M1E},{(uint)u32Px4_M2S},{(uint)u32Px5_M2E},{(uint)u32Px6_M3S},{(uint)u32Px7_M3E},{(uint)u32Px8_M4S},{(uint)u32Px9_M4E},{(uint)u32Px10_MDS},{(uint)u32Px11_MDE},{(uint)u32Px12_M1P},{(uint)u32Px13_M2P},{(uint)u32Px14_M3P},{(uint)u32Px15_M4P},{(uint)u32Px16_MDP},\r\n\0";

                        // 發送TCP數據
                        SendTCPData(obj, CMD_U15);
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetVibration(uint u32Px1_FRQ, uint u32Px2_M1S,
                                                  uint u32Px3_M1E,
                                                  uint u32Px4_M2S,
                                                  uint u32Px5_M2E,
                                                  uint u32Px6_M3S,
                                                  uint u32Px7_M3E,
                                                  uint u32Px8_M4S,
                                                  uint u32Px9_M4E,
                                                  uint u32Px10_MDS,
                                                  uint u32Px11_MDE,
                                                  uint u32Px12_M1P,
                                                  uint u32Px13_M2P,
                                                  uint u32Px14_M3P,
                                                  uint u32Px15_M4P,
                                                  uint u32Px16_MDP)
        {

            // 確認變量數值
            if (u32Px1_FRQ > 1500) u32Px1_FRQ = 1500;
            if (u32Px2_M1S > 1000) u32Px2_M1S = 1000;
            if (u32Px3_M1E > 1000) u32Px3_M1E = 1000;
            if (u32Px4_M2S > 1000) u32Px4_M2S = 1000;
            if (u32Px5_M2E > 1000) u32Px5_M2E = 1000;
            if (u32Px6_M3S > 1000) u32Px6_M3S = 1000;
            if (u32Px7_M3E > 1000) u32Px7_M3E = 1000;
            if (u32Px8_M4S > 1000) u32Px8_M4S = 1000;
            if (u32Px9_M4E > 1000) u32Px9_M4E = 1000;
            if (u32Px10_MDS > 1000) u32Px10_MDS = 1000;
            if (u32Px11_MDE > 1000) u32Px11_MDE = 1000;
            if (u32Px12_M1P > 1000) u32Px12_M1P = 1000;
            if (u32Px13_M2P > 1000) u32Px13_M2P = 1000;
            if (u32Px14_M3P > 1000) u32Px14_M3P = 1000;
            if (u32Px15_M4P > 1000) u32Px15_M4P = 1000;
            if (u32Px16_MDP > 1000) u32Px16_MDP = 1000;

            // 儲存變量
            uint u32SaveFrequency = 0;
            uint u32SaveVibrationSource1_StartPhase = 0;
            uint u32SaveVibrationSource1_StopPhase = 0;
            uint u32SaveVibrationSource2_StartPhase = 0;
            uint u32SaveVibrationSource2_StopPhase = 0;
            uint u32SaveVibrationSource3_StartPhase = 0;
            uint u32SaveVibrationSource3_StopPhase = 0;
            uint u32SaveVibrationSource4_StartPhase = 0;
            uint u32SaveVibrationSource4_StopPhase = 0;
            uint u32SaveBlackDepotSource_StartPhase = 0;
            uint u32SaveBlackDepotSource_StopPhase = 0;
            uint u32SaveVibrationSource1_Power = 0;
            uint u32SaveVibrationSource2_Power = 0;
            uint u32SaveVibrationSource3_Power = 0;
            uint u32SaveVibrationSource4_Power = 0;
            uint u32SaveBlackDepotSource_Power = 0;

            // 檢查變量是否變更
            if (u32SaveFrequency != u32Px1_FRQ ||
                 u32SaveVibrationSource1_StartPhase != u32Px2_M1S ||
                 u32SaveVibrationSource1_StopPhase != u32Px3_M1E ||
                 u32SaveVibrationSource2_StartPhase != u32Px4_M2S ||
                 u32SaveVibrationSource2_StopPhase != u32Px5_M2E ||
                 u32SaveVibrationSource3_StartPhase != u32Px6_M3S ||
                 u32SaveVibrationSource3_StopPhase != u32Px7_M3E ||
                 u32SaveVibrationSource4_StartPhase != u32Px8_M4S ||
                 u32SaveVibrationSource4_StopPhase != u32Px9_M4E ||
                 u32SaveBlackDepotSource_StartPhase != u32Px10_MDS ||
                 u32SaveBlackDepotSource_StopPhase != u32Px11_MDE ||
                 u32SaveVibrationSource1_Power != u32Px12_M1P ||
                 u32SaveVibrationSource2_Power != u32Px13_M2P ||
                 u32SaveVibrationSource3_Power != u32Px14_M3P ||
                 u32SaveVibrationSource4_Power != u32Px15_M4P ||
                 u32SaveBlackDepotSource_Power != u32Px16_MDP)
            {

                // 更新保存的變量
                u32SaveFrequency = u32Px1_FRQ;
                u32SaveVibrationSource1_StartPhase = u32Px2_M1S;
                u32SaveVibrationSource1_StopPhase = u32Px3_M1E;
                u32SaveVibrationSource2_StartPhase = u32Px4_M2S;
                u32SaveVibrationSource2_StopPhase = u32Px5_M2E;
                u32SaveVibrationSource3_StartPhase = u32Px6_M3S;
                u32SaveVibrationSource3_StopPhase = u32Px7_M3E;
                u32SaveVibrationSource4_StartPhase = u32Px8_M4S;
                u32SaveVibrationSource4_StopPhase = u32Px9_M4E;
                u32SaveBlackDepotSource_StartPhase = u32Px10_MDS;
                u32SaveBlackDepotSource_StopPhase = u32Px11_MDE;
                u32SaveVibrationSource1_Power = u32Px12_M1P;
                u32SaveVibrationSource2_Power = u32Px13_M2P;
                u32SaveVibrationSource3_Power = u32Px14_M3P;
                u32SaveVibrationSource4_Power = u32Px15_M4P;
                u32SaveBlackDepotSource_Power = u32Px16_MDP;

                // 發送命令
                Console.WriteLine("發送命令");
                Px16_SendTestCMD(client, xe_U15_CMD.xeUC_TestMode_Parameters, u32SaveFrequency,
                                                                              u32SaveVibrationSource1_StartPhase,
                                                                              u32SaveVibrationSource1_StopPhase,
                                                                              u32SaveVibrationSource2_StartPhase,
                                                                              u32SaveVibrationSource2_StopPhase,
                                                                              u32SaveVibrationSource3_StartPhase,
                                                                              u32SaveVibrationSource3_StopPhase,
                                                                              u32SaveVibrationSource4_StartPhase,
                                                                              u32SaveVibrationSource4_StopPhase,
                                                                              u32SaveBlackDepotSource_StartPhase,
                                                                              u32SaveBlackDepotSource_StopPhase,
                                                                              u32SaveVibrationSource1_Power,
                                                                              u32SaveVibrationSource2_Power,
                                                                              u32SaveVibrationSource3_Power,
                                                                              u32SaveVibrationSource4_Power,
                                                                              u32SaveBlackDepotSource_Power);
            }
        }


        public void SetVibrationLED(uint u32LEDLevel)
        {
            Console.WriteLine("更新 LED 等級");
            Px1_SendCMD(xe_U15_CMD.xeUC_TestMode_LightLevel, u32LEDLevel);
        }

        public void SendTCPData(object obj, string data)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            if (client != null && stream != null)
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(data);
                stream.Write(bytesToSend, 0, bytesToSend.Length);
            }
        }

    }  // end of internal class Vibration
}
