using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class UnitUnionInfo
    {
        List<(int, int)> materials = new List<(int, int)>();

        int createdUID;

        public UnitUnionInfo(int index)
        {
            createdUID = index + 2;

            switch(createdUID)
            {
                case 3:
                    materials.Add((1, 1));
                    materials.Add((2, 1));
                    break;
                case 4:
                    materials.Add((1, 2));
                    break;
                case 5:
                    materials.Add((2, 2));
                    break;
            }
        }

        public List<(int, int)> GetMaterials()
        {
            return materials;
        }

        public int GetCreatedUID()
        {
            return createdUID;
        }
    }

    public class UnitInfoData
    {
        private int _uid;

        private int _count;

        private int _activeCount;

        private Dictionary<int, UnitUnionInfo> _unitUnionInfo = new Dictionary<int, UnitUnionInfo>();

        public UnitInfoData(int uid)
        {
            _uid = uid;
            _count = 1;
            _activeCount = 0;

            //_unitUnionInfo = Managers.Data.GetUnionList(uid);

            _unitUnionInfo.Add(1, new UnitUnionInfo(1));
            _unitUnionInfo.Add(2, new UnitUnionInfo(2));
            _unitUnionInfo.Add(3, new UnitUnionInfo(3));

            //////////////////////////////위에거는 자동 세팅 되도록 할거


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

        public bool CheckUnionButtonActive(int index)
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

        public bool CheckCanUnion(int index)
        {
            if (_unitUnionInfo.TryGetValue(index, out var ret))
            {
                return StageLogic.Instance.CheckUnitUnion(ret); ;
            }
            return false;

        }

        public UnitUnionInfo GetUnitUnionData(int index)
        {
            if(_unitUnionInfo.TryGetValue(index, out var ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }
    }
}
