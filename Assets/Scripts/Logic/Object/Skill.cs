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

        bool _acktive;

        public Skill(SkillInfoScript skillInfo, long createTick)
        {
            _skillInfo = skillInfo;
            _acktive = false;

            var skillBuffList = StageLogic.Instance.dataManager.GetSkillBuffInfoScriptListAll().FindAll(_ => _.skillUID == _skillInfo.skillUID);

            foreach (var buffInfo in skillBuffList)
            {
                _buffInfoList.Add(StageLogic.Instance.dataManager.GetBuffInfoScriptDictionary(buffInfo.buffUID));
            }

            _damage = skillInfo.baseDamage;
            _datamgePercent = skillInfo.damagePercent;
            _durationTick = (long)(skillInfo.durationTime * Define.OneSecondTick);

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
                monster.GetDamaged(_damage, _datamgePercent);

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