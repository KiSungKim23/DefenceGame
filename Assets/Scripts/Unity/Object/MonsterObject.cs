using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class MonsterObject : MonoBehaviour
    {
        Logic.Monster _monster;

        public long maxHP;
        public long currentHP;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_monster != null)
            {
                var positionData = _monster.GetPosition();
                transform.position = new Vector3(positionData.X, positionData.Y, positionData.Z);


                long monsterHP = _monster.GetCurrentHP();
                if (monsterHP != currentHP)
                {
                    currentHP = monsterHP;
                }

                if(_monster.State == Define.MonsterState.dead)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

        public void SetMonsterData(Logic.Monster monster)
        {
            _monster = monster;
            if (_monster != null)
            {
                var position = _monster.GetPosition();
                transform.position = new Vector3(position.X, position.Y, position.Z);
                maxHP = _monster.GetMaxHP();
                currentHP = _monster.GetCurrentHP();
            }
        }
    }
}
