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

        public int ActiveMonsterCount { get { return _activeMonsterCount; } }

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
                        var removeSection = StageLogic.Instance.sectionManager.GetSectionData(sectionData.Item1);
                        if (removeSection != null)
                        {
                            removeSection.RemoveSectionMonsterData(monster.Value);
                        }
                        else StageLogic.Instance.errorOccurred.Invoke(Define.Errors.E_LogicError);

                        var moveSection = StageLogic.Instance.sectionManager.GetSectionData(sectionData.Item2);
                        if (moveSection != null)
                        {
                            moveSection.AddSectionMonsterData(monster.Value);
                        }
                        else StageLogic.Instance.errorOccurred.Invoke(Define.Errors.E_LogicError);

                        monster.Value.SetSectionIndex(sectionData.Item2);
                    }
                }
            }
        }

        public void MonsterDead(List<Monster> monsters)
        {
            bool isDeadMoster = (monsters.Count != 0);

            foreach(var monster in monsters)
            {
                var sectionData = StageLogic.Instance.sectionManager.GetSectionData(monster.GetSectionIndex());

                if (sectionData != null)
                {
                    sectionData.RemoveSectionMonsterData(monster);
                }
                else StageLogic.Instance.errorOccurred(Define.Errors.E_LogicError);
            }

            if(monsters.Count != 0)
            {
                StageLogic.Instance.monsterManager.SetActiveCount();
            }
        }

        public Monster AddMonster(long CreateTick)
        {
            var createMonster = _monsters.FirstOrDefault(monster => monster.Value.State == Define.MonsterState.dead || monster.Value.State == Define.MonsterState.wait);
            createMonster.Value.SetMonster(CreateTick, StageLogic.Instance.StageLevel);
            var addMonsterSection = StageLogic.Instance.sectionManager.GetSectionData(createMonster.Value.NowSectionIndex);
            if (addMonsterSection != null)
            {
                addMonsterSection.AddSectionMonsterData(createMonster.Value);
            }
            else
            {
                StageLogic.Instance.errorOccurred.Invoke(Define.Errors.E_LogicError);
            }
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
                MonsterCreated.Invoke(monster);
                SetActiveCount();
            }
        }

        public bool CheckUnitAttack()
        {
            if (_activeMonsterCount > 0 || _stageMonsterCount < Define.MonsterStageCreateCount)
            {
                return true;
            }

            return false;
        }
        
        public void SetActiveCount()
        {
            _activeMonsterCount = _monsters.Where(_ => _.Value.State == Define.MonsterState.active).ToList().Count;
        }
    }
}
