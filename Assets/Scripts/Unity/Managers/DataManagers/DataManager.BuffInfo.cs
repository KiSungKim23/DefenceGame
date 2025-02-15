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
        private Dictionary<int , Logic.BuffInfoScript> _buffinfoDictionary = new Dictionary<int , Logic.BuffInfoScript>();

        public void LoadBuffInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "BuffInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                List<Logic.BuffInfoScript> dataList = JsonConvert.DeserializeObject<List<Logic.BuffInfoScript>>(json);

                _buffinfoDictionary = dataList.ToDictionary(_ => _.buffUID);
            }
            else
            {
                Debug.LogError("BuffInfoScript파일을 찾을 수 없습니다");
            }
        }

        public Dictionary<int , Logic.BuffInfoScript> GetBuffInfoScriptDictionaryAll()
        {
            return _buffinfoDictionary;
        }

        public Logic.BuffInfoScript GetBuffInfoScriptDictionary(int keyData)
        {
            if(_buffinfoDictionary.TryGetValue(keyData, out var ret))
            {
                return ret;
            }
            return null;
        }
    }
}
