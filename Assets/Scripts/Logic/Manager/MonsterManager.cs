using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class MonsterManager
    {
        long _tick = 0;

        private Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();

        public void Init(long tick)
        {
            _tick = tick;
            SetMonsterInit();
        }

        public void Update(long tick)
        {
            _tick = tick;

            foreach (var monster in _monsters)
            {
                if (monster.Value.State == Define.MonsterState.active)
                {
                    monster.Value.Update(_tick);

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

        public long GetUpdateTick(long currentTick)
        {
            long ret = currentTick;

            foreach (var monster in _monsters)
            {
                if (monster.Value.State == Define.MonsterState.active)
                {
                    long checkTick = monster.Value.CheckUpdateTick(currentTick);
                    ret = checkTick < ret ? checkTick : ret;
                }
            }

            return ret;

        }
    }
}
