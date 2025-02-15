using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class UnitInfoScript
    {
        [Key(0)] public int unitUID;
        [Key(1)] public Define.UnitGrade unitGrade;
        [Key(2)] public float attackRange;
        [Key(3)] public float attackSpeed;
        [Key(4)] public Define.SkillType baseSkillType;
        [Key(5)] public Define.SkillType skillType1;
        [Key(6)] public Define.SkillType skillType2;
        [Key(7)] public Define.SkillType skillType3;
        [Key(8)] public string unitName;
        [Key(9)] public string objectPrefabName;
        [Key(10)] public string iconPrefabName;
    }
}
