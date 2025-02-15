using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConverter.Script
{
    public class VariableInfo
    {
        public string variableType;  
        public string variableName;  
        public bool IsKey;         
        public int KeyIndex;      
    }


    public class ConvertClass
    {
        string _excelPath;

        string[] excelFiles;

        string clientDataManagerPath;
        string clientJsonPath;
        string clientIDataManagerPath;
        string clientDataClassPath;

        string serverDataManagerPath;
        string serverJsonPath;
        string serverIDataManagerPath;

        public Action<string> MessageBoxCreate;

        public void SetPathData(string excelPath, string clientPath, string serverPath)
        {
            _excelPath = excelPath;
            excelFiles = Directory.GetFiles(_excelPath, "*.xlsx");

            if (excelFiles.Length == 0)
            {
                MessageBoxCreate.Invoke("해당 폴더에 .xlsx 파일이 없습니다.");
                return;
            }
            clientDataManagerPath = clientPath + "\\Scripts\\Unity\\Managers\\DataManagers\\";
            clientJsonPath = clientPath + "\\Resources\\Scripts\\";
            clientIDataManagerPath = clientPath + "\\Scripts\\Logic\\Core\\DataManager\\";
            clientDataClassPath = clientPath + "\\Scripts\\Logic\\Core\\DataManager\\ScriptClass\\";

            serverDataManagerPath = serverPath;
            serverJsonPath = serverPath;
            serverIDataManagerPath = serverPath;
        }

        public void ConvertExcel()
        {
            List<(string, string)> allScriptClassLists = new List<(string, string)>();

            foreach (string excelFile in excelFiles)
            {
                var sheetDict = ReadExcelSheets(excelFile);

                foreach (var kvp in sheetDict)
                {
                    string sheetName = kvp.Key;   
                    DataTable table = kvp.Value; 

                    string className = sheetName.Trim();

                    var fieldDefs = ParseFieldDefinitions(table);
                    if (fieldDefs.Item1.Count == 0)
                    {
                        MessageBoxCreate.Invoke($"시트 '{sheetName}'에 필드 정의가 없습니다.");
                        continue;
                    }

                    var rowDataList = ParseRowData(table, fieldDefs.Item1);
                    string jsonString = JsonConvert.SerializeObject(rowDataList, Formatting.Indented);

                    string jsonFilePath = Path.Combine(clientJsonPath, $"{className}.json");
                    if (File.Exists(jsonFilePath)) File.Delete(jsonFilePath);
                    File.WriteAllText(jsonFilePath, jsonString);

                    string csClientClassCode = GenerateCode.GenerateClientClassCode(className, fieldDefs.Item1);
                    string csClientClassFilePath = Path.Combine(clientDataClassPath, $"{className}Script.cs");
                    if (File.Exists(csClientClassFilePath)) File.Delete(csClientClassFilePath);
                    File.WriteAllText(csClientClassFilePath, csClientClassCode);

                    string keyType = fieldDefs.Item2;

                    if (!fieldDefs.Item2.Contains("None"))
                    {
                        keyType = fieldDefs.Item1.Find(_ => _.variableName.Contains(fieldDefs.Item2)).variableType;
                        string csClientDataManagerCode = GenerateCode.GenerateClientDataManagerDictionryCode(clientJsonPath, className, fieldDefs.Item1, fieldDefs.Item2, keyType);
                        string csClientDataManagerFilePath = Path.Combine(clientDataManagerPath, $"DataManager.{className}.cs");
                        if (File.Exists(csClientDataManagerFilePath)) File.Delete(csClientDataManagerFilePath);
                        File.WriteAllText(csClientDataManagerFilePath, csClientDataManagerCode);
                    }
                    else
                    {
                        string csClientDataManagerCode = GenerateCode.GenerateClientDataManagerListCode(clientJsonPath, className, fieldDefs.Item1);
                        string csClientDataManagerFilePath = Path.Combine(clientDataManagerPath, $"DataManager.{className}.cs");
                        if (File.Exists(csClientDataManagerFilePath)) File.Delete(csClientDataManagerFilePath);
                        File.WriteAllText(csClientDataManagerFilePath, csClientDataManagerCode);
                    }


                    allScriptClassLists.Add((keyType, className));
                }
            }


            string csClientDataManagerCoreCode = GenerateCode.GenerateClientDataManagerCoreCode(allScriptClassLists);
            string csClientDataManagerCoreFilePath = Path.Combine(clientDataManagerPath, $"DataManager.Core.cs");
            if (File.Exists(csClientDataManagerCoreFilePath)) File.Delete(csClientDataManagerCoreFilePath);
            File.WriteAllText(csClientDataManagerCoreFilePath, csClientDataManagerCoreCode);

            string csClientIDataManagerCode = GenerateCode.GenerateClientIDataManagerCode(allScriptClassLists);
            string csClientIDataManagerFilePath = Path.Combine(clientIDataManagerPath, $"IDataManager.cs");
            if (File.Exists(csClientIDataManagerFilePath)) File.Delete(csClientIDataManagerFilePath);
            File.WriteAllText(csClientIDataManagerFilePath, csClientIDataManagerCode);

            MessageBoxCreate.Invoke("생성 완료");
        }

        public static Dictionary<string, DataTable> ReadExcelSheets(string filePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var resultDict = new Dictionary<string, DataTable>();

            if (!File.Exists(filePath))
                return resultDict;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();
                    foreach (DataTable table in result.Tables)
                    {
                        string sheetName = table.TableName;
                        resultDict[sheetName] = table;
                    }
                }
            }
            return resultDict;
        }
        public static (List<VariableInfo>, string) ParseFieldDefinitions(DataTable table)
        {
            var fieldDefs = new List<VariableInfo>();
            string keyName = "None";

            if (table.Rows.Count < 2)
            {
                return (fieldDefs, keyName);
            }

            DataRow typeRow = table.Rows[0]; 
            DataRow nameRow = table.Rows[1];

            int colCount = table.Columns.Count;
            int keyIndexCounter = 0;

            for (int col = 0; col < colCount; col++)
            {
                string fieldType = Convert.ToString(typeRow[col]).Trim(); 
                string rawName = Convert.ToString(nameRow[col]).Trim();  

                if (string.IsNullOrEmpty(fieldType) && string.IsNullOrEmpty(rawName))
                    continue;

                bool isKey = false;
                if (rawName.StartsWith("!"))
                {
                    isKey = true;
                    rawName = rawName.Substring(1);
                    keyName = rawName;
                }

                var fd = new VariableInfo
                {
                    variableType = fieldType,
                    variableName = rawName,
                    IsKey = isKey,
                    KeyIndex = keyIndexCounter++
                };
                fieldDefs.Add(fd);
            }

            return (fieldDefs, keyName);
        }

        public List<Dictionary<string, object>> ParseRowData(DataTable table, List<VariableInfo> fieldDefs)
        {
            var result = new List<Dictionary<string, object>>();

            for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++)
            {
                DataRow row = table.Rows[rowIndex];
                var dict = new Dictionary<string, object>();

                for (int colIndex = 0; colIndex < fieldDefs.Count; colIndex++)
                {
                    string fieldName = fieldDefs[colIndex].variableName;
                    string fieldType = fieldDefs[colIndex].variableType.ToLower();
                    object cellValue = row[colIndex];

                    if (cellValue != DBNull.Value)
                    {
                        cellValue = ConvertToCorrectType(cellValue, fieldType);
                    }
                    else
                    {
                        cellValue = GetDefaultValue(fieldType); 
                    }

                    dict[fieldName] = cellValue;
                }

                result.Add(dict);
            }

            return result;
        }


        public int GetExcelCount()
        {
            return excelFiles.Length;
        }
        private object ConvertToCorrectType(object value, string targetType)
        {
            try
            {
                if (targetType == "int")
                {
                    return Convert.ToInt32(value);
                }
                if (targetType == "long")
                {
                    return Convert.ToInt64(value);
                }
                if (targetType == "float")
                {
                    return Convert.ToSingle(value);
                }
                if (targetType == "double")
                {
                    return Convert.ToDouble(value);
                }
                if (targetType == "bool")
                {
                    return Convert.ToBoolean(value);
                }
                if (targetType == "string")
                {
                    return value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBoxCreate.Invoke($" 변환 실패: {value} → {targetType} ({ex.Message})");
            }

            return value; 
        }
        private static object GetDefaultValue(string fieldType)
        {
            switch(fieldType)
            {
                case "int":
                    return 0;
                case "long":
                    return 0L;
                case "float":
                    return 0f;
                case "double":
                    return 0.0;
                case "bool":
                    return false;
                case "string":
                    return "";
                default:
                    return null;
            }
        }
    }
}
