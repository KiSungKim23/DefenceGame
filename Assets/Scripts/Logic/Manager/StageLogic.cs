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

        public UnitManager unitManager { get { return _instance._unit; } }
        public MonsterManager monsterManager { get { return _instance._monster; } }
        public IDataManager dataManager { get { return _instance._data; } }
        #endregion

        #region action
        public Action<UnitData> UnitCardAdd;
        #endregion

        #region system
        private Dictionary<(int, int), Section> _sectionDatas = new Dictionary<(int, int), Section>();

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
            unitManager.Init(_updateTick);
            _stageStartTick = _updateTick + (long)(Define.ReadyStageTime * Define.OneSecondTick);
            monsterManager.Init(_stageStartTick);

            SetSectionData();
        }

        public void Update(long currentTick)
        {
            if (_updateTick == 0)
                Init(currentTick);

            while(_updateTick < currentTick)
            {
                if (_monster.StageMonsterCount >= Define.MonsterMaxCount)
                    return;

                 _updateTick = CheckUpdateTick(currentTick);
                if (_updateTick >= _stageStartTick) StageStart();
                monsterManager.Update(_updateTick);
                unitManager.Update(_updateTick);
                SectionUpdate(_updateTick); 
            }
        }

        private long CheckUpdateTick(long currentTick)
        {
            long updateTick = _stageStartTick < currentTick ? _stageStartTick : currentTick;
            updateTick = monsterManager.GetUpdateTick(currentTick, updateTick);
            updateTick = CheckSkillActiveTick(updateTick);
            return updateTick;
        }
        public static void Clear()
        {
            //Clear
        }

        private void StageStart()
        {
            _unit.GetUnitInfo(_stageLevel);
            _monster.SetStageStart(_stageStartTick);
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

        public void SetDataManager(IDataManager dataManager)
        {
            _data = dataManager;
        }
    }
}