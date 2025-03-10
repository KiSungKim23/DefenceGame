using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Logic
{
    public class Section
    {
        private StageLogic _stageLogic;

        List<Monster> _monsters;
        private (int, int) _sectionIndex;
        private Vector3 _worldPosition;
        public Vector3 _exitPosition;

        List<Unit> _attackWaitUnits;
        List<Skill> _activeWaitSkills;

        public Action<Skill> ActiveSkill;

        public Section(StageLogic stageLogic, (int, int) sectionIndex)
        {
            _stageLogic = stageLogic;
            _monsters = new List<Monster>();
            _attackWaitUnits = new List<Unit>();
            _activeWaitSkills = new List<Skill>();
            _sectionIndex = sectionIndex;
            SetSectionPosition(sectionIndex);
        }

        public void Update(long currentTick)
        {
            foreach(var unit in _attackWaitUnits)
            {
                if (_monsters.Count > 0)
                    unit.UnitAttack(currentTick, this);
            }

            foreach (var skill in _activeWaitSkills)
            {
                if (skill.CheckActive() == false && skill.GetActiveTick() <= currentTick)
                {
                    skill.Active(this, _monsters);
                }
            }
        }

        private void SetSectionPosition((int, int) sectionIndex)
        {
            float positionX = (sectionIndex.Item1 * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2));
            float positionY = (sectionIndex.Item2 * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2));

            _worldPosition = new Vector3(positionX, positionY, 0);

            float halfSecttionSize = Define.SectionSize / 2;
            int maxIndex = Define.SectionCount - 1;

            float exitX = positionX;
            float exitY = positionY;

            if (sectionIndex.Item1 == 0 && sectionIndex.Item2 <= maxIndex - 1 && sectionIndex.Item2 > 0)
            {
                exitY -= halfSecttionSize;
            }
            else if (sectionIndex.Item1 >= 0 && sectionIndex.Item1 < maxIndex && sectionIndex.Item2 == 0)
            {
                exitX += halfSecttionSize;
            }
            else if (sectionIndex.Item1 == maxIndex && sectionIndex.Item2 >= 0 && sectionIndex.Item2 < maxIndex)
            {
                exitY += halfSecttionSize;
            }
            else if (sectionIndex.Item1 <= maxIndex - 1 && sectionIndex.Item1 > 0 && sectionIndex.Item2 == maxIndex)
            {
                exitX -= halfSecttionSize;
            }

            _exitPosition = new Vector3(exitX, exitY, 0);
        }

        public void RemoveSectionMonsterData(Monster monster)
        {
            _monsters.Remove(monster);
        }

        public void AddSectionMonsterData(Monster monster)
        {
            _monsters.Add(monster);
        }

        public void AddSkill(Skill skill)
        {
            _activeWaitSkills.Add(skill);
        }

        public long CheckActiveSkillTick()
        {
            var noneActiveSkill = _activeWaitSkills.FindAll(_ => _.CheckActive() == false);
            long ret = 0;

            foreach(var skill in noneActiveSkill)
            {
                var tick = skill.GetActiveTick();
                ret = ret == 0 || tick < ret ? tick : ret;
            }
            return ret;
        }

        public Vector3 GetSectionWorldPosition()
        {
            return _worldPosition;
        }

        public Vector3 GetSectionExitPosition()
        {
            return _exitPosition;
        }

        public void AddAttackWaitUnit(Unit unit)
        {
            if (_attackWaitUnits.Any(_ => _ == unit) == false)
            {
                _attackWaitUnits.Add(unit);
            }
        }

        public void ClearWaitUnitData(Unit unit)
        {
            if (_attackWaitUnits.Any(_ => _ == unit) == true)
            {
                _attackWaitUnits.Remove(unit);
            }
        }

        public int GetSectionDataCount()
        {
            return _monsters.Count;
        }

        public (int, int) GetSectionIndex()
        {
            return _sectionIndex;
        }

        public long CheckGetUnitAttackTick()
        {
            long ret = long.MaxValue;

            foreach(var attackWaitUnit in _attackWaitUnits)
            {
                if (attackWaitUnit.CanAttackTick < ret)
                    ret = attackWaitUnit.CanAttackTick;
            }

            return ret;
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

        private (int, int) GetPreviousSectionIndex((int, int) currentIndex)
        {
            int maxIndex = Define.SectionCount - 1;

            if (currentIndex.Item1 == 0 && currentIndex.Item2 < maxIndex)
            {
                return (currentIndex.Item1, currentIndex.Item2 + 1);
            }
            else if (currentIndex.Item2 == 0 && currentIndex.Item1 > 0)
            {
                return (currentIndex.Item1 - 1, currentIndex.Item2);
            }
            else if (currentIndex.Item1 == maxIndex && currentIndex.Item2 > 0 && currentIndex.Item2 < maxIndex)
            {
                return (maxIndex, currentIndex.Item2 - 1);
            }
            else if (currentIndex.Item1 == maxIndex && currentIndex.Item2 == maxIndex)
            {
                return (maxIndex - 1, maxIndex);
            }
            else if (currentIndex.Item2 == maxIndex && currentIndex.Item1 < maxIndex)
            {
                return (currentIndex.Item1 + 1, maxIndex);
            }

            return (0, maxIndex);
        }

        public Section GetNextSection()
        {
            (int, int) nextIndex = GetNextSectionIndex(_sectionIndex);
            var nextSection = _stageLogic.sectionManager.GetSectionData(nextIndex);
            if (nextSection == null)
            {
                _stageLogic.errorOccurred.Invoke(Define.Errors.E_LogicError);
            }
            return nextSection;
        }

        public Section GetPreviousSection()
        {
            (int, int) previousIndex = GetPreviousSectionIndex(_sectionIndex);
            var previousSection = _stageLogic.sectionManager.GetSectionData(previousIndex);
            if (previousSection == null)
            {
                _stageLogic.errorOccurred.Invoke(Define.Errors.E_LogicError);
            }
            return previousSection;
        }
    }
}
