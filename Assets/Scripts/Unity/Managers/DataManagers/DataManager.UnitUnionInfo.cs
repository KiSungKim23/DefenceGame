using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Client
{

    public partial class DataManager : Logic.IDataManager
    {
        private List< Logic.UnitUnionInfoScript> _unitunioninfoList = new List< Logic.UnitUnionInfoScript>();

        public void LoadUnitUnionInfoScript()
        {
            string filePath = Path.Combine(@"C:\DefenceGame\DefenceGame\Assets\Resources\Scripts\", "UnitUnionInfo.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _unitunioninfoList = JsonConvert.DeserializeObject<List<Logic.UnitUnionInfoScript>>(json);
            }
            else
            {
                Debug.LogError("UnitUnionInfoScript파일을 찾을 수 없습니다");
            }
        }

        public List<Logic.UnitUnionInfoScript> GetUnitUnionInfoScriptListAll()
        {
            return _unitunioninfoList;
        }
    }
}
