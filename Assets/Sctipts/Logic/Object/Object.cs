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
            //long updateTick = tick - 10; //�ӽ÷� ������ �����δ� ������ ��� ���°� ����� ��� UpdateTick�� �߰���Ű�� ��
            //StageManager.UpdateTickSchuler.AddTick(updateTick);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

    }
}