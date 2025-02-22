using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class Skill
    {
        SkillInfoScript _skillInfo;

        long _activeTick;

        List<BuffInfoScript> _buffInfoList = new List<BuffInfoScript>();

        long _damage;
        float _datamgePercent;
        long _durationTick;
        int _range;

        bool _acktive;

        public Skill(SkillInfoScript skillInfo, long createTick, int range = 0)
        {
            _skillInfo = skillInfo;
            _acktive = false;

            var skillBuffList = StageLogic.Instance.dataManager.GetSkillBuffInfoScriptListAll().FindAll(_ => _.skillUID == _skillInfo.skillUID);

            foreach (var buffInfo in skillBuffList)
            {
                _buffInfoList.Add(StageLogic.Instance.dataManager.GetBuffInfoScriptDictionary(buffInfo.buffUID));
            }

            _damage = skillInfo.baseDamage + 400;
            _datamgePercent = skillInfo.damagePercent;
            _durationTick = (long)(skillInfo.durationTime * Define.OneSecondTick);

            _activeTick = createTick + _durationTick + (long)(skillInfo.SkillChainDelay * range * Define.OneSecondTick);
        }

        public long GetActiveTick()
        {
            return _activeTick;
        }

        public void Active(Section sectionData, List<Monster> monsters)
        {
            _acktive = true;

            foreach (var monster in monsters)
            {
                if (monster.State == Define.MonsterState.active)
                {
                    monster.GetDamaged(_damage, _datamgePercent);

                    foreach (var buff in _buffInfoList)
                    {
                        monster.AddBuff(new Buff(buff, _activeTick));
                    }
                }
            }

            sectionData.ActiveSkill.Invoke(this);

            var deadMonsterList = monsters.FindAll(_ => _.State == Define.MonsterState.dead);
            StageLogic.Instance.monsterManager.MonsterDead(deadMonsterList);
        }

        public bool CheckActive()
        {
            return _acktive;
        }

        public void SetRange(int range)
        {
            _range = range;
        }
    }
}