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
        private Dictionary<int , Logic.UnitInfoScript> _unitinfoDictionary = new Dictionary<int , Logic.UnitInfoScript>();

        public void LoadUnitInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "UnitInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                List<Logic.UnitInfoScript> dataList = JsonConvert.DeserializeObject<List<Logic.UnitInfoScript>>(json);

                _unitinfoDictionary = dataList.ToDictionary(_ => _.unitUID);
            }
            else
            {
                Debug.LogError("UnitInfoScript파일을 찾을 수 없습니다");
            }
        }

        public Dictionary<int , Logic.UnitInfoScript> GetUnitInfoScriptDictionaryAll()
        {
            return _unitinfoDictionary;
        }

        public Logic.UnitInfoScript GetUnitInfoScriptDictionary(int keyData)
        {
            if(_unitinfoDictionary.TryGetValue(keyData, out var ret))
            {
                return ret;
            }
            return null;
        }
    }
}
