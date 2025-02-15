using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class SkillInfoData
    {
        Define.SkillType _skillType;

        long _probability;
        int _skillLevel;

        public SkillInfoData(Define.SkillType skillType, int skillLevel)
        {
            //이후에 스크립트에 넣어서 레벨별 데미지 세팅하는걸로 변경
            _skillType = skillType;
            _skillLevel = skillLevel;
        }

        public Define.SkillType GetSkillType()
        {
            return _skillType;
        }

        public int GetSkillLevel()
        {
            return _skillLevel;
        }
    }
}
