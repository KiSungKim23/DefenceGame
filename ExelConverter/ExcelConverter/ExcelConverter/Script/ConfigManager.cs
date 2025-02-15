using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConverter.Script
{
    public class ConfigData
    {
        public string ExcelPath;
        public string ClientPath;
        public string ServerPath;
    }

    public class ConfigManager
    {
        public Action<string> MessageBoxCreate;

        private static string configFilePath = "config.json";

        public void SaveConfig(ConfigData config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configFilePath, json);
                Console.WriteLine($"설정 저장 완료: {configFilePath}");
            }
            catch (Exception ex)
            {
                MessageBoxCreate.Invoke($"설정 저장 실패: {ex.Message}");
            }
        }


        public ConfigData LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string json = File.ReadAllText(configFilePath);
                    return JsonConvert.DeserializeObject<ConfigData>(json);
                }
                else
                {
                    return new ConfigData();
                }
            }
            catch (Exception ex)
            {
                MessageBoxCreate($"설정 불러오기 실패: {ex.Message}");
                return new ConfigData();
            }
        }

    }
}
