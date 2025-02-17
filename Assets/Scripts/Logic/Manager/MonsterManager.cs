using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class MonsterManager
    {

        long _addMonsterTick;
        private int _stageMonsterCount;
        private int _activeMonsterCount;

        public int StageMonsterCount { get { return _stageMonsterCount; } }

        public Action<Monster> MonsterCreated;

        private Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();

        public void Init(long stageStartTick)
        {
            _addMonsterTick = stageStartTick;
            SetMonsterInit();
        }

        public void Update(long updateTick)
        {
            MonsterCreate(updateTick);

            foreach (var monster in _monsters)
            {
                if (monster.Value.State == Define.MonsterState.active)
                {
                    monster.Value.Update(updateTick);

                    ((int, int), (int, int)) sectionData = monster.Value.GetSectionData();
                    if (sectionData.Item1 != sectionData.Item2)
                    {
                        StageLogic.Instance.SectionDatas[sectionData.Item1].RemoveSectionMonsterData(monster.Value);
                        StageLogic.Instance.SectionDatas[sectionData.Item2].AddSectionMonsterData(monster.Value);
                        monster.Value.SetSectionIndex(sectionData.Item2);
                    }
                }
            }
        }

        public Monster AddMonster(long CreateTick)
        {
            var createMonster = _monsters.FirstOrDefault(monster => monster.Value.State == Define.MonsterState.dead || monster.Value.State == Define.MonsterState.wait);
            createMonster.Value.SetMonster(CreateTick, StageLogic.Instance.StageLevel);
            StageLogic.Instance.SectionDatas[createMonster.Value.NowSectionIndex].AddSectionMonsterData(createMonster.Value);
            return createMonster.Value;
        }

        private void SetMonsterInit()
        {
            if (_monsters.Count == 0)
            {
                for (int i = 0; i < 150; i++)
                {
                    var monster = new Monster(i);
                    _monsters.Add(i, monster);
                }
            }
        }

        public long GetUpdateTick(long currentTick, long updateTick)
        {
            if (_addMonsterTick < updateTick && _stageMonsterCount < Define.MonsterStageCreateCount)
            {
                updateTick = _addMonsterTick;
            }

            foreach (var monster in _monsters)
            {
                if (monster.Value.State == Define.MonsterState.active)
                {
                    long checkTick = monster.Value.CheckUpdateTick(currentTick);
                    if (checkTick < updateTick)
                        updateTick = checkTick;
                }
            }
            return updateTick;
        }

        public void SetStageStart(long stageStartTick)
        {
            _stageMonsterCount = 0;
            _addMonsterTick = stageStartTick;
        }

        public void MonsterCreate(long updateTick)
        {
            if (_stageMonsterCount >= Define.MonsterStageCreateCount)
            {
                return;
            }

            if (_addMonsterTick <= updateTick)
            {
                var monster = AddMonster(_addMonsterTick);
                _addMonsterTick += (long)(Define.MonsterCreateTime * Define.OneSecondTick);
                _stageMonsterCount++;
                _activeMonsterCount++;
                MonsterCreated.Invoke(monster);
            }
        }

        public bool CheckUnitAttack()
        {
            if (_activeMonsterCount >= 0 || _stageMonsterCount <= Define.MonsterStageCreateCount)
            {
                return true;
            }

            return false;
        }

    }
}
