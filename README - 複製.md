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
            // ��l�ƦC��M�����|
            strFileName = strNameFile; // Set the file name
            JsonNeedleContentList = new List<JsonNeedleContent>();
            GenerateFilePath();

            // �p�G���s�b�AŪ���{���ƾ�
            if (File.Exists(FilePath)) {
                try {
                    var jsonString = File.ReadAllText(FilePath);
                    JsonNeedleContentList = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString) ?? new List<JsonNeedleContent>();
                    Console.WriteLine("���\��l�� JSON ���C");
                } catch (Exception ex) {
                    Console.WriteLine("Ū�� JSON ���ɵo�Ϳ��~: " + ex.Message);
                }
            } else {
                Console.WriteLine("��󤣦s�b�A��l�Ƭ��ŦC��C");
            }
        }  //end of public void InitialJsonFile(string strNameFile) {
        public string GenerateFilePath() {
            FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strFileName);
            return FilePath;
        }  //end of public string GenerateFilePath() {
        public void AddJsonContent(JsonNeedleContent newContent) {
            if (newContent == null) {
                throw new ArgumentNullException(nameof(newContent), "�s���e���ର��");
            }

            JsonNeedleContentList.Add(newContent);
            WriteDataToJsonFile();
            Console.WriteLine("���\�K�[�s�� JSON ���e�ç�s���C");
        }  //end of public void AddJsonContent(JsonNeedleContent newContent) {
        public void WriteDataToJsonFile() {
            try {
                // ���ƧǨåh��
                SortAndRemoveDuplicates();

                // �ǦC�ƦC�� JSON �r��
                string jsonString = JsonSerializer.Serialize(JsonNeedleContentList, new JsonSerializerOptions { WriteIndented = true });

                // ��X�ǦC�Ƶ��G�챱��x�A���U�ո�
                Console.WriteLine("��s�᪺ JSON ��󤺮e:\n" + jsonString);

                // �g�J JSON �r�����
                File.WriteAllText(FilePath, jsonString);

                Console.WriteLine("���\�g�J JSON ���C");
            } catch (Exception ex) {
                // �B�z���~�A�Ҧp�O�����~�H��
                Console.WriteLine("�g�J JSON ���ɵo�Ϳ��~: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile() {
        public void SortAndRemoveDuplicates() {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any()) {
                Console.WriteLine("�C���šA�L�ݱƧǩM�h���C");
                return;
            }

            // �ϥ� LINQ �h���ñƧ�
            JsonNeedleContentList = JsonNeedleContentList
                .GroupBy(p => p.u32Index)
                .Select(g => g.First())    // ���C�Ӳժ��Ĥ@�Ӷ���
                .OrderBy(p => p.u32Index)  // �� u32Index �Ƨ�
                .ToList();

            Console.WriteLine("�ƧǨåh�������C");
        }  //end of public void SortAndRemoveDuplicates() {
        public void RemoveJsonContentByIndex(uint u32Index) {
            if (JsonNeedleContentList == null || !JsonNeedleContentList.Any()) {
                Console.WriteLine("�C���šA�L�ݧR�����e�C");
                return;
            }

            // �ϥ� RemoveAll ��k�ӧR���Ҧ��ŦX���󪺶���
            int removedCount = JsonNeedleContentList.RemoveAll(p => p.u32Index == u32Index);

            if (removedCount > 0) {
                // �p�G�����سQ�R���A�h�g�J��s�᪺�C�����
                WriteDataToJsonFile();
                Console.WriteLine($"���\�R�� {removedCount} �� u32Index �� {u32Index} �����ءC");
            } else {
                // �p�G�����ǰt�����ءA��ܴ���
                Console.WriteLine($"����� u32Index �� {u32Index} �����ءC");
            }
        }  //end of public void RemoveJsonContentByIndex(uint u32Index) {
        public void WriteDataToJsonFile_Test() {
            // �ϥΥ��T�������ӳЫع�H
            List<JsonNeedleContent> TestReadWriteJson = new List<JsonNeedleContent>
            {
                new JsonNeedleContent { strLabel = "PinA", u32Index =  0, dblXCoordinate = 1.234, dblYCoordinate = 5.678, bReplace = true, bVisible = true },
                new JsonNeedleContent { strLabel = "SSKR", u32Index =  3, dblXCoordinate = 2.345, dblYCoordinate = 6.789, bReplace = true, bVisible = true },
                new JsonNeedleContent { strLabel = "FUCK", u32Index = 18, dblXCoordinate = 3.456, dblYCoordinate = 7.890, bReplace = true, bVisible = true },
            };

            // �ǦC�ƹ�H�� JSON �r��
            string jsonString = JsonSerializer.Serialize(TestReadWriteJson, new JsonSerializerOptions { WriteIndented = true });

            // �T�O�ؿ��s�b�A�p�G���s�b�h�Ы�
            string filePath = GenerateFilePath();

            try {
                // �g�J JSON �r�����
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine("���ռƾڦ��\�g�J JSON ���C");
            } catch (Exception ex) {
                // ����ÿ�X�ԲӪ����~�H��
                Console.WriteLine("���ռƾڼg�J JSON ���ɵo�Ϳ��~: " + ex.Message);
            }
        }  //end of public void WriteDataToJsonFile_Test() {
        public string ReadIndexFromJsonFile(uint u32Index) {
            string rslt = "";

            try {
                // Ū�� JSON ��󤺮e
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // �p�G JSON �r�ꤣ���šA�h�i��ϧǦC��
                if (!string.IsNullOrEmpty(jsonString)) {
                    // �ϧǦC�� JSON �r�ꬰ List<JsonContent> ��H
                    List<JsonNeedleContent> TestReadWriteJson = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString);

                    // �d�� u32_Index
                    JsonNeedleContent GotNeedle = TestReadWriteJson?.FirstOrDefault(p => p.u32Index == u32Index);

                    if (GotNeedle != null) {
                        // ���A��X�H��
                        rslt = $"{GotNeedle.strLabel} {GotNeedle.u32Index} {GotNeedle.dblXCoordinate} {GotNeedle.dblYCoordinate}";
                        Console.WriteLine("�d�ߵ��G: " + rslt);
                    } else {
                        // �S��캡������
                        Console.WriteLine("�䤣��ŦX����");
                    }
                } else {
                    // �S���
                    Console.WriteLine("��󤣦s�b�Τ�󤺮e����");
                }
            } catch (Exception ex) {
                // ����ÿ�X�ԲӪ����~�H��
                Console.WriteLine("Ū�� JSON ���ɵo�Ϳ��~: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadIndexFromJsonFile(uint u32Index) {
        public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
            string rslt = "";

            try {
                // Ū�� JSON ��󤺮e
                string jsonString = File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;

                // �p�G JSON �r�ꤣ���šA�h�i��ϧǦC��
                if (!string.IsNullOrEmpty(jsonString)) {
                    // �ϧǦC�� JSON �r�ꬰ List<JsonContent> ��H
                    List<JsonNeedleContent> Needle = JsonSerializer.Deserialize<List<JsonNeedleContent>>(jsonString);

                    // �ھ�X�MY�d��L�o
                    var filteredNeedle = Needle?.Where(p => p.dblXCoordinate >= min_X && p.dblXCoordinate <= max_X &&
                                                            p.dblYCoordinate >= min_Y && p.dblYCoordinate <= max_Y);

                    if (filteredNeedle != null && filteredNeedle.Any()) {
                        // ��캡������
                        rslt = string.Join("", filteredNeedle.Select(p => $"{p.strLabel} (u32Index: {p.u32Index}, X: {p.dblXCoordinate}, Y: {p.dblYCoordinate}) \r\n"));
                        Console.WriteLine("�z�ﵲ�G:\n" + rslt);
                    } else {
                        // �S��캡������
                        Console.WriteLine("�䤣��ŦX����");
                    }
                } else {
                    // �S���
                    Console.WriteLine("��󤣦s�b�Τ�󤺮e����");
                }
            } catch (Exception ex) {
                // ����ÿ�X�ԲӪ����~�H��
                Console.WriteLine("Ū�� JSON ���ɵo�Ϳ��~: " + ex.Message);
            }

            return rslt;
        }  //end of public string ReadRangeDataFetcherFromJsonFile(double max_X, double min_X, double max_Y, double min_Y) {
    }  //end of public class JsonNeedleHandle { 


