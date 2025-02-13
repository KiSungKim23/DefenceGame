using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class UpdateScheduler
    {
        private long _currentTick = 0;

        private SortedSet<long> _tickSet = new SortedSet<long>();

        public void AddTick(long tick)
        {
            if (_tickSet.Contains(tick) || tick <= _currentTick)
            {
                return;
            }

            _tickSet.Add(tick);
        }

        public long GetUpdateTick(long currentTick)
        {
            if (_tickSet.Count <= 0)
            {
                if (_currentTick < currentTick)
                    _currentTick = currentTick;
                return -1;
            }
            var ret = _tickSet.Min;
            _tickSet.Remove(ret);

            return ret;
        }
    }
}
