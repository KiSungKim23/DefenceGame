using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Logic
{
    public abstract class Object
    {
        protected long _currentTick;
        protected long _removeTick;
        protected int _uid;

        protected Vector3 _position;

        public abstract void Init(long createTick);

        public void CheckUpdateTick(long tick)
        {
            //long updateTick = tick - 10; //임시로 넣은거 실제로는 몬스터의 경우 상태가 변경될 경우 UpdateTick에 추가시키려 함
            //StageManager.UpdateTickSchuler.AddTick(updateTick);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

    }
}