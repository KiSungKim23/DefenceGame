using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class UnitUnionInfo
    {
        Dictionary<int, int> materials = new Dictionary<int, int>();
        int createdUID;

        public UnitUnionInfo(UnitUnionInfoScript scriptData)
        {
            createdUID = scriptData.unitUID;

            materials.Add(scriptData.mainMaterialUID, 1);

            if(scriptData.materail2UID != 0)
            {
                if (materials.ContainsKey(scriptData.materail2UID)) materials[scriptData.materail2UID]++;
                else materials.Add(scriptData.materail2UID, 1);
            }
            if (scriptData.materail3UID != 0)
            {
                if (materials.ContainsKey(scriptData.materail3UID)) materials[scriptData.materail3UID]++;
                else materials.Add(scriptData.materail3UID, 1);
            }
            if (scriptData.materail4UID != 0)
            {
                if (materials.ContainsKey(scriptData.materail4UID)) materials[scriptData.materail4UID]++;
                else materials.Add(scriptData.materail4UID, 1);
            }
            if (scriptData.materail5UID != 0)
            {
                if (materials.ContainsKey(scriptData.materail5UID)) materials[scriptData.materail5UID]++;
                else materials.Add(scriptData.materail5UID, 1);
            }
            if (scriptData.materail6UID != 0)
            {
                if (materials.ContainsKey(scriptData.materail6UID)) materials[scriptData.materail6UID]++;
                else materials.Add(scriptData.materail6UID, 1);
            }

        }

        public Dictionary<int, int> GetMaterials()
        {
            return materials;
        }

        public int GetCreatedUID()
        {
            return createdUID;
        }
    }

    public class UnitData
    {
        private int _uid;

        private int _count;

        private int _activeCount;

        private Dictionary<int, UnitUnionInfo> _unitUnionInfo = new Dictionary<int, UnitUnionInfo>();

        public UnitData(int uid)
        {
            _uid = uid;
            _count = 1;
            _activeCount = 0;

            var unionScriptList = StageLogic.Instance.dataManager.GetUnitUnionInfoScriptListAll().FindAll(_=>_.mainMaterialUID == _uid);

            foreach(var unionScript in unionScriptList)
            {
                _unitUnionInfo.Add(unionScript.unitUID, new UnitUnionInfo(unionScript));
            }
        }

        public void AddCount()
        {
            _count++;
        }

        public bool CheckUnitActive()
        {
            if (_count - _activeCount > 0)
            {
                _activeCount++;
                return true;
            }
            return false;
        }

        public bool CheckUsingUnit(int count)
        {
            return _count >= count;
        }

        public int UsingUnit(int count)
        {
            int inactiveUnits = _count - _activeCount;

            if (inactiveUnits >= count)
            {
                _count -= count;
                return 0;
            }
            else
            {
                _count -= count;
                int usedActiveUnits = count - inactiveUnits;
                _activeCount -= usedActiveUnits;
                return usedActiveUnits;
            }
        }

        public int GetCount()
        {
            return _count;
        }

        public int GetActiveCount()
        {
            return _activeCount;
        }

        public int GetUID()
        {
            return _uid;
        }

        public bool CheckUnionButtonActive(int createUnitUnionData)
        {
            foreach (var unitUnionInfo in _unitUnionInfo)
            {
                switch (unitUnionInfo.Key)
                {
                    case 1:
                        return true;
                    case 2:
                        return true;
                    case 3:
                        return true;
                }
            }
            return false;
        }

        public bool CheckCanUnion(int unionUID)
        {
            if (_unitUnionInfo.TryGetValue(unionUID, out var ret))
            {
                return StageLogic.Instance.unitManager.CheckUnitUnion(ret); ;
            }
            return false;

        }

        public UnitUnionInfo GetUnitUnionData(int uid)
        {
            if(_unitUnionInfo.TryGetValue(uid, out var ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }
        public Dictionary<int, UnitUnionInfo> GetUnitUnionDatas()
        {
            return _unitUnionInfo;
        }

        public void DeActive()
        {
            _activeCount--;
        }

        public bool CheckCanActive()
        {
            return _count - _activeCount > 0;
        }
    }
}
