using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Logic
{

    public class Monster : Object
    {
        float destValue = (Define.SectionSize * (Define.SectionCount / 2)) - (Define.SectionSize / 2);

        Vector3[] waypoints = new Vector3[]
        {
            new Vector3(-(Define.SectionSize * (Define.SectionCount / 2)) + (Define.SectionSize / 2), (Define.SectionSize * (Define.SectionCount / 2)) - (Define.SectionSize / 2), 0),
            new Vector3(-(Define.SectionSize * (Define.SectionCount / 2)) + (Define.SectionSize / 2), -(Define.SectionSize * (Define.SectionCount / 2)) + (Define.SectionSize / 2), 0),
            new Vector3((Define.SectionSize * (Define.SectionCount / 2)) - (Define.SectionSize / 2), -(Define.SectionSize * (Define.SectionCount / 2)) + (Define.SectionSize / 2), 0),
            new Vector3((Define.SectionSize * (Define.SectionCount / 2)) - (Define.SectionSize / 2), (Define.SectionSize * (Define.SectionCount / 2)) - (Define.SectionSize / 2), 0)
        };

        Define.MonsterState _state;
        public Define.MonsterState State { get { return _state; } }

        public Vector3 _destPosition;

        public float _speed;
        private int _waypointIndex = 0;

        (int, int) _nowSectionIndex;

        private int _objectID;

        private long _maxHP;
        private long _currentHP;
        private int _armor;

        public List<Buff> _buffList = new List<Buff>();

        public (int, int) NowSectionIndex { get { return _nowSectionIndex; } }

        public Monster(int objectID)
        {
            _objectID = objectID;
            _state = Define.MonsterState.wait;
            _position = waypoints[0];
            _position.Y += (Define.SectionSize / 3);
            _destPosition = waypoints[1];
            _waypointIndex = 1;
            _nowSectionIndex = GetSectionIndex();
            _maxHP = 0;
            _currentHP = 0;
            _speed = 0;
            _armor = 0;
        }

        public override void Init(long createTick)
        {
            _currentTick = createTick;
            _state = Define.MonsterState.active;
            _position = waypoints[0];
            _position.Y += (Define.SectionSize / 3);
            _destPosition = waypoints[1];
            _waypointIndex = 1;
            _nowSectionIndex = GetSectionIndex();
        }


        public void SetMonster(long createTick, int stageLevel)
        {
            Init(createTick);

            var monsterInfoScript = StageLogic.Data.GetMonsterInfoScriptDictionary(stageLevel);
            _maxHP = monsterInfoScript.maxHP;
            _currentHP = monsterInfoScript.maxHP;
            _speed = monsterInfoScript.speed;
            _armor = monsterInfoScript.armor;
        }

        public void Update(long tick)
        {
            if (_state == Define.MonsterState.active)
            {
                var value = CheckPosition(tick);
                _position = value.position;
                _destPosition = value.destPosition;
                _waypointIndex = value.waypointIndex;
            }

            _currentTick = tick;
        }

        public (Vector3 position, Vector3 destPosition, int waypointIndex, (int, int) reachSection) CheckPosition(long tick, bool checkSection = false)
        {
            float movingDistance = GetMonsterMovingDistance(tick);

            Vector3 currentPosition = _position;
            Vector3 destination = _destPosition;

            int waypointIndex = _waypointIndex;
            (int, int) reachSection = _nowSectionIndex;

            while (movingDistance > 0)
            {
                float distanceToWaypoint = Vector3.Distance(currentPosition, destination);

                if (movingDistance >= distanceToWaypoint)
                {
                    movingDistance -= distanceToWaypoint;
                    currentPosition = destination;
                    waypointIndex = (waypointIndex + 1) % waypoints.Length;
                    destination = waypoints[waypointIndex];
                }
                else
                {
                    Vector3 direction = Vector3.Normalize(destination - currentPosition);
                    currentPosition += direction * movingDistance;
                    movingDistance = 0;
                }
            }

            if (checkSection)
            {
                reachSection = CheckSectionIndex(currentPosition);
            }

            return (currentPosition, destination, waypointIndex, reachSection);
        }

         
        public ((int, int), (int, int)) GetSectionData()
        {
            return (_nowSectionIndex, GetSectionIndex());
        }

        public void SetSectionIndex((int, int) sectionData)
        {
            _nowSectionIndex = sectionData;
        }

        public (int, int) CheckSectionIndex(Vector3 position)
        {
            int sectionX = (int)((position.X + (Define.SectionSize * (Define.SectionCount / 2))) / Define.SectionSize);
            int sectionY = (int)((position.Y + (Define.SectionSize * (Define.SectionCount / 2))) / Define.SectionSize);
            return (sectionX, sectionY);
        }

        public (int, int) GetSectionIndex()
        {
            return CheckSectionIndex(_position);
        }

        public void GetDamaged(long baseDamage,float percentDamage)
        {
            _currentHP -= baseDamage;

            if (_currentHP < 0)
            {
                _state = Define.MonsterState.dead;
            }
        }

        public void AddBuff(Buff buff)
        {
            _buffList.Add(buff);
        }

        public long CheckUpdateTick(long tick)
        {
            long checkTick = _currentTick;
            long ret = tick;
            (int, int) index = _nowSectionIndex;

            bool isFirst = false;

            Section checkSection = StageLogic.Instance.SectionDatas[index];

            while (checkTick < tick)
            {
                long exitTick = GetExitTick(checkTick, checkSection, isFirst);
                long attackTick = checkSection.CheckGetUnitAttackTick();

                if (attackTick < checkTick)
                {
                    return checkTick;
                }

                if (attackTick < exitTick)
                {
                    return attackTick;
                }

                checkTick = exitTick;
                checkSection = GetNextSection(checkSection);
                isFirst = true;
            }

            return ret;
        }

        private long GetExitTick(long enterTick, Section section, bool isFirst)
        {
            float sectionDistance = Define.SectionSize;

            if (isFirst)
            {
                if(_destPosition == section.GetSectionWorldPosition())
                {
                    sectionDistance = Vector3.Distance(_position, _destPosition) + Define.SectionSize / 2;
                }
                else
                {
                    sectionDistance = Vector3.Distance(_position, section.GetSectionExitPosition());
                }
            }

            if (_speed <= 0) return long.MaxValue;

            long travelTick = GetTravelTick(sectionDistance, enterTick);
            return enterTick + travelTick;
        }

        private Section GetNextSection(Section currentSection)
        {
            (int, int) nextIndex = GetNextSectionIndex(currentSection.GetSectionIndex());
            return StageLogic.Instance.SectionDatas[nextIndex];
        }

        private (int, int) GetNextSectionIndex((int, int) currentIndex)
        {
            int maxIndex = Define.SectionCount - 1;

            if (currentIndex.Item1 == 0 && currentIndex.Item2 > 0)
            {
                return (currentIndex.Item1, currentIndex.Item2 - 1);
            }
            else if (currentIndex.Item2 == 0 && currentIndex.Item1 < maxIndex)
            {
                return (currentIndex.Item1 + 1, currentIndex.Item2);
            }
            else if (currentIndex.Item1 == maxIndex && currentIndex.Item2 < maxIndex)
            {
                return (currentIndex.Item1, currentIndex.Item2 + 1);
            }
            else if (currentIndex.Item2 == maxIndex && currentIndex.Item1 > 0)
            {
                return (currentIndex.Item1 - 1, currentIndex.Item2);
            }

            return (0, maxIndex);
        }

        public int GetObjectID()
        {
            return _objectID;
        }

        public long GetMaxHP()
        {
            return _maxHP;
        }

        public long GetCurrentHP()
        {
            return _currentHP;
        }

        private float GetMonsterMovingDistance(long updateTick)
        {

            float currentBuffValue = _buffList.Where(_ => _.GetEndTick() > _currentTick).Sum(_ => _.GetEffectStrength());
            var buffList = _buffList.OrderBy(_ => _.GetEndTick()).Where(_ => _.GetEndTick() > _currentTick).ToList();

            long buffEndTick = 0;
            long tempTick = _currentTick;

            float ret = 0;
            foreach (var buff in buffList)
            {
                buffEndTick = buff.GetEndTick();

                var speed = _speed * (1 - (currentBuffValue > 1 ? 1 : currentBuffValue));

                if (buffEndTick >= updateTick)
                {
                    ret += (updateTick - tempTick) * speed * Define.SpeedScaleFactor;
                    break;
                }

                ret += (buffEndTick - tempTick) * speed * Define.SpeedScaleFactor;
                tempTick = buffEndTick;
                currentBuffValue -= buff.GetEffectStrength();
            }

            return ret == 0 ? (updateTick - _currentTick) * _speed * Define.SpeedScaleFactor : ret;
        }


        private long GetTravelTick(float distance, long startTick)
        {
            long currentTick = startTick;
            float distanceTemp = distance;

            float currentBuffValue = _buffList.Where(_ => _.GetEndTick() > startTick).Sum(_ => _.GetEffectStrength());
            List<Buff> buffList = _buffList.Where(_ => _.GetEndTick() > startTick).OrderBy(_ => _.GetEndTick()).ToList();

            if (buffList.Count == 0)
            {
                return (long)(distanceTemp / _speed / Define.SpeedScaleFactor);
            }

            foreach (var buff in buffList)
            {
                long buffEndTick = buff.GetEndTick();
                long temp = buffEndTick - currentTick;
                float currentSpeed = _speed * (1 - (currentBuffValue < 1.0f ? currentBuffValue : 1.0f));

                if (currentSpeed <= 0)
                {
                    currentTick = buffEndTick;
                    continue;
                }
                float MovementDistance = temp * currentSpeed * Define.SpeedScaleFactor;

                if (MovementDistance >= distanceTemp)
                {
                    currentTick += (long)(distanceTemp / currentSpeed / Define.SpeedScaleFactor);
                    return currentTick - startTick;
                }
                else
                {
                    distanceTemp -= MovementDistance ;
                    currentTick = buffEndTick;

                    currentBuffValue -= buff.GetEffectStrength();
                }
            }

            currentTick += (long)(distanceTemp / _speed / Define.SpeedScaleFactor);

            return currentTick - startTick;
        }
    }
}



