using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class UnitManager
    {
        long _tick = 0;

        public Action<Unit> ActiveUnitCreated;
        public Action<Unit> ActiveUnitRemoved;

        public Dictionary<int, UnitData> _allUnit = new Dictionary<int, UnitData>();
        public List<Unit> _activeUnit = new List<Unit>();

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

        public void Init(long Tick)
        {
            _tick = Tick;
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
            List<UnitData> retList = new List<UnitData>();

            if (stageLevel == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    AddUnitInfo(1);
                }
                for (int i = 0; i < 3; i++)
                {
                    AddUnitInfo(2);
                }
            }
            else
            {
                AddUnitInfo(1);
                AddUnitInfo(2);
            }

        }

        public void AddUnitInfo(int UID)
        {
            UnitData actionData = null;

            if (_allUnit.TryGetValue(UID, out var unitInfo))
            {
                if (unitInfo.GetCount() == 0)
                    actionData = unitInfo;

                unitInfo.AddCount();
            }
            else
            {
                actionData = new UnitData(UID);
                _allUnit.Add(UID, actionData);
            }

            if(actionData != null)
            {
                StageLogic.Instance.UnitCardAdd.Invoke(actionData);
            }

        }

        public bool CheckUnitUnion(UnitUnionInfo unionInfoList)
        {
            foreach (var material in unionInfoList.GetMaterials())
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
            if (_allUnit.TryGetValue(uid, out var unit))
            {
                int usingActiveUnitCount = unit.UsingUnit(count);

                for(int i = 0; i < usingActiveUnitCount; i++)
                {
                    var activeUnit = _activeUnit.Find(_ => _.GetUID() == uid);
                    RemoveActiveUnit(activeUnit);
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

        public bool ChckCanUnion(UnitUnionInfoScript unionInfo)
        {
            Dictionary<int, int> metarialInfo = new Dictionary<int, int>();

            metarialInfo.Add(unionInfo.mainMaterialUID, 1);
            if (unionInfo.materail2UID != 0)
            {
                if (metarialInfo.TryGetValue(unionInfo.materail2UID, out var count)) metarialInfo[unionInfo.materail2UID] = count + 1;
                else metarialInfo.Add(unionInfo.materail2UID, 1);
            }
            if (unionInfo.materail3UID != 0)
            {
                if (metarialInfo.TryGetValue(unionInfo.materail3UID, out var count)) metarialInfo[unionInfo.materail3UID] = count + 1;
                else metarialInfo.Add(unionInfo.materail3UID, 1);
            }
            if (unionInfo.materail4UID != 0)
            {
                if (metarialInfo.TryGetValue(unionInfo.materail4UID, out var count)) metarialInfo[unionInfo.materail4UID] = count + 1;
                else metarialInfo.Add(unionInfo.materail4UID, 1);
            }
            if (unionInfo.materail5UID != 0)
            {
                if (metarialInfo.TryGetValue(unionInfo.materail5UID, out var count)) metarialInfo[unionInfo.materail5UID] = count + 1;
                else metarialInfo.Add(unionInfo.materail5UID, 1);
            }
            if (unionInfo.materail6UID != 0)
            {
                if (metarialInfo.TryGetValue(unionInfo.materail6UID, out var count)) metarialInfo[unionInfo.materail6UID] = count + 1;
                else metarialInfo.Add(unionInfo.materail6UID, 1);
            }

            return false;
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
