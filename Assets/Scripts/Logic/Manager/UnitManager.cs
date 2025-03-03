using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class UnitManager
    {
        private StageLogic _stageLogic;

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
        private List<(long, Unit)> _deactiveUnitData = new List<(long, Unit)>();
        private List<(long, (int, int), int)> _moveUnitData = new List<(long, (int, int), int)>();

        private int _objectIndex = 0;

        private int _grade1Level = 0;
        private int _grade2Level = 0;
        private int _grade3Level = 0;

        public UnitManager(StageLogic stageLogic)
        {
            _stageLogic = stageLogic;
        }

        public void Update(long currentTick)
        {
            foreach(var unit in _activeUnit)
            {
                unit.Update(currentTick);
            }
        }

        public void LateUpdate(long currentTick)
        {
            MoveUnits(currentTick);
            UnionUnits(currentTick);
            DeactiveUnits(currentTick);
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

            normalUnitInfoList = _stageLogic.dataManager.GetUnitInfoScriptDictionaryAll().Where(_ => _.Value.unitGrade == 1).Select(_ => _.Value).ToList();

            foreach (var normalUnit in normalUnitInfoList)
            {
                normalAppearedCount.Add(normalUnit.unitUID, 0);
            }

            _objectIndex = 0;
            _grade1Level = 0;
            _grade2Level = 0;
            _grade3Level = 0;
        }

        public Unit SetUnitActive(UnitData unitInfo, (int, int) section, long createTime)
        {
            if (unitInfo.CheckUnitActive() && CheckUnitSet(section))
            {
                var ret = new Unit(_stageLogic, _objectIndex++, unitInfo, section);
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

            int index = (int)(_stageLogic.RandomValue / (Define.MaxRandomValue / normalUnitInfoList.Count));

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
                actionData = new UnitData(_stageLogic, selectedNormalUnit.unitUID);
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
                actionData = new UnitData(_stageLogic, UID);
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

        public void UsingUnit(int uid, int count, long currentTick)
        {
            if (_allUnit.TryGetValue(uid, out var unit))
            {
                int usingActiveUnitCount = unit.UsingUnit(count);

                for(int i = 0; i < usingActiveUnitCount; i++)
                {
                    var activeUnit = _activeUnit.Find(_ => _.GetUID() == uid && _deactiveUnitData.Find(_1 => _1.Item2.GetObjectIndex() == _.GetObjectIndex()).Item1 == 0);
                    _deactiveUnitData.Add((currentTick, activeUnit));
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
        public void DeactiveUnit(int deactiveUnitObjectIndex)
        {
            var removeUnit = _activeUnit.Find(_ => _.GetObjectIndex() == deactiveUnitObjectIndex);

            ActiveUnitRemoved.Invoke(removeUnit);
            removeUnit.Clear();
            _activeUnit.Remove(removeUnit);
        }
        public void UnionUnits(long updateTick)
        {
            var deleteUnionDataList = _unionDatas.FindAll(_ => _.Item1 <= updateTick);
            foreach (var uniondata in deleteUnionDataList)
            {
                UnitUnion(uniondata.Item2, updateTick);
                _unionDatas.Remove(uniondata);
            }
        }

        public void MoveUnits(long updateTick)
        {
            var moveDataList = _moveUnitData.FindAll(_ => _.Item1 <= updateTick);
            foreach (var moveData in moveDataList)
            {
                var unit = _activeUnit.Find(_ => _.GetObjectIndex() == moveData.Item3);
                if (unit != null)
                {
                    unit.MovePosition(moveData.Item2);
                }
                _moveUnitData.Remove(moveData);
            }
        }

        public void DeactiveUnits(long updateTick)
        {
            var deactiveUnitList = _deactiveUnitData.FindAll(_ => _.Item1 <= updateTick);

            foreach (var deActiveUnit in deactiveUnitList)
            {
                DeactiveUnit(deActiveUnit.Item2.GetObjectIndex());
                _deactiveUnitData.Remove(deActiveUnit);
            }

        }

        public void UnitUnion(UnitUnionInfo unitInfoData, long currnetTick)
        {
            if (unitInfoData != null)
            {
                foreach (var material in unitInfoData.GetMaterials())
                {
                    UsingUnit(material.Key, material.Value, currnetTick);
                }

                AddUnitInfo(unitInfoData.GetCreatedUID());
            }
        }

        public Define.Errors AddUnionData(int unitUID, int targetUnitUID, long addTick)
        {
            if (_allUnit.TryGetValue(unitUID, out var unitData) == false)
                return Define.Errors.E_AddUnionData_NoneUnitData;

            var unionData = unitData.GetUnitUnionData(targetUnitUID);

            if (unionData == null)
                return Define.Errors.E_AddUnionData_NoneUnionData;

            _unionDatas.Add((addTick, unionData));

            return Define.Errors.S_OK;
        }

        public Define.Errors SetUnit(int unitUID, (int, int) section, long createTime)
        {
            if (_allUnit.TryGetValue(unitUID, out var unitData) == false)
                return Define.Errors.E_SetUnit_NontUnitData;

            var activeUnit = SetUnitActive(unitData, section, createTime);
            if (activeUnit == null)
                return Define.Errors.E_SetUnit_NontActiveUnit;

            ActiveUnitCreated.Invoke(activeUnit);
            return Define.Errors.S_OK;
        }

        public Define.Errors AddMoveUnit(int unitObjectIndex, (int, int) section, long setTick)
        {
            if (_moveUnitData.Find(_ => _.Item3 == unitObjectIndex).Item1 != 0)
                return Define.Errors.E_MoveUnit_WaitMove;

            _moveUnitData.Add((setTick, section, unitObjectIndex));
            return Define.Errors.S_OK;
        }
        public Define.Errors AddDeactiveUnit(int objectIndex, long currentTick)
        {
            if (_deactiveUnitData.Find(_ => _.Item2.GetObjectIndex() == objectIndex).Item1 != 0)
                return Define.Errors.E_AddDeactiveUnit_AlreadyDeactiveUnitData;

            var activeUnit = _activeUnit.Find(_ => _.GetObjectIndex() == objectIndex);
            if (activeUnit == null)
                return Define.Errors.E_AddDeactiveUnit_NoneActive;

            _deactiveUnitData.Add((currentTick, activeUnit));
            return Define.Errors.S_OK;
        }
        public Define.Errors SetTarget(int activeUnitObjectIndex, (int, int) sectionIndex, long setTick)
        {
            var activeUnit = _activeUnit.Find(_ => _.GetObjectIndex() == activeUnitObjectIndex);
            if (activeUnit == null)
                return Define.Errors.E_SetTarget_NoneActiveUnit;
            var setTargetCheck = activeUnit.SetTarget(sectionIndex, setTick);
            if (setTargetCheck)
                return Define.Errors.S_OK;
            else
                return Define.Errors.E_SetTarget_NoneSectionData;
        }

    }
}
