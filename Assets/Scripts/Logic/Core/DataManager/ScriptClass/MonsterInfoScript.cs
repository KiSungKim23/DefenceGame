using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class MonsterInfoScript
    {
        [Key(0)] public int monsterUID;
        [Key(1)] public float speed;
        [Key(2)] public long maxHP;
        [Key(3)] public int armor;
        [Key(4)] public string monsterName;
        [Key(5)] public string monsterPrefab;
    }
}
