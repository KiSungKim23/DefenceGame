using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class SkillInfoScript
    {
        [Key(0)] public int skillUID;
        [Key(1)] public Define.SkillType skillID;
        [Key(2)] public int skillGrade;
        [Key(3)] public int skillRange;
        [Key(4)] public float SkillChainDelay;
        [Key(5)] public float probability;
        [Key(6)] public float castTime;
        [Key(7)] public float durationTime;
        [Key(8)] public long baseDamage;
        [Key(9)] public float damagePercent;
        [Key(10)] public long damageIncrease;
        [Key(11)] public string skillName;
        [Key(12)] public string effectPrefab;
    }
}
