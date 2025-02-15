using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInfo
{

    Define.BuffType _buffType;
    int _buffLevel;

    long _durationTick;
    float _value;

    public BuffInfo(Define.BuffType buffType, int buffLevel)
    {
        _buffType = buffType;
        _buffLevel = buffLevel;

        _durationTick = Define.OneSecondTick;

        switch (_buffType)
        {
            case Define.BuffType.Armor:
                _value = 1f;
                break;
            case Define.BuffType.Moving:
                if (_buffLevel == 100)
                {
                    _value = 1f;
                }
                else
                {
                    _value = 0.8f;
                }
                break;
        }
    }

    public long GetDurationTick()
    {
        return _durationTick;
    }

    public float GetValue()
    {
        return _value;
    }

}
