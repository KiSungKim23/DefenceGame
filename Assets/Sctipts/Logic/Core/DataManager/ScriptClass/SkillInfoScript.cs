using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class SkillInfoScript
    {
        [Key(0)] public Define.SkillType skillType;
        [Key(1)] public long probability;
        [Key(2)] public long durationTick;
        [Key(3)] public int skillLevel;
        [Key(4)] public int damage;
        [Key(4)] public int addDamage;
    }
}