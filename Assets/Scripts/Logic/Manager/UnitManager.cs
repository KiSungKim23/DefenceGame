using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class UnitManager
    {
        long _tick = 0;

        public Dictionary<int, UnitData> _allUnit = new Dictionary<int, UnitData>();
        public List<Unit> _activeUnit = new List<Unit>();

        public Dictionary<int, UnitData> AllUnit { get { return _allUnit; } }
        public List<Unit> ActiveUnit { get { return _activeUnit; } }

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
            //stageLevel에 따라 지급하는 유닛 카드 갯수 차이가 있음

            //랜덤으로 줘야하는데 일단은 테스트 용이니까 2종류만 주자 1 -> 딜 2 -> 속박으로 세팅해서 작업 ㄱㄱ 
            //나중에 long으로 4개 놓고 비트연산 해서 고정으로 랜덤 테이블 갖는거 만들거임

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
            StageLogic.Instance.ActiveUnitRemoved.Invoke(removeUnit);
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

    }
}
