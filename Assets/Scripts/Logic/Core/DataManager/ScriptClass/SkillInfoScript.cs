using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class SkillInfoScript
    {
        [Key(0)] public Define.SkillType skillType;
        [Key(1)] public float probability;
        [Key(2)] public long durationTick;
        [Key(3)] public int damage;
        [Key(4)] public int addDamage;
    }
}
