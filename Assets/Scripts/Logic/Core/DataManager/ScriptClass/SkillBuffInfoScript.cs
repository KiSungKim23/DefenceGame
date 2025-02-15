using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class SkillBuffInfoScript
    {
        [Key(0)] public Define.SkillType skillType;
        [Key(1)] public int buffUID;
    }
}
