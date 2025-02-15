using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class UnitUnionInfoScript
    {
        [Key(0)] public int unitUID;
        [Key(1)] public int mainMaterialUID;
        [Key(2)] public int materail2UID;
        [Key(3)] public int materail3UID;
        [Key(4)] public int materail4UID;
        [Key(5)] public int materail5UID;
        [Key(6)] public int materail6UID;
    }
}
