using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class BuffInfoScript
    {
        [Key(0)] public int buffUID;
        [Key(1)] public Define.BuffType buffType;
        [Key(2)] public int buffLevel;
        [Key(3)] public long durationTick;
        [Key(4)] public float value1;
        [Key(5)] public float value2;
    }
}
