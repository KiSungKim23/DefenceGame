using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Logic
{

    public class Unit : Object
    {
        Define.UnitState _state;

        private int _objectIndex;

        public Define.UnitState State { get { return _state; } }

        public Section _targetSection;
        public long _targetSetTick;
        private UnitInfoData _unitInfoData;

        private List<Section> _canAttackSectionData = new List<Section>();
        public Action<long> unitAttack;

        private SkillInfoData _unitSkillData;

        private (int, int) _sectionIndex;

        private float _attackRange;
        private float _attackSpeed;
        private long _canAttackTick;

        public long CanAttackTick { get { return _canAttackTick; } }
        public (int, int) SectionIndex { get { return _sectionIndex; } }


        public Unit(int objectIndex, UnitInfoData unitInfo, (int, int) section)
        {
            _targetSetTick = 0;
            _objectIndex = objectIndex;
            _sectionIndex = section;
            //uid 기반으로 값 세팅 -> 스킬리스트, 공격 속도 ,공격 범위 
            //나중에 스크립트 읽는거 만들어서 스크립트 세팅후 메니저를 통해 값 가져오는 걸로

            _unitInfoData = unitInfo;
            SetStat(unitInfo.GetUID());
            SetWorldPosition();
            CheckCanAttackSection();
        }

        public void Update(long currentTick)
        {
            if(_targetSection != null && StageLogic.Instance.CheckUnitAttack())
            {
                if(_canAttackTick <= _targetSetTick)
                {
                    _canAttackTick = _targetSetTick <= _currentTick ? _targetSetTick : _currentTick;
                }

                while (_canAttackTick <= currentTick)
                {
                    Skill addSkill = new Skill(_unitSkillData, _canAttackTick);
                    _targetSection.AddSkill(addSkill);
                    unitAttack.Invoke(_canAttackTick);

                    _canAttackTick += (long)(_attackSpeed * Define.OneSecondTick);
                }
                _currentTick = currentTick;
            }
        }

        public void Clear()
        {
            foreach (var section in _canAttackSectionData)
            {
                section.ClearWaitUnitData(this);
            }
            _canAttackSectionData.Clear();
        }

        public override void Init(long createTick)
        {
            _currentTick = createTick;
            _state = Define.UnitState.wait;
            _canAttackTick = createTick + (long)(_attackSpeed * Define.OneSecondTick);
        }

        public void MovePosition((int, int) moveSectionIndex)
        {
            _sectionIndex = moveSectionIndex;
            SetWorldPosition();
            CheckCanAttackSection();

            if (_targetSection != null)
            {
                if (_canAttackSectionData.Any(_ => _.GetSectionIndex() == _targetSection.GetSectionIndex()) == false)
                {
                    _targetSection = null;
                }
                else
                {
                    foreach (var section in _canAttackSectionData)
                    {
                        section.ClearWaitUnitData(this);
                    }
                    _canAttackSectionData.Clear();
                }
            }

        }

        public void SetWorldPosition()
        {
            _position.X = (_sectionIndex.Item1 * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2));
            _position.Y = (_sectionIndex.Item2 * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2));
            _position.Z = 0f;
        }

        public void CheckCanAttackSection()
        {
            if (_canAttackSectionData.Count != 0)
            {
                foreach (var section in _canAttackSectionData)
                {
                    section.ClearWaitUnitData(this);
                }
                _canAttackSectionData.Clear();
            }

            foreach (var sectionInfo in StageLogic.Instance.SectionDatas)
            {
                if (Vector3.Distance(_position, sectionInfo.Value.GetSectionWorldPosition()) < _attackRange)
                {
                    _canAttackSectionData.Add(sectionInfo.Value);
                    sectionInfo.Value.AddAttackWaitUnit(this);
                }
            }
        }

        public void UnitAttack(long tick, Section sectionData)
        {
            if (StageLogic.Instance.CheckUnitAttack() && _canAttackTick <= tick)
            {
                //skill 계산 후 Section에 넣어주기
                Skill addSkill = new Skill(_unitSkillData, tick);
                sectionData.AddSkill(addSkill);
                unitAttack.Invoke(tick);
                _canAttackTick = tick + (long)(_attackSpeed * Define.OneSecondTick);
            }
        }

        public int GetUID()
        {
            return _uid;
        }

        public int GetObjectIndex()
        {
            return _objectIndex;
        }

        private void SetStat(int uid)
        {



            switch(uid)
            {
                case 1:
                    _attackRange = 2f;
                    _attackSpeed = 2f;

                    _unitSkillData = new SkillInfoData(Define.SkillType.Slow, 1);
                    break;
                case 2:
                    _attackRange = 3f;
                    _attackSpeed = 2f;

                    _unitSkillData = new SkillInfoData(Define.SkillType.Attack, 1);
                    break;
                case 3:
                    _attackRange = 2f;
                    _attackSpeed = 1f;

                    _unitSkillData = new SkillInfoData(Define.SkillType.Attack, 1);
                    break;
                case 4:
                    _attackRange = 5f;
                    _attackSpeed = 1f;

                    _unitSkillData = new SkillInfoData(Define.SkillType.Attack, 1);
                    break;
                case 5:
                    _attackRange = 4f;
                    _attackSpeed = 1f;

                    _unitSkillData = new SkillInfoData(Define.SkillType.Attack, 1);
                    break;
            }

        }

        public UnitInfoData GetUnitInfoData()
        {
            return _unitInfoData;
        }

        public (int, int) GetSectionIndex()
        {
            return _sectionIndex;
        }

        public List<Section> GetCanAttackSections()
        {
            return _canAttackSectionData;
        }

        public void RemoveTarget()
        {
            if(_targetSection != null)
            {
                CheckCanAttackSection();
                _targetSection = null;
            }
        }

        public void SetTarget((int, int) targetSectionIndex, long currentTick)
        {
            if (StageLogic.Instance.SectionDatas.TryGetValue(targetSectionIndex, out var sectionData))
            {
                foreach (var section in _canAttackSectionData)
                {
                    section.ClearWaitUnitData(this);
                }
                _canAttackSectionData.Clear();
                _targetSection = sectionData;
                _targetSetTick = currentTick;
            }
        }

    }

}