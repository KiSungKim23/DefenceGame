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
        [Key(2)] public long durationTick;
        [Key(3)] public float value1;
        [Key(4)] public float value2;
    }
}
