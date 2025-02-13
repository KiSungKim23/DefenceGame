using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class Skill
    {
        SkillInfoData _skillInfo;

        long _activeTick;

        List<BuffInfo> buffInfoList = new List<BuffInfo>();

        int _damage;
        long _durationTick;

        bool _acktive;

        public Skill(SkillInfoData skillInfo, long createTick)
        {
            _skillInfo = skillInfo;
            _acktive = false;

            switch (_skillInfo.GetSkillType())
            {
                case Define.SkillType.Attack:
                    _damage = 20 * skillInfo.GetSkillLevel();
                    _durationTick = 0;// Define.OneSecondTick / 4;
                    break;
                case Define.SkillType.Slow:
                    _damage = 0 * skillInfo.GetSkillLevel();
                    _durationTick = 0;// Define.OneSecondTick / 2;
                    buffInfoList.Add(new BuffInfo(Define.BuffType.Moving, 1));
                    break;
                case Define.SkillType.Stun:
                    _damage = 20 * skillInfo.GetSkillLevel();
                    _durationTick = 0;//Define.OneSecondTick / 2;
                    buffInfoList.Add(new BuffInfo(Define.BuffType.Moving, 100));
                    break;
            }

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

                foreach (var buff in buffInfoList)
                {
                    monster.AddBuff(new Buff(buff, _activeTick));
                }
            }

            sectionData.ActiveSkill.Invoke(this);

            var deadMonsterList = monsters.FindAll(_ => _.State == Define.MonsterState.dead);
            foreach(var monster in deadMonsterList)
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