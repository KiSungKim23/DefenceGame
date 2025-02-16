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
        [Key(2)] public int buffGrade;
        [Key(3)] public float durationTime;
        [Key(4)] public float effectStrength;
        [Key(5)] public string buffName;
    }
}
