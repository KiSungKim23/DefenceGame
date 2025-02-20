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

        public Action<Define.Errors> errorOccurred;
        public Action<string> debug;

        #region manager
        MonsterManager _monster = new MonsterManager();
        UnitManager _unit = new UnitManager();
        SectionManager _section = new SectionManager();
        IDataManager _data;

        public UnitManager unitManager { get { return Instance._unit; } }
        public MonsterManager monsterManager { get { return Instance._monster; } }
        public SectionManager sectionManager { get { return Instance._section; } }
        public IDataManager dataManager { get { return Instance._data; } }
        #endregion

        #region system
        private BaseRandomProbabiltty randomProbabiltty;

        public long RandomValue { get { return randomProbabiltty.GetRandomValue(); } }

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
            _stageStartTick = currentTick + (long)(Define.ReadyStageTime * Define.OneSecondTick);
            unitManager.Init();
            monsterManager.Init(_stageStartTick);
            sectionManager.Init();
        }

        public void Update(long currentTick)
        {
            if (_updateTick == 0)
                Init(currentTick);

            while(_updateTick < currentTick)
            {
                if (_monster.ActiveMonsterCount >= Define.MonsterMaxCount)
                    return;

                 _updateTick = CheckUpdateTick(currentTick);
                //debug.Invoke(monsterManager.StageMonsterCount.ToString());
                if (_updateTick >= _stageStartTick) StageStart();
                monsterManager.Update(_updateTick);
                unitManager.Update(_updateTick); 
                sectionManager.Update(_updateTick);
            }
        }

        private long CheckUpdateTick(long currentTick)
        {
            long updateTick = _stageStartTick < currentTick ? _stageStartTick : currentTick;
            updateTick = monsterManager.GetUpdateTick(currentTick, updateTick);
            updateTick = sectionManager.CheckSkillActiveTick(updateTick);
            return updateTick;
        }

        private void StageStart()
        {
            _unit.GetUnitInfo(_stageLevel);
            _monster.SetStageStart(_stageStartTick);
            _stageStartTick += (long)(Define.DuringStageTime * Define.OneSecondTick);
            _stageLevel++;
        }

        public long GetCurrentTick()
        {
            return _updateTick;
        }

        public void SetDataManager(IDataManager dataManager)
        {
            _data = dataManager;
        }

        public static void Clear()
        {
            //Clear
        }
    }
}