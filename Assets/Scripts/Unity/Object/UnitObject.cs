using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class UnitObject : MonoBehaviour
    {
        Logic.Unit _unit;

        [SerializeField]
        public SpriteRenderer unitImage;

        long attackTick = 0;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(attackTick + Define.OneSecondTick / 4 >= DateTime.UtcNow.Ticks)
            {
                unitImage.color = new Color(100, 100, 100);
            }
            else
            {
                unitImage.color = new Color(0, 0, 0);
            }

        }

        public void SetUnitData(Logic.Unit unit)
        {
            _unit = unit;

            if(_unit != null)
            {
                var position = _unit.GetPosition();
                transform.position = new Vector3(position.X, position.Y, position.Z);
                _unit.unitAttack = UnitAttack;
            }
        }

        public void UnitAttack(long tick)
        {
            attackTick = tick;
        }

        public int GetUnitObjectIndex()
        {
            return _unit.GetObjectIndex();
        }

        public Logic.Unit GetUnitData()
        {
            return _unit;
        }

        public void MoveUnitPosition()
        {
            var position = _unit.GetPosition();
            transform.position = new Vector3(position.X, position.Y, position.Z);
        }
    }
}
