using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    [Serializable]
    [MessagePackObject]
    public class UnitInfoScript
    {
        [Key(0)] public int unitUID;
        [Key(1)] public Define.UnitGrade unitGrade;
        [Key(2)] public float attackRange;
        [Key(3)] public float attackSpeed;
    }
}