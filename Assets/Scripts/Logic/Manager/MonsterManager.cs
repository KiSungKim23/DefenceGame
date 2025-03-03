using GameVals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class MonsterManager
    {
        private StageLogic _stageLogic;

        long _addMonsterTick;
        private int _stageMonsterCount;
        private int _activeMonsterCount;

        public int ActiveMonsterCount { get { return _activeMonsterCount; } }

        public Action<Monster> MonsterCreated;

        private Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();

        public MonsterManager(StageLogic stageLogic)
        {
            _stageLogic = stageLogic;
        }

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
                        var removeSection = _stageLogic.sectionManager.GetSectionData(sectionData.Item1);
                        if (removeSection != null)
                        {
                            removeSection.RemoveSectionMonsterData(monster.Value);
                        }
                        else _stageLogic.errorOccurred.Invoke(Define.Errors.E_LogicError);

                        var moveSection = _stageLogic.sectionManager.GetSectionData(sectionData.Item2);
                        if (moveSection != null)
                        {
                            moveSection.AddSectionMonsterData(monster.Value);
                        }
                        else _stageLogic.errorOccurred.Invoke(Define.Errors.E_LogicError);

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
                var sectionData = _stageLogic.sectionManager.GetSectionData(monster.GetSectionIndex());

                if (sectionData != null)
                {
                    sectionData.RemoveSectionMonsterData(monster);
                }
                else _stageLogic.errorOccurred(Define.Errors.E_LogicError);
            }

            if(monsters.Count != 0)
            {
                _stageLogic.monsterManager.SetActiveCount();
            }
        }

        public Monster AddMonster(long CreateTick)
        {
            var createMonster = _monsters.FirstOrDefault(monster => monster.Value.State == Define.MonsterState.dead || monster.Value.State == Define.MonsterState.wait);
            createMonster.Value.SetMonster(CreateTick, _stageLogic.StageLevel);
            var addMonsterSection = _stageLogic.sectionManager.GetSectionData(createMonster.Value.NowSectionIndex);
            if (addMonsterSection != null)
            {
                addMonsterSection.AddSectionMonsterData(createMonster.Value);
            }
            else
            {
                _stageLogic.errorOccurred.Invoke(Define.Errors.E_LogicError);
            }
            return createMonster.Value;
        }

        private void SetMonsterInit()
        {
            if (_monsters.Count == 0)
            {
                for (int i = 0; i < 150; i++)
                {
                    var monster = new Monster(_stageLogic, i);
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

        public List<MonsterCheckData> GetMonsterCheckDatas()
        {
            List<MonsterCheckData> monsterCheckData = new List<MonsterCheckData>();
            var activeMonsters = _monsters.Where(_ => _.Value.State == Define.MonsterState.active).ToList();

            foreach(var activeMonster in activeMonsters)
            {
                var checkData = new MonsterCheckData();
                checkData.objectID = activeMonster.Value.GetObjectID();
                var sectionIndex = activeMonster.Value.GetSectionIndex();
                checkData.nowSectionIndexX = sectionIndex.Item1;
                checkData.nowSectionIndexY = sectionIndex.Item2;
                checkData.currentHP = activeMonster.Value.GetCurrentHP();

                monsterCheckData.Add(checkData);
            }

            return monsterCheckData;
        }
    }
}
