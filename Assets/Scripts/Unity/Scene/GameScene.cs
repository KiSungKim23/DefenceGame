using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class GameScene : BaseScene
    {
        public GamePannal gamePannal;

        public GameObject sectionBase;
        public GameObject unitBase;
        public GameObject monsterBase;
        public GameObject sectionButtonBase;
        public GameObject LoadingImage;

        public SectionButton sectionButtonOrigin;

        public long updateTick = 0;
        public long currentTick = 0;
        public float updateTime = 0f;
        public float setNowTimeTemp = 0f;

        private float setNowTime = 0f;

        public bool updateSetTick = true;

        public bool setTime = false;

        public TextMeshProUGUI stageLevelText;
        public TextMeshProUGUI monsterCountText;

        private int _stageLevel;
        private int _monsterCount;

        private Dictionary<int, MonsterObject> _monsters = new Dictionary<int, MonsterObject>();
        private List<UnitObject> _activeUnits = new List<UnitObject>();

        #region SectionButton
        private Dictionary<(int, int), SectionButton> _sectionUnitButtons = new Dictionary<(int, int), SectionButton>();
        private Dictionary<(int, int), SectionButton> _sectionMonsterButtons = new Dictionary<(int, int), SectionButton>();
        #endregion

        private Dictionary<(int, int), SectionObject> _sections = new Dictionary<(int, int), SectionObject>();


        // Start is called before the first frame update
        void Start()
        {
            //Stage.Init(DateTime.UtcNow.Ticks);
            Managers.Stage.monsterManager.MonsterCreated = CreateMonster;
            Managers.Stage.unitManager.ActiveUnitCreated = CreateActiveUnit;
            Managers.Stage.unitManager.ActiveUnitRemoved = RemoveActiveUnit;

            CreateSectionData();
            gamePannal.Init();
            LoadingImage.SetActive(false);
            _stageLevel = 0;
            _monsterCount = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (setTime)
            {
                setNowTime = setNowTimeTemp;
                setTime = false;
            }

            if(_stageLevel != Managers.Stage.StageLevel)
            {
                _stageLevel = Managers.Stage.StageLevel;
                stageLevelText.SetText("Stage : " + _stageLevel.ToString());
            }

            if(_monsterCount != Managers.Stage.monsterManager.ActiveMonsterCount)
            {
                _monsterCount = Managers.Stage.monsterManager.ActiveMonsterCount;
                monsterCountText.SetText(_monsterCount.ToString() + " / " + Define.MonsterMaxCount.ToString());
            }

            if (updateSetTick == false)
            {
                long tick = DateTime.UtcNow.Ticks;

                updateTick += tick - currentTick;

                Managers.Stage.Update(updateTick);

                currentTick = tick;
            }
            else
            {
                if (updateTime != setNowTime)
                {
                    updateTick = (long)(setNowTime * Define.OneSecondTick);
                    Managers.Stage.Update(updateTick);
                    updateTime = setNowTime;
                }
            }
        }

        public void CreateMonster(Logic.Monster monster)
        {
            if (_monsters.TryGetValue(monster.GetObjectID(), out var temp))
            {
                temp.SetMonsterData(monster);
                temp.gameObject.SetActive(true);
            }
            else
            {
                GameObject monsterObject = Managers.Resource.Instantiate("GameObject/Monster", monsterBase.transform);

                var monsterComponent = monsterObject.GetComponent<MonsterObject>();
                if (monsterComponent != null)
                {
                    monsterComponent.SetMonsterData(monster); 
                    _monsters.Add(monster.GetObjectID(), monsterComponent);
                }
            }
        }

        public void CreateActiveUnit(Logic.Unit unit)
        {
            GameObject unitObject = Managers.Resource.Instantiate("GameObject/Unit", unitBase.transform);

            var unitComponent = unitObject.GetComponent<UnitObject>();
            if (unitComponent != null)
            {
                if (_sectionUnitButtons.TryGetValue(unit.GetSectionIndex(), out var optionButton))
                {
                    optionButton.SetUnitData(unitComponent);
                }
                else
                {
                    Destroy(unitObject);
                    Debug.LogError("생성실패, 섹션 버튼 정보 없음");
                    return;
                }

                unitComponent.SetUnitData(unit);
                _activeUnits.Add(unitComponent);
            }
            else
            {
                Destroy(unitObject);
                Debug.LogError("생성실패, 옵젝 복사 안됨");
                return;
            }
        }

        public void RemoveActiveUnit(Logic.Unit unit)
        {
            var unitObject = _activeUnits.Find(_ =>_.GetUnitObjectIndex() == unit.GetObjectIndex());
            _activeUnits.Remove(unitObject);
            Destroy(unitObject.gameObject);
        }

        private void CreateSectionData()
        {
            if(_sections.Count == 0)
            {
                for (int i = 0; i < Define.SectionCount; i++)
                {
                    for (int j = 0; j < Define.SectionCount; j++)
                    {
                        if (i == 0 || i == Define.SectionCount - 1 || j == 0 || j == Define.SectionCount - 1)
                        {
                            CreateMonsterSectionObject(i, j);
                        }
                        CreateSectionButton(i, j);
                    }
                }
            }
        }

        private void CreateMonsterSectionObject(int i, int j)
        {
            GameObject gameObject = Managers.Resource.Instantiate("GameObject/Section", sectionBase.transform);

            var sectionComponent = gameObject.GetComponent<SectionObject>();
            if (sectionComponent != null)
            {
                if (Managers.Stage.sectionManager.SectionDatas.TryGetValue((i, j), out var section))
                {
                    sectionComponent.SetSection(section);
                    _sections.Add((i, j), sectionComponent);
                }
            }
        }

        private void CreateSectionButton(int i, int j)
        {
            GameObject gameObject = Managers.Resource.Instantiate(sectionButtonOrigin.gameObject, sectionButtonBase.transform);

            var sectionButtonComponent = gameObject.GetComponent<SectionButton>();
            if (sectionButtonComponent != null)
            {
                sectionButtonComponent.Init();
                sectionButtonComponent.SetSection((i, j));
                switch(sectionButtonComponent.GetSectionButtonType())
                {
                    case Define.SectionButtonType.Monster:
                        _sectionMonsterButtons.Add((i, j), sectionButtonComponent);
                        break;
                    case Define.SectionButtonType.Unit:
                        _sectionUnitButtons.Add((i, j), sectionButtonComponent);
                        break;
                }
            }
        }

        public void SetUnitDataInUnitButton((int, int) sectionIndex, UnitObject unit)
        {
            if (_sectionUnitButtons.TryGetValue(sectionIndex, out var optionButton))
            {
                optionButton.SetUnitData(unit);
            }
        }

        public void TargetSetOn(List<Logic.Section> sectionLists, UnitObject unit)
        {
            foreach(var sectionData in sectionLists)
            {
                if(_sectionMonsterButtons.TryGetValue(sectionData.GetSectionIndex(), out var targetButton))
                {
                    targetButton.SetUnitData(unit);
                    targetButton.gameObject.SetActive(true);
                }
            }
        }

        public void ResetTargetBtn()
        {
            foreach(var sectionbtn in _sectionMonsterButtons)
            {
                sectionbtn.Value.ResetUnitData();
                sectionbtn.Value.gameObject.SetActive(false);
            }
        }

        public void CheckSectionTarget(float positionX, float positionY, float attackRange)
        {
            List<SectionButton> activeSectionButton = new List<SectionButton>();

            foreach (var monsterSection in _sectionMonsterButtons)
            {
                bool check = monsterSection.Value.CheckIsActive(positionX, positionY, attackRange);
                monsterSection.Value.SetActive(check);
            }
        }

    }
}