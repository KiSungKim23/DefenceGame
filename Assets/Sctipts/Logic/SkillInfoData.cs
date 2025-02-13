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
            //���Ŀ� ��ũ��Ʈ�� �־ ������ ������ �����ϴ°ɷ� ����
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
