using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Client
{

    public partial class DataManager : Logic.IDataManager
    {
        private List< Logic.SkillBuffInfoScript> _skillbuffinfoList = new List< Logic.SkillBuffInfoScript>();

        public void LoadSkillBuffInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "SkillBuffInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _skillbuffinfoList = JsonConvert.DeserializeObject<List<Logic.SkillBuffInfoScript>>(json);
            }
            else
            {
                Debug.LogError("SkillBuffInfoScript파일을 찾을 수 없습니다");
            }
        }

        public List<Logic.SkillBuffInfoScript> GetSkillBuffInfoScriptListAll()
        {
            return _skillbuffinfoList;
        }
    }
}
