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
        [Key(2)] public int maxHP;
        [Key(3)] public string monsterPrefab;
    }
}
