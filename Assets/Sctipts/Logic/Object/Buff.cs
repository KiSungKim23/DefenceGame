using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class Buff
    {
        BuffInfo _buffInfo;

        long _endTick;
        float _value;

        public Buff(BuffInfo buffInfo, long createTick)
        {
            _buffInfo = buffInfo;
            _endTick = createTick + _buffInfo.GetDurationTick();
            _value = _buffInfo.GetValue();
        }

        public long GetEndTick()
        {
            return _endTick;
        }

        public float GetValue()
        {
            return _value;
        }

    }
}