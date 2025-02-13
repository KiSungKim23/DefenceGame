using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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