using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class UnitManager
    {
        long _tick = 0;

        public Dictionary<int, UnitInfoData> _allUnit = new Dictionary<int, UnitInfoData>();
        public List<Unit> _activeUnit = new List<Unit>();

        public Dictionary<int, UnitInfoData> AllUnit { get { return _allUnit; } }
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

        public Unit SetUnitActive(UnitInfoData unitInfo, (int, int) section, long createTime)
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
            //stageLevel�� ���� �����ϴ� ���� ī�� ���� ���̰� ����

            //�������� ����ϴµ� �ϴ��� �׽�Ʈ ���̴ϱ� 2������ ���� 1 -> �� 2 -> �ӹ����� �����ؼ� �۾� ���� 
            //���߿� long���� 4�� ���� ��Ʈ���� �ؼ� �������� ���� ���̺� ���°� �������

            List<UnitInfoData> retList = new List<UnitInfoData>();

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
            UnitInfoData actionData = null;

            if (_allUnit.TryGetValue(UID, out var unitInfo))
            {
                if (unitInfo.GetCount() == 0)
                    actionData = unitInfo;

                unitInfo.AddCount();
            }
            else
            {
                actionData = new UnitInfoData(UID);
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

    }
}
