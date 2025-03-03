using GameVals;
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
        public Action<Define.Errors> errorOccurred;
        public Action<string> debug;

        #region manager
        MonsterManager _monster;
        UnitManager _unit;
        SectionManager _section;
        IDataManager _data;

        public UnitManager unitManager { get { return this._unit; } }
        public MonsterManager monsterManager { get { return this._monster; } }
        public SectionManager sectionManager { get { return this._section; } }
        public IDataManager dataManager { get { return this._data; } }
        #endregion

        #region system
        private BaseRandomProbabiltty randomProbabiltty;

        public long RandomValue { get { return randomProbabiltty.GetRandomValue(); } }

        #endregion

        public StageLogic()
        {
            _monster = new MonsterManager(this);
            _unit = new UnitManager(this); 
            _section = new SectionManager(this);
        }

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

            _stageLevel = 0;
            _updateTick = currentTick;
            _stageStartTick = currentTick + (long)(Define.ReadyStageTime * Define.OneSecondTick);
            unitManager.Init();
            monsterManager.Init(_stageStartTick);
            sectionManager.Init();
        }

        public List<MonsterCheckData> Update(long currentTick ,bool isCheck = false)
        {
            List<MonsterCheckData> checkData = null;

            if (_updateTick == 0)
                Init(currentTick);

            while(_updateTick < currentTick)
            {
                if (_monster.ActiveMonsterCount >= Define.MonsterMaxCount)
                    return checkData;

                 _updateTick = CheckUpdateTick(currentTick);
                if (_updateTick >= _stageStartTick) StageStart();
                monsterManager.Update(_updateTick);
                unitManager.Update(_updateTick);
                sectionManager.Update(_updateTick);

                unitManager.LateUpdate(_updateTick);

                if(isCheck && _updateTick == currentTick)
                {
                    checkData = monsterManager.GetMonsterCheckDatas();
                }
            }

            return checkData;
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