using MessagePack;
using System;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class UnitInfoScript
    {
        [Key(0)] public int unitUID;
        [Key(1)] public int unitGrade;
        [Key(2)] public float attackRange;
        [Key(3)] public float attackSpeed;
        [Key(4)] public int baseSkillID;
        [Key(5)] public int skill1ID;
        [Key(6)] public int skill2ID;
        [Key(7)] public int skill3ID;
        [Key(8)] public string unitName;
        [Key(9)] public string objectPrefabName;
        [Key(10)] public string iconPrefabName;
    }
}
