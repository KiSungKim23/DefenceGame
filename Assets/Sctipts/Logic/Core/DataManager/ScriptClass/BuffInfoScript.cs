using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class BuffInfoScript
    {
        [Key(0)] public Define.BuffType buffType;
        [Key(1)] public int buffUID;
        [Key(2)] public int buffLevel;
        [Key(3)] public long durationTick;
        [Key(4)] public float value1;
        [Key(5)] public float value2;
    }
}