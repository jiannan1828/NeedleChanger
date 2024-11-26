//Profile of Needle 
    public class JsonNeedleContent {
        public string strLabel { get; set; }
        public uint u32Index { get; set; }
        public double dblXCoordinate { get; set; }
        public double dblYCoordinate { get; set; }
        public bool bReplace { get; set; }
        public bool bVisible { get; set; }
    }  //end of public void RoundCoordinates(int decimalPlaces) {

    //Handle of Needle 
    public class JsonNeedleHandle { 
        public List<JsonNeedleContent> JsonNeedleContentList { get; set; }
        public string FilePath { get; set; }
        public string strFileName { get; set; }

        public void InitialJsonFile(string strNameFile) {
            // 初始化列表和文件路徑
            strFileName = strNameFile; // Set the file name
            JsonNeedleContentList = new List<JsonNeedleContent>();
            GenerateFilePath();

            // 如果文件存在，讀取現有數據
            if (File.Exists(FilePath)) {
                try {
                    var jsonString = File.ReadAllText(FilePath);
                    JsonNeedleContentList = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString) ?? new List<JsonNeedleContent>();
                    Console.WriteLine("成功初始化 JSON 文件。");
                } catch (Exception ex) {
                    Console.WriteLine("讀取 JSON 文件時發生錯誤: " + ex.Message);
                }
            } else {
                Console.WriteLine("文件不存在，初始化為空列表。");
            }
        }  //end of public void InitialJsonFile(string strNameFile) {
        public string GenerateFilePath() {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            return FilePath;
        }  //end of public string GenerateFilePath() {
        public void AddJsonContent(JsonNeedleContent newContent) {
            if (newContent == null) {
                throw new ArgumentNullException(nameof(newContent), "新內容不能為空");
            }

            JsonNeedleContentList.Add(newContent);
            WriteDataToJsonFile();
            Console.WriteLine("成功添加新的 JSON 內容並更新文件。");
        }  //end of public void AddJsonContent(JsonNeedleContent newContent) {
        public void WriteDataToJsonFile() {
            try {
                // 先排序並去重
                SortAndRemoveDuplicates();

                // 序列化列表為 JSON 字串
                string jsonString = JsonSerializer.Serialize(JsonNeedleContentList, new JsonSerializerOptions { WriteIndented = true });

                // 輸出序列化結果到控制台，幫助調試
                Console.WriteLine("更新後的 JSON 文件內容:\n" + jsonString);

                // 寫入 JSON 字串到文件
                File.WriteAllText(FilePath, jsonString);

                Console.WriteLine("成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 處理錯誤，例如記錄錯誤信息
                Console.WriteLine("寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile() {
        public void SortAndRemoveDuplicates() {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any()) {
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
        public void RemoveJsonContentByIndex(uint u32Index) {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any()) {
                Console.WriteLine("列表為空，無需刪除內容。");
                return;
            }

            // 使用 RemoveAll 方法來刪除所有符合條件的項目
            int removedCount = JsonNeedleContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0) {
                // 如果有項目被刪除，則寫入更新後的列表到文件
                WriteDataToJsonFile();
                Console.WriteLine($"成功刪除 {removedCount} 個 u32Index 為 {u32Index} 的項目。");
            } else {
                // 如果未找到匹配的項目，顯示提示
                Console.WriteLine($"未找到 u32Index 為 {u32Index} 的項目。");
            }
        }  //end of public void RemoveJsonContentByIndex(uint u32Index) {
        public void WriteDataToJsonFile_Test() {
            // 使用正確的類型來創建對象
            List<JsonNeedleContent> TestReadWriteJson = new List<JsonNeedleContent>
            {
                new JsonNeedleContent { strLabel = "PinA", u32Index =  0, dblXCoordinate = 1.234, dblYCoordinate = 5.678, bReplace = true, bVisible = true },
                new JsonNeedleContent { strLabel = "SSKR", u32Index =  3, dblXCoordinate = 2.345, dblYCoordinate = 6.789, bReplace = true, bVisible = true },
                new JsonNeedleContent { strLabel = "FUCK", u32Index = 18, dblXCoordinate = 3.456, dblYCoordinate = 7.890, bReplace = true, bVisible = true },
            };

            // 序列化對象為 JSON 字串
            string jsonString = JsonSerializer.Serialize(TestReadWriteJson, new JsonSerializerOptions { WriteIndented = true });

            // 確保目錄存在，如果不存在則創建
            string filePath = GenerateFilePath();

            try {
                // 寫入 JSON 字串到文件
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine("測試數據成功寫入 JSON 文件。");
            } catch (Exception ex) {
                // 捕獲並輸出詳細的錯誤信息
                Console.WriteLine("測試數據寫入 JSON 文件時發生錯誤: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile_Test() {
        public string ReadIndexFromJsonFile(uint u32Index) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonNeedleContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString);

                    // 查找 u32_Index
                    JsonNeedleContent GotNeedle = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32Index);

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
        }  //end of public string ReadIndexFromJsonFile(uint u32Index) {
        public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
            string rslt = "";

            try {
                // 讀取 JSON 文件內容
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // 如果 JSON 字串不為空，則進行反序列化
                if (!string.IsNullOrEmpty(jsonString)) {
                    // 反序列化 JSON 字串為 List<JsonContent> 對象
                    List<JsonNeedleContent> Needle = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString);

                    // 根據X和Y範圍過濾
                    var filteredNeedle = Needle?.Where(p => p.dblXCoordinate >= min_X && p.dblXCoordinate <= max_X &&
                                                            p.dblYCoordinate >= min_Y && p.dblYCoordinate <= max_Y);

                    if (filteredNeedle != null && filteredNeedle.Any()) {
                        // 找到滿足條件的
                        rslt = string.Join("", filteredNeedle.Select(p => $"{p.strLabel} (u32Index: {p.u32Index}, X: {p.dblXCoordinate}, Y: {p.dblYCoordinate}) \r\n"));
                        Console.WriteLine("篩選結果:\n" + rslt);
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
        }  //end of public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
    }  //end of public class JsonNeedleHandle { 


