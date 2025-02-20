using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class UnitManager
    {
        public Action<Unit> ActiveUnitCreated;
        public Action<Unit> ActiveUnitRemoved;
        public Action<UnitData> UnitCardAdd;

        public Dictionary<int, UnitData> _allUnit = new Dictionary<int, UnitData>();
        public List<Unit> _activeUnit = new List<Unit>();

        public Dictionary<int, int> normalAppearedCount;
        public List<UnitInfoScript> normalUnitInfoList;

        public Dictionary<int, UnitData> AllUnit { get { return _allUnit; } }
        public List<Unit> ActiveUnit { get { return _activeUnit; } }

        private List<(long, UnitUnionInfo)> _unionDatas = new List<(long, UnitUnionInfo)>();

        private int _objectIndex = 0;

        private int _grade1Level = 0;
        private int _grade2Level = 0;
        private int _grade3Level = 0;

        public void Update(long currentTick)
        {
            foreach(var unit in _activeUnit)
            {
                unit.Update(currentTick);
            }

            UnionUnit(currentTick);
        }

        public void Init()
        {
            if (normalUnitInfoList == null)
                normalUnitInfoList = new List<UnitInfoScript>();
            else
                normalUnitInfoList.Clear();

            if (normalAppearedCount == null)
                normalAppearedCount = new Dictionary<int, int>();
            else
                normalAppearedCount.Clear();

            normalUnitInfoList = StageLogic.Instance.dataManager.GetUnitInfoScriptDictionaryAll().Where(_ => _.Value.unitGrade == 1).Select(_ => _.Value).ToList();

            foreach(var normalUnit in normalUnitInfoList)
            {
                normalAppearedCount.Add(normalUnit.unitUID, 0);
            }
        }

        public Unit SetUnitActive(UnitData unitInfo, (int, int) section, long createTime)
        {
            if (unitInfo.CheckUnitActive() && CheckUnitSet(section))
            {
                var ret = new Unit(_objectIndex++, unitInfo, section);
                ret.Init(createTime);
                _activeUnit.Add(ret);
                return ret;
            }

            return null;
        }

        public bool CheckUnitSet((int, int) sectionIndex)
        {
            int inSectionCount = 0;

            foreach (var activeUnit in _activeUnit)
            {
                if (activeUnit.SectionIndex == sectionIndex)
                {
                    inSectionCount++;
                }
            }

            return inSectionCount < Define.InSctionUnitCount;
        }

        public void GetUnitInfo(int stageLevel)
        {
            if (stageLevel == 0)
            {
                for(int i = 0; i< Define.UnitStartStageCreateCount; i++)
                {
                    AddUnitInfo();
                }
                AddUnitInfo(1);
                AddUnitInfo(1);
                AddUnitInfo(1);
                AddUnitInfo(1);
                AddUnitInfo(1);
                AddUnitInfo(1);
                AddUnitInfo(1);
            }
            else
            {
                for (int i = 0; i < Define.UnitStageCreateCount; i++)
                {
                    AddUnitInfo();
                }
            }

        }

        public void AddUnitInfo()
        {
            UnitData actionData = null;
            UnitInfoScript selectedNormalUnit = null;

            int index = (int)(StageLogic.Instance.RandomValue / (Define.MaxRandomValue / normalUnitInfoList.Count));

            for (int i = 0; i < normalUnitInfoList.Count; i++)
            {
                if(index == i)
                {
                    selectedNormalUnit = normalUnitInfoList[i];
                }
            }

            if(selectedNormalUnit == null)
            {
                int minKey = normalAppearedCount
                   .Aggregate((x, y) => x.Value <= y.Value ? x : y)
                   .Key;

                selectedNormalUnit = normalUnitInfoList.Find(_ => _.unitUID == minKey);
                normalAppearedCount[minKey]++;
            }
            else
            {
                normalAppearedCount[selectedNormalUnit.unitUID]++;
            }

            if (_allUnit.TryGetValue(selectedNormalUnit.unitUID, out var unitInfo))
            {
                actionData = unitInfo;
                unitInfo.AddCount();
            }
            else
            {
                actionData = new UnitData(selectedNormalUnit.unitUID);
                _allUnit.Add(selectedNormalUnit.unitUID, actionData);
            }

            if(actionData != null)
            {
                UnitCardAdd.Invoke(actionData);
            }
        }

        public void AddUnitInfo(int UID)
        {
            UnitData actionData = null;
            if (_allUnit.TryGetValue(UID, out var unitInfo))
            {
                if (unitInfo.GetCount() >= 0)
                    actionData = unitInfo;

                unitInfo.AddCount();
            }
            else
            {
                actionData = new UnitData(UID);
                _allUnit.Add(UID, actionData);
            }

            if (actionData != null)
            {
                UnitCardAdd.Invoke(actionData);
            }
        }

        public bool CheckUnitUnion(UnitUnionInfo unitInfo)
        {
            foreach (var material in unitInfo.GetMaterials())
            {
                if (CheckCanUsingUnit(material.Key, material.Value) == false)
                    return false;
            }

            return true;
        }

        public bool CheckCanUsingUnit(int materialUID, int materialCount)
        {
            if(_allUnit.TryGetValue(materialUID, out var data))
            {
                return data.CheckUsingUnit(materialCount);
            }

            return false;
        }

        public void UsingUnit(int uid, int count)
        {
            List<int> removedObjectID = new List<int>();

            if (_allUnit.TryGetValue(uid, out var unit))
            {
                int usingActiveUnitCount = unit.UsingUnit(count);

                for(int i = 0; i < usingActiveUnitCount; i++)
                {
                    var activeUnit = _activeUnit.Find(_ => _.GetUID() == uid && removedObjectID.Find(_1 => _1 == _.GetObjectIndex()) == 0);
                    //var activeUnit = _activeUnit.Find(_ => _.GetUID() == uid);

                    RemoveActiveUnit(activeUnit);
                    removedObjectID.Add(activeUnit.GetObjectIndex());
                }
            }

        }

        public void UpgradeGrade(int grade)
        {
            switch(grade)
            {
                case 1:
                    _grade1Level++;
                    break;
                case 2:
                    _grade2Level++;
                    break;
                case 3:
                    _grade3Level++;
                    break;
            }
        }

        public void RemoveActiveUnit(Logic.Unit removeUnit)
        {
            ActiveUnitRemoved.Invoke(removeUnit);
            removeUnit.Clear();
            _activeUnit.Remove(removeUnit);
        }

        public void UnionUnit(long updateTick)
        {
            var deleteUnionDataList = _unionDatas.FindAll(_ => _.Item1 < updateTick);
            foreach (var uniondata in deleteUnionDataList)
            {
                UnitUnion(uniondata.Item2);
                _unionDatas.Remove(uniondata);
            }
        }

        public void UnitUnion(UnitUnionInfo unitInfoData)
        {
            if (unitInfoData != null)
            {
                foreach (var material in unitInfoData.GetMaterials())
                {
                    UsingUnit(material.Key, material.Value);
                }

                AddUnitInfo(unitInfoData.GetCreatedUID());
            }
        }

        public void AddUnionData(long addTick, UnitUnionInfo unitdata)
        {
            _unionDatas.Add((addTick, unitdata));
        }

        public bool SetUnit(UnitData unitInfo, (int, int) section, long createTime)
        {
            var activeUnit = SetUnitActive(unitInfo, section, createTime);
            if (activeUnit == null)
                return false;

            ActiveUnitCreated.Invoke(activeUnit);
            return true;
        }

        public void MoveUnit(Unit unit, (int, int) section)
        {
            unit.MovePosition(section);
        }


    }
}
