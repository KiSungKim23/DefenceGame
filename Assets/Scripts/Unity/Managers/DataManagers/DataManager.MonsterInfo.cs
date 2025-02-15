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
        private Dictionary<int , Logic.MonsterInfoScript> _monsterinfoDictionary = new Dictionary<int , Logic.MonsterInfoScript>();

        public void LoadMonsterInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "MonsterInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                List<Logic.MonsterInfoScript> dataList = JsonConvert.DeserializeObject<List<Logic.MonsterInfoScript>>(json);

                _monsterinfoDictionary = dataList.ToDictionary(_ => _.monsterUID);
            }
            else
            {
                Debug.LogError("MonsterInfoScript파일을 찾을 수 없습니다");
            }
        }

        public Dictionary<int , Logic.MonsterInfoScript> GetMonsterInfoScriptDictionaryAll()
        {
            return _monsterinfoDictionary;
        }

        public Logic.MonsterInfoScript GetMonsterInfoScriptDictionary(int keyData)
        {
            if(_monsterinfoDictionary.TryGetValue(keyData, out var ret))
            {
                return ret;
            }
            return null;
        }
    }
}
