using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Logic
{

    public class Unit : Object
    {
        private StageLogic _stageLogic;

        Define.UnitState _state;

        private int _objectIndex;

        public Define.UnitState State { get { return _state; } }

        public Section _targetSection;
        public long _targetSetTick;
        private UnitData _unitData;

        private List<Section> _canAttackSectionData = new List<Section>();
        public Action<long> unitAttack;

        private SkillInfoScript _unitBaseSkillInfo;
        private List<SkillInfoScript> _unitSkillInfos;

        private (int, int) _sectionIndex;

        private UnitInfoScript _unitInfoScript;

        private long _canAttackTick;

        private bool _attackWait;

        public long CanAttackTick { get { return _canAttackTick; } }
        public (int, int) SectionIndex { get { return _sectionIndex; } }


        public Unit(StageLogic stageLogic, int objectIndex, UnitData unitInfo, (int, int) section)
        {
            _stageLogic = stageLogic;
            _targetSetTick = 0;
            _objectIndex = objectIndex;
            _sectionIndex = section;
            _unitData = unitInfo;
            _uid = unitInfo.GetUID();
            SetScript(unitInfo.GetUID());
            SetWorldPosition();
            CheckCanAttackSection();
            _attackWait = false;
        }

        public void Update(long currentTick)
        {
            if (_targetSection != null)
            {
                if (_stageLogic.monsterManager.CheckUnitAttack())
                {
                    if(_attackWait == true)
                    {
                        _canAttackTick = currentTick;
                        _attackWait = false;
                    }

                    if (_canAttackTick <= _targetSetTick)
                    {
                        _canAttackTick = _targetSetTick <= _currentTick ? _targetSetTick : _currentTick;
                    }

                    while (_canAttackTick <= currentTick)
                    {

                        var addSkillInfo = GetActiveSkill();

                        Skill addSkill = new Skill(_stageLogic, GetActiveSkill(), _canAttackTick);
                        _targetSection.AddSkill(addSkill);

                        if (addSkillInfo.skillRange > 1)
                        {
                            var nextSection = _targetSection.GetNextSection();
                            var previousSection = _targetSection.GetPreviousSection();
                            for (int i = 1; i < addSkillInfo.skillRange; i++)
                            {
                                Skill addSkillNextSection = new Skill(_stageLogic, GetActiveSkill(), _canAttackTick, i);
                                nextSection.AddSkill(addSkillNextSection);
                                Skill addSkillPreviousSection = new Skill(_stageLogic, GetActiveSkill(), _canAttackTick, i);
                                previousSection.AddSkill(addSkillPreviousSection);

                                nextSection = nextSection.GetNextSection();
                                previousSection = previousSection.GetPreviousSection();
                            }
                        }
                        unitAttack.Invoke(_canAttackTick);
                        _canAttackTick += (long)(_unitInfoScript.attackSpeed * Define.OneSecondTick);
                    }

                    _currentTick = currentTick;
                }
                else
                {
                    _attackWait = true;
                }
            }
        }

        public void Clear()
        {
            _unitData.DeActive();

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
            _canAttackTick = createTick + (long)(_unitInfoScript.attackSpeed * Define.OneSecondTick);
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

            foreach (var sectionInfo in _stageLogic.sectionManager.SectionDatas)
            {
                if (Vector3.Distance(_position, sectionInfo.Value.GetSectionWorldPosition()) < _unitInfoScript.attackRange)
                {
                    _canAttackSectionData.Add(sectionInfo.Value);
                    sectionInfo.Value.AddAttackWaitUnit(this);
                }
            }
        }

        public void UnitAttack(long tick, Section sectionData)
        {
            if (_stageLogic.monsterManager.CheckUnitAttack() && _canAttackTick <= tick)
            {
                var addSkillInfo = GetActiveSkill();

                Skill addSkill = new Skill(_stageLogic,GetActiveSkill(), tick);
                sectionData.AddSkill(addSkill);

                if (addSkillInfo.skillRange > 1)
                {
                    var nextSection = sectionData.GetNextSection();
                    var previousSection = sectionData.GetPreviousSection();
                    for (int i = 1; i < addSkillInfo.skillRange; i++)
                    {
                        Skill addSkillNextSection = new Skill(_stageLogic,GetActiveSkill(), tick, i);
                        nextSection.AddSkill(addSkillNextSection);
                        Skill addSkillPreviousSection = new Skill(_stageLogic, GetActiveSkill(), tick, i);
                        previousSection.AddSkill(addSkillPreviousSection);

                        nextSection = nextSection.GetNextSection();
                        previousSection = previousSection.GetPreviousSection();
                    }
                }

                unitAttack.Invoke(tick);
                _canAttackTick = tick + (long)(_unitInfoScript.attackSpeed * Define.OneSecondTick);
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

        private void SetScript(int uid)
        {
            _unitInfoScript = _stageLogic.dataManager.GetUnitInfoScriptDictionary(uid);

            _unitBaseSkillInfo = _stageLogic.dataManager.GetSkillInfoScriptDictionary(_unitInfoScript.baseSkillID);
            _unitSkillInfos = new List<SkillInfoScript>();
            if (_unitInfoScript.skill1ID != 0)
                _unitSkillInfos.Add(_stageLogic.dataManager.GetSkillInfoScriptDictionary(_unitInfoScript.skill1ID));
            if (_unitInfoScript.skill2ID != 0)
                _unitSkillInfos.Add(_stageLogic.dataManager.GetSkillInfoScriptDictionary(_unitInfoScript.skill2ID));
            if (_unitInfoScript.skill3ID != 0)
                _unitSkillInfos.Add(_stageLogic.dataManager.GetSkillInfoScriptDictionary(_unitInfoScript.skill3ID));

        }

        public UnitData GetUnitInfoData()
        {
            return _unitData;
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
            if (_targetSection != null)
            {
                CheckCanAttackSection();
                _targetSection = null;
            }
        }

        public bool SetTarget((int, int) targetSectionIndex, long currentTick)
        {
            if (_stageLogic.sectionManager.SectionDatas.TryGetValue(targetSectionIndex, out var sectionData))
            {
                foreach (var section in _canAttackSectionData)
                {
                    section.ClearWaitUnitData(this);
                }
                _canAttackSectionData.Clear();
                _targetSection = sectionData;
                _targetSetTick = currentTick;
                return true;
            }
            else
            {
                return false;
            }
        }

        public SkillInfoScript GetActiveSkill()
        {
            if(_unitSkillInfos.Count == 0)
            {
                return _unitBaseSkillInfo;
            }

            long probability = _stageLogic.RandomValue;
            long nowProbability = 0;

            foreach(var skillInfo in _unitSkillInfos)
            {
                nowProbability += (long)(skillInfo.probability * Define.MaxRandomValue);
                if (probability < nowProbability)
                    return skillInfo;
            }

            return _unitBaseSkillInfo;
        }

    }

}