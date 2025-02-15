using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class Buff
    {
        BuffInfoScript _buffInfo;

        long _endTick;
        float _value1;
        float _value2;

        public Buff(BuffInfoScript buffInfo, long createTick)
        {
            _buffInfo = buffInfo;
            _endTick = createTick + _buffInfo.durationTick;
            _value1 = _buffInfo.value1;
            _value2 = _buffInfo.value2;
        }

        public long GetEndTick()
        {
            return _endTick;
        }

        public float GetValue1()
        {
            return _value1;
        }

        public float GetValue2()
        {
            return _value2;
        }
    }
}