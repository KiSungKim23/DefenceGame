using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class Skill
    {
        SkillInfoScript _skillInfo;

        long _activeTick;

        List<BuffInfoScript> _buffInfoList = new List<BuffInfoScript>();

        int _damage;
        long _durationTick;

        bool _acktive;

        public Skill(SkillInfoScript skillInfo, long createTick)
        {
            _skillInfo = skillInfo;
            _acktive = false;

            var skillBuffList = StageLogic.Data.GetSkillBuffInfoScriptListAll().FindAll(_ => _.skillType == _skillInfo.skillType);

            foreach (var buffInfo in skillBuffList)
            {
                _buffInfoList.Add(StageLogic.Data.GetBuffInfoScriptDictionary(buffInfo.buffUID));
            }

            _damage = skillInfo.damage;
            _durationTick = skillInfo.durationTick;

            _activeTick = createTick + _durationTick;
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
                monster.GetDamaged(_damage);

                foreach (var buff in _buffInfoList)
                {
                    monster.AddBuff(new Buff(buff, _activeTick));
                }
            }

            sectionData.ActiveSkill.Invoke(this);

            var deadMonsterList = monsters.FindAll(_ => _.State == Define.MonsterState.dead);
            foreach (var monster in deadMonsterList)
            {
                StageLogic.Instance.SectionDatas[monster.GetSectionIndex()].RemoveSectionMonsterData(monster);
            }

        }

        public bool CheckActive()
        {
            return _acktive;
        }
    }
}