using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static InjectorInspector.Program;

//---------------------------------------------------------------------------------------
//JSON
using System.IO;
using System.Text.Json;
using System.Reflection.Emit;

namespace InjectorInspector
{
    public partial class ReplaceForm : Form
    {
        // 定義一個 BindingList 用來綁定到 DataGridView
        public BindingList<JsonReplaceModeContent> jsonContentList;

        // 在 Form 類中創建 apiJsonReplaceModeHandle 的實例
        public apiJsonReplaceModeHandle apiHandler;

        public ReplaceForm()
        {
            InitializeComponent();
        }

        public void ReplaceForm_Load(object sender, EventArgs e)
        {
            this.Text = "ReplaceForm_L=123";

            //當表單開啟時，讀json檔案
            if(true)
            {
                // 初始化 apiJsonReplaceModeHandle，並指定檔案名稱
                if (apiHandler == null)
                {
                    // 如果 apiHandler 是 null，進行初始化
                    apiHandler = new apiJsonReplaceModeHandle();
                }

                // 指定檔案名稱並初始化 JSON 文件
                apiHandler.InitialJsonFile("SaveReplaceJason.json");

                // 使用 apiJsonReplaceModeHandle 中的 JsonNeedleContentList 資料
                jsonContentList = new BindingList<JsonReplaceModeContent>(apiHandler.JsonNeedleContentList);

                // 綁定資料到 DataGridView
                dataGridView1.DataSource = jsonContentList;

                // 設定 DataGridView 顯示格式
                dataGridView1.AutoGenerateColumns = true;  // 自動生成欄位
                dataGridView1.AllowUserToAddRows = true;  // 允許使用者新增資料
                dataGridView1.AllowUserToDeleteRows = true;  // 允許刪除行
                dataGridView1.ReadOnly = false;            // 允許編輯資料

                // 創建 ComboBox Column 並設置資料來源
                var comboBoxColumn = new DataGridViewComboBoxColumn();
                comboBoxColumn.Name = "strLabel";
                comboBoxColumn.HeaderText = "Label";
                comboBoxColumn.DataSource = new List<string> { 
                    "吸嘴X軸",
                    "吸嘴Y軸",
                    "吸嘴Z軸",
                    "吸嘴R軸",
                    "載盤X軸",
                    "載盤Y軸",
                    "植針Z軸",
                    "植針R軸",
                    "工作門",
                    "Socket檢測",
                    "3D掃描",
                    "吸針嘴",
                    "植針嘴"
                }; // 下拉選單選項
                comboBoxColumn.DataPropertyName = "strLabel";  // 綁定到資料屬性

                // 移除自動生成的 strLabel 欄位，並使用 ComboBox 欄位替換
                if (dataGridView1.Columns.Contains("strLabel"))
                {
                    dataGridView1.Columns.Remove("strLabel");
                }

                // 添加新的 ComboBox 欄位
                dataGridView1.Columns.Add(comboBoxColumn);

                // 將 strLabel 欄位移至第二格
                dataGridView1.Columns["strLabel"].DisplayIndex = 1;

                //Note資料寬度為400
                dataGridView1.Columns["strNote"].Width = 400;

                // 設定 DataGridView 的 DataError 事件處理
                dataGridView1.DataError += DataGridView1_DataError;

                // 設定 CellDoubleClick 事件來捕獲雙擊格子的操作
                if(false)dataGridView1.CellClick += DataGridView1_CellClick;
            }
        }

        public void ReplaceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Text = "ReplaceForm_L=456";

            form1.btn_置換.Enabled = true;
        }

        public void btn_Save_Click(object sender, EventArgs e)
        {
            // 清空舊資料
            //apiHandler.JsonNeedleContentList.Clear();

            // 將 BindingList 中的資料添加到 apiHandler
            foreach (var item in jsonContentList)
            {
                apiHandler.AddJsonContent(item, bForceWriteConflict: true); // true 表示會更新重複的項目
            }

            // 儲存資料至 JSON 檔案
            apiHandler.WriteDataToJsonFile();

           Console.WriteLine("資料已成功保存至 JSON 檔案！");
        }

        public void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // 這裡可以處理錯誤，避免應用崩潰
            // 比如說，若 ComboBox 中的值無效，可以重設為預設值
            if (e.Exception != null)
            {
                Console.WriteLine("DataGridView 發生錯誤: " + e.Exception.Message);

                // 你可以選擇重設為預設值
                //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "吸嘴R軸"; // 假設 "吸嘴R軸" 是有效的值
                e.ThrowException = false; // 防止異常拋出，讓應用繼續執行
            }
        }

        public void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 確保點擊的行和列是有效的
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // 獲取該行該列的單元格內容
                var cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // 獲取該行的所有欄位
                var rowValues = dataGridView1.Rows[e.RowIndex].Cells.Cast<DataGridViewCell>()
                    .Select(cell => cell.Value?.ToString() ?? "null")
                    .ToArray();

                // 打印所選擇的格子資料
                Console.WriteLine($"雙擊位置：行 {e.RowIndex}, 列 {e.ColumnIndex}, 資料：{cellValue}");

                // 打印該行所有資料
                Console.WriteLine("該行所有資料: ");
                foreach (var value in rowValues)
                {
                    Console.WriteLine(value);
                }
            }
        }
    }






    //Profile of ReplaceMode 
    public class JsonReplaceModeContent
    {
        public string strLabel { get; set; }
        public uint u32Index { get; set; }
        public double dbPosition { get; set; }
        public string strNote { get; set; }
    }  //end of public class JsonReplaceModeContent

    //Handle of ReplaceMode 
    public class apiJsonReplaceModeHandle
    {
        public List<JsonReplaceModeContent> JsonNeedleContentList { get; set; }
        public string FilePath { get; set; }
        public string strFileName { get; set; }

        public void InitialJsonFile(string strNameFile)
        {
            // 初始化列表和文件路徑
            strFileName = strNameFile;
            JsonNeedleContentList = new List<JsonReplaceModeContent>();
            GenerateFilePath();

            // 如果文件存在，讀取現有數據
            if (File.Exists(FilePath))
            {
                try
                {
                    var jsonString = File.ReadAllText(FilePath);
                    JsonNeedleContentList = JsonSerializer.Deserialize<List<JsonReplaceModeContent>>(jsonString) ?? new List<JsonReplaceModeContent>();
                    Console.WriteLine("成功初始化 JSON 文件。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("文件不存在，初始化為空列表。");
            }
        }  // end of public void InitialJsonFile(string strNameFile)

        public string GenerateFilePath()
        {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            return FilePath;
        }  //end of public string GenerateFilePath() {
        public void AddJsonContent(JsonReplaceModeContent newContent, bool bForceWriteConflict)
        {
            if (newContent == null)
            {
                throw new ArgumentNullException(nameof(newContent), "新內容不能為空");
            }

            // 嘗試查找是否有相同 u32Index 的項目
            var existingContent = JsonNeedleContentList.FirstOrDefault(p => p.u32Index == newContent.u32Index);

            if (existingContent == null)
            {
                // 如果沒有找到對應的項目，則新增一個新項目
                JsonNeedleContentList.Add(newContent);
                Console.WriteLine($"成功添加新的項目，u32Index 為 {newContent.u32Index}。");
            }
            else
            {
                // 如果找到對應的項目，根據 bForceWriteConflict 決定是否更新
                if (bForceWriteConflict == true)
                {
                    existingContent.strLabel = newContent.strLabel;
                    existingContent.dbPosition = newContent.dbPosition;
                    existingContent.strNote = newContent.strNote;
                    Console.WriteLine($"已更新 u32Index 為 {newContent.u32Index} 的項目。");
                }
            }

            // 更新 JSON 文件
            WriteDataToJsonFile();
        }  // end of public void AddJsonContent(JsonReplaceModeContent newContent, bool bForceWriteConflict)


        public void WriteDataToJsonFile()
        {
            try
            {
                // 先排序並去重
                SortAndRemoveDuplicates();

                // 序列化列表為 JSON 字串
                string jsonString = JsonSerializer.Serialize(JsonNeedleContentList, new JsonSerializerOptions { WriteIndented = true });

                // 輸出序列化結果到控制台，幫助調試
                Console.WriteLine("更新後的 JSON 文件內容:\n" + jsonString);

                // 寫入 JSON 字串到文件
                File.WriteAllText(FilePath, jsonString);

                Console.WriteLine("成功寫入 JSON 文件。");
            }
            catch (Exception ex)
            {
                // 處理錯誤，例如記錄錯誤信息
                Console.WriteLine("寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile() {
        public void SortAndRemoveDuplicates()
        {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any())
            {
                Console.WriteLine("列表為空，無需排序和去重。");
                return;
            }

            // 使用 LINQ 去重並排序
            JsonNeedleContentList = JsonNeedleContentList
                .GroupBy(p => p.u32Index)
                .Select(g => g.First())    // 取每個組的第一個項目
                .OrderBy(p => p.u32Index)  // 按 u32Index 排序
                .ToList();

            Console.WriteLine("排序並去重完成。");
        }  //end of public void SortAndRemoveDuplicates() {
        public void RemoveJsonContentByIndex(uint u32Index)
        {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any())
            {
                Console.WriteLine("列表為空，無需刪除內容。");
                return;
            }

            // 使用 RemoveAll 方法來刪除所有符合條件的項目
            int removedCount = JsonNeedleContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0)
            {
                // 如果有項目被刪除，則寫入更新後的列表到文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功刪除 {removedCount} 個 u32Index 為 {u32Index} 的項目。");
            }
            else
            {
                // 如果未找到匹配的項目，顯示提示
                Console.WriteLine($"未找到 u32Index 為 {u32Index} 的項目。");
            }
        }  //end of public void RemoveJsonContentByIndex(uint u32Index)
        public void UpdateJsonContentByIndex(uint u32Index, JsonReplaceModeContent updatedContent)
        {
            if (updatedContent == null)
            {
                throw new ArgumentNullException(nameof(updatedContent), "更新的內容不能為空");
            }

            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any())
            {
                Console.WriteLine("列表為空，無需更新內容。");
                return;
            }

            // 查找指定 u32Index 的項目
            var existingContent = JsonNeedleContentList.FirstOrDefault(p => p.u32Index == u32Index);

            if (existingContent != null)
            {
                // 找到對應的項目，進行更新
                existingContent.strLabel = updatedContent.strLabel;
                existingContent.dbPosition = updatedContent.dbPosition;
                existingContent.strNote = updatedContent.strNote;

                // 更新後，重新寫入文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功更新 u32Index 為 {u32Index} 的項目。");
            }
            else
            {
                Console.WriteLine($"找不到 u32Index 為 {u32Index} 的項目。");
            }
        }  // end of public void UpdateJsonContentByIndex(uint u32Index, JsonReplaceModeContent updatedContent)

        public JsonReplaceModeContent ReadJsonContentByIndex(uint u32Index)
        {
            JsonReplaceModeContent jtmcRSLT = new JsonReplaceModeContent();

            try
            {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString))
                {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonReplaceModeContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonReplaceModeContent>>(jsonString);

                    // 查找 u32_Index
                    JsonReplaceModeContent GotNeedle = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32Index);

                    if (GotNeedle != null)
                    {
                        // 找到，輸出信息
                        string rslt = "";
                        rslt = $"{GotNeedle.strLabel} {GotNeedle.u32Index} {GotNeedle.dbPosition} {GotNeedle.strNote}";

                        jtmcRSLT.strLabel = GotNeedle.strLabel;
                        jtmcRSLT.u32Index = GotNeedle.u32Index;
                        jtmcRSLT.dbPosition = GotNeedle.dbPosition;
                        jtmcRSLT.strNote = GotNeedle.strNote;

                        Console.WriteLine("查詢結果: " + rslt);
                    }
                    else
                    {
                        // 沒找到滿足條件的
                        Console.WriteLine("找不到符合條件的");
                    }
                }
                else
                {
                    // 沒文件
                    Console.WriteLine("文件不存在或文件內容為空");
                }
            }
            catch (Exception ex)
            {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
            }

            return jtmcRSLT;
        }  //end of public JsonReplaceModeContent ReadJsonContentByIndex(uint u32Index)

    }  //end of public class apiJsonReplaceModeHandle
}
