using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class StageLogic
    {
        #region variable
        public int _stageLevel;
        long _stageStartTick;
        long _addMonsterTick;
        private int _stageMonsterCount;
        private int _activeMonsterCount;

        public int StageMonsterCount { get { return _stageMonsterCount; } }
        public int ActiveMonsterCount { get { return _activeMonsterCount; } }

        public int StageLevel { get { return _stageLevel; } }
        public long StageStartTick { get { return _stageStartTick; } }

        long _updateTick = 0;

        #endregion

        #region singlton
        private static StageLogic _instance;

        public static StageLogic Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StageLogic();
                }
                return _instance;
            }
        }

        #endregion

        #region manager
        MonsterManager _monster = new MonsterManager();
        UnitManager _unit = new UnitManager();
        IDataManager _data;

        public static UnitManager Unit { get { return _instance._unit; } }
        public static MonsterManager Monster { get { return _instance._monster; } }

        public static IDataManager Data { get { return _instance._data; } }
        #endregion

        #region action
        public Action<Monster> MonsterCreated;
        public Action<Unit> ActiveUnitCreated;
        public Action<Unit> ActiveUnitRemoved;
        public Action<UnitInfoData> UnitCardAdd;
        #endregion

        #region system
        private Dictionary<(int, int), Section> _sectionDatas = new Dictionary<(int, int), Section>();

        private List<(long, UnitInfoData, int)> _unionDatas = new List<(long, UnitInfoData, int)>();

        private BaseRandomProbabiltty randomProbabiltty;

        public long RandomValue { get { return randomProbabiltty.GetRandomValue(); } }

        public Dictionary<(int, int), Section> SectionDatas 
        { 
            get 
            { 
                if(_sectionDatas.Count == 0)
                {
                    SetSectionData();
                }

                return _instance._sectionDatas; 
            } 
        }
        #endregion

        public void Init(long currentTick)
        {

            System.Random random = new System.Random();
            var initValue = new List<long>();

            for (int i = 0; i < 3; i++)
            {
                byte[] buffer = new byte[8];
                random.NextBytes(buffer);
                long seedValue = BitConverter.ToInt64(buffer, 0);
                initValue.Add(seedValue);
            }

            if (randomProbabiltty == null)
            {
                randomProbabiltty = new BaseRandomProbabiltty(initValue);
            }
            else
            {
                randomProbabiltty.Init(initValue);
            }

            _updateTick = currentTick;
            Unit.Init(_updateTick);
            Monster.Init(_updateTick);

            _stageStartTick = _updateTick + (long)(Define.ReadyStageTime * Define.OneSecondTick);
            _addMonsterTick = _stageStartTick;

            SetSectionData();
        }

        public void Update(long currentTick)
        {
            if (_updateTick == 0)
                Init(currentTick);

            while(_updateTick < currentTick)
            {
                if (_activeMonsterCount >= Define.MonsterMaxCount)
                    return;

                 _updateTick = CheckUpdateTick(currentTick);
                StageMonsterCreate();
                Monster.Update(_updateTick);
                Unit.Update(_updateTick);
                SectionUpdate(_updateTick); 
                
                UnionUnit();
            }
        }

        private long CheckUpdateTick(long currentTick)
        {
            long updateTick = _stageStartTick < currentTick ? _stageStartTick : currentTick;
            updateTick = (_addMonsterTick < updateTick && _stageMonsterCount < Define.MonsterStageCreateCount) ? _stageStartTick : updateTick;
            long checkMonsterUpdateTick = _monster.GetUpdateTick(currentTick);
            updateTick = checkMonsterUpdateTick < updateTick ? checkMonsterUpdateTick : updateTick;
            updateTick = CheckSkillActiveTick(updateTick);
            return updateTick;
        }

        public void StageMonsterCreate()
        {
            if (_updateTick >= _stageStartTick)
            {
                StageStart(_updateTick);
            }

            if (_stageMonsterCount >= Define.MonsterStageCreateCount)
            {
                return;
            }

            if (_addMonsterTick <= _updateTick)
            {
                var monster = Monster.AddMonster(_addMonsterTick);
                _addMonsterTick += (long)(Define.MonsterCreateTime * Define.OneSecondTick);
                _stageMonsterCount++;
                _activeMonsterCount++;
                MonsterCreated.Invoke(monster);
            }
        }

        public static void Clear()
        {
            //Clear
        }

        private void StageStart(long updateTick)
        {
            _unit.GetUnitInfo(_stageLevel);
            _stageMonsterCount = 0;
            _addMonsterTick = _stageStartTick;
            _stageStartTick += (long)(Define.DuringStageTime * Define.OneSecondTick);
            _stageLevel++;
        }

        public void SetSectionData()
        {
            if (_sectionDatas.Count == 0)
            {
                for (int i = 0; i < Define.SectionCount; i++)
                {
                    for (int j = 0; j < Define.SectionCount; j++)
                    {
                        if (i == 0 || i == Define.SectionCount - 1 || j == 0 || j == Define.SectionCount - 1)
                        {
                            _sectionDatas.Add((i, j), new Section((i, j)));
                        }
                    }
                }
            }
        }

        public bool SetUnit(UnitInfoData unitInfo, (int, int) section, long createTime)
        {
            var activeUnit = _unit.SetUnitActive(unitInfo, section, createTime);
            if (activeUnit == null)
                return false;

            ActiveUnitCreated.Invoke(activeUnit);
            return true;
        }

        public void MoveUnit(Unit unit, (int, int) section)
        {
            unit.MovePosition(section);
        }

        public bool CheckUnitAttack()
        {
            if(_activeMonsterCount >= 0 || _stageMonsterCount <= Define.MonsterStageCreateCount)
            {
                return true;
            }

            return false;
        }

        public bool CheckUnitUnion(UnitUnionInfo unionInfoList)
        {
            foreach(var material in unionInfoList.GetMaterials())
            {
                if (_unit.CheckCanUsingUnit(material.Item1, material.Item2) == false)
                    return false;
            }

            return true;
        }

        public void UnionUnit()
        {
            var deleteUnionDataList = _unionDatas.FindAll(_ => _.Item1 < _updateTick);
            foreach (var uniondata in deleteUnionDataList)
            {
                UnitUnion(uniondata.Item2, uniondata.Item3);
                _unionDatas.Remove(uniondata);
            }
        }

        public void UnitUnion(UnitInfoData unitInfoData, int index)
        {
            var unionData = unitInfoData.GetUnitUnionData(index);

            if(unionData != null)
            {
                foreach(var material in unionData.GetMaterials())
                {
                    _unit.UsingUnit(material.Item1, material.Item2);
                }

                _unit.AddUnitInfo(unionData.GetCreatedUID());
            }
        }

        public void AddUnionData(long addTick, UnitInfoData unitdata, int index)
        {
            _unionDatas.Add((addTick, unitdata, index));
        }

        public void UpgradeGrade(int upgradeGrade)
        {
            _unit.UpgradeGrade(upgradeGrade);
        }

        public long GetCurrentTick()
        {
            return _updateTick;
        }

        public void SectionUpdate(long updateTick)
        {
            foreach(var section in _sectionDatas)
            {
                section.Value.Update(updateTick);
            }
        }

        public long CheckSkillActiveTick(long checkTick)
        {
            long ret = checkTick;

            foreach(var section in _sectionDatas)
            {
                long sectionCheckTick = section.Value.CheckActiveSkillTick();
                ret = ((sectionCheckTick == 0) || (ret < sectionCheckTick)) ? ret : sectionCheckTick;
            }

            return ret;
        }

        public void RemoveActiveUnit(Unit activeUnit)
        {
            Unit.RemoveActiveUnit(activeUnit);
        }

        public void SetDataManager(IDataManager dataManager)
        {
            _data = dataManager;
        }
    }
}