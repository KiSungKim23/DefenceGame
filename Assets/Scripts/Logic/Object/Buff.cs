using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class Buff
    {
        BuffInfoScript _buffInfo;

        long _endTick;
        float _effectStrength;

        public Buff(BuffInfoScript buffInfo, long createTick)
        {
            _buffInfo = buffInfo;
            _endTick = createTick + (long)(buffInfo.durationTime * Define.OneSecondTick);
            _effectStrength = buffInfo.effectStrength;
        }

        public long GetEndTick()
        {
            return _endTick;
        }

        public float GetEffectStrength()
        {
            return _effectStrength;
        }

    }
}