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
        private Dictionary<int , Logic.UnitUnionInfoScript> _unitunioninfoDictionary = new Dictionary<int , Logic.UnitUnionInfoScript>();

        public void LoadUnitUnionInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "UnitUnionInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                List<Logic.UnitUnionInfoScript> dataList = JsonConvert.DeserializeObject<List<Logic.UnitUnionInfoScript>>(json);

                _unitunioninfoDictionary = dataList.ToDictionary(_ => _.unitUID);
            }
            else
            {
                Debug.LogError("UnitUnionInfoScript파일을 찾을 수 없습니다");
            }
        }

        public Dictionary<int , Logic.UnitUnionInfoScript> GetUnitUnionInfoScriptDictionaryAll()
        {
            return _unitunioninfoDictionary;
        }

        public Logic.UnitUnionInfoScript GetUnitUnionInfoScriptDictionary(int keyData)
        {
            if(_unitunioninfoDictionary.TryGetValue(keyData, out var ret))
            {
                return ret;
            }
            return null;
        }
    }
}
