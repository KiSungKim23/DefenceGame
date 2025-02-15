using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConverter.Script
{

    public static class GenerateCode
    {
        public static string GenerateClientClassCode(string className, List<VariableInfo> fieldDefs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using MessagePack;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace Logic");
            sb.AppendLine("{");
            sb.AppendLine("    [Serializable]");
            sb.AppendLine("    [MessagePackObject]");
            sb.AppendLine($"    public class {className}Script");
            sb.AppendLine("    {");

            foreach (var fd in fieldDefs)
            {
                sb.AppendLine($"        [Key({fd.KeyIndex})] public {fd.variableType} {fd.variableName};");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateClientDataManagerListCode(string jsonPath, string className, List<VariableInfo> fieldDefs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace Client");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    public partial class DataManager : Logic.IDataManager");
            sb.AppendLine("    {");
            sb.AppendLine($"        private List< Logic.{className}Script> _{className.ToLower()}List = new List< Logic.{className}Script>();");
            sb.AppendLine();
            sb.AppendLine($"        public void Load{className}Script()");
            sb.AppendLine("        {");
            sb.AppendLine($"            string filePath = Path.Combine(@\"{jsonPath}\", \"{className}.json\");");
            sb.AppendLine();
            sb.AppendLine("            if (File.Exists(filePath))");
            sb.AppendLine("            {");
            sb.AppendLine("                string json = File.ReadAllText(filePath);");
            sb.AppendLine($"                _{className.ToLower()}List = JsonConvert.DeserializeObject<List<Logic.{className}Script>>(json);");
            sb.AppendLine("            }");
            sb.AppendLine("            else");
            sb.AppendLine("            {");
            sb.AppendLine($"                Debug.LogError(\"{className}Script파일을 찾을 수 없습니다\");");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public List<Logic.{className}Script> Get{className}ScriptListAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return _{className.ToLower()}List;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateClientDataManagerDictionryCode(string jsonPath, string className, List<VariableInfo> fieldDefs, string keyName, string keyType)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine();
            sb.AppendLine("namespace Client");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    public partial class DataManager : Logic.IDataManager");
            sb.AppendLine("    {");
            sb.AppendLine($"        private Dictionary<{keyType} , Logic.{className}Script> _{className.ToLower()}Dictionary = new Dictionary<{keyType} , Logic.{className}Script>();");
            sb.AppendLine();
            sb.AppendLine($"        public void Load{className}Script()");
            sb.AppendLine("        {");
            sb.AppendLine($"            string filePath = Path.Combine(@\"{jsonPath}\", \"{className}.json\");");
            sb.AppendLine();
            sb.AppendLine("            if (File.Exists(filePath))");
            sb.AppendLine("            {");
            sb.AppendLine("                string json = File.ReadAllText(filePath);");
            sb.AppendLine();
            sb.AppendLine($"                List<Logic.{className}Script> dataList = JsonConvert.DeserializeObject<List<Logic.{className}Script>>(json);");
            sb.AppendLine();
            sb.AppendLine($"                _{className.ToLower()}Dictionary = dataList.ToDictionary(_ => _.{keyName});");
            sb.AppendLine("            }");
            sb.AppendLine("            else");
            sb.AppendLine("            {");
            sb.AppendLine($"                Debug.LogError(\"{className}Script파일을 찾을 수 없습니다\");");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public Dictionary<{keyType} , Logic.{className}Script> Get{className}ScriptDictionaryAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return _{className.ToLower()}Dictionary;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public Logic.{className}Script Get{className}ScriptDictionary({keyType} keyData)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if(_{className.ToLower()}Dictionary.TryGetValue(keyData, out var ret))");
            sb.AppendLine("            {");
            sb.AppendLine("                return ret;");
            sb.AppendLine("            }");
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string GenerateClientDataManagerCoreCode(List<(string, string)> allScriptClassList)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using MessagePack;");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace Client");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    public partial class DataManager : Logic.IDataManager");
            sb.AppendLine("    {");
            sb.AppendLine("        public void Init()");
            sb.AppendLine("        {");
            foreach (var className in allScriptClassList)
            {
                sb.AppendLine($"            Load{className.Item2}Script();");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
        public static string GenerateClientIDataManagerCode(List<(string, string)> allScriptClassList)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("namespace Logic");
            sb.AppendLine("{");
            sb.AppendLine();
            sb.AppendLine("    public interface IDataManager ");
            sb.AppendLine("    {");
            foreach (var className in allScriptClassList)
            {
                if(className.Item1.Contains("None"))
                {
                    sb.AppendLine($"            public List<{className.Item2}Script> Get{className.Item2}ScriptListAll();");
                }
                else
                {
                    sb.AppendLine($"            public Dictionary<{className.Item1} , Logic.{className.Item2}Script> Get{className.Item2}ScriptDictionaryAll();");
                    sb.AppendLine($"            public {className.Item2}Script Get{className.Item2}ScriptDictionary({className.Item1} keyData);");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
