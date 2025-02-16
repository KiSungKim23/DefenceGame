using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Client
{

    public partial class DataManager : Logic.IDataManager
    {
        private Dictionary<int , Logic.SkillInfoScript> _skillinfoDictionary = new Dictionary<int , Logic.SkillInfoScript>();

        public void LoadSkillInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "SkillInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                List<Logic.SkillInfoScript> dataList = JsonConvert.DeserializeObject<List<Logic.SkillInfoScript>>(json);

                _skillinfoDictionary = dataList.ToDictionary(_ => _.skillUID);
            }
            else
            {
                Debug.LogError("SkillInfoScript파일을 찾을 수 없습니다");
            }
        }

        public Dictionary<int , Logic.SkillInfoScript> GetSkillInfoScriptDictionaryAll()
        {
            return _skillinfoDictionary;
        }

        public Logic.SkillInfoScript GetSkillInfoScriptDictionary(int keyData)
        {
            if(_skillinfoDictionary.TryGetValue(keyData, out var ret))
            {
                return ret;
            }
            return null;
        }
    }
}
