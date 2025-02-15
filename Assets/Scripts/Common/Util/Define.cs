using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;

public class Define 
{
    static public float DuringStageTime = 10;
    static public float ReadyStageTime = 1;
    static public float MonsterCreateTime = 1;

    static public int MonsterMaxCount = 100;
    static public int MonsterStageCreateCount = 5;

    static public int MaxUnitCount = 10;

    static public float SpeedScaleFactor = 0.0000001f;

    static public float SectionSize = 0.5f;

    static public int SectionCount = 10;

    static public int SectionUISize = 96;

    static public int InSctionUnitCount = 1;

    static public long OneSecondTick = 10000000;

    public enum Scene
    {
        Unknown,
        ResourceRelease,
        GameScene,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public  enum UnitGrade
    {
        Normal,
        Hero,
        Unique,
        Legend
    }

    public enum MonsterState
    {
        wait = 0,
        active = 1,
        dead = 3,
    }
    public enum UnitState
    {
        wait = 0,
        active = 1,
    }

    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
    }

    public enum PannalType
    {
        Unit,
        Upgrade,
        Draw,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SkillType
    {
        None = 0, //�̰� ���߿� ��ų �ϳ� �ϳ� ����͵� ���� ������ �ɵ� ex) FireBall �� �̷� ����
        Attack = 1, 
        Slow = 2,
        Stun = 3, 
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BuffType
    {
        Moving = 1,// �̵� ���� ����
        Armor = 2,// ���� ���� ����
    }

    public enum SectionButtonType
    {
        Monster,
        Unit,
    }
}
