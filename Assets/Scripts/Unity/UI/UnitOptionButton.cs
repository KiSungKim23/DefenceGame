using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class UnitOptionButton : MonoBehaviour
    {
        private int _unitCount;

        private Logic.UnitData _unitData;
        private SectionButton _activeUnitButton;

        public GameScene gameScene;

        public Button optionDeactiveBtn;
        public Button UnitUnion1Btn;
        public Button UnitUnion2Btn;
        public Button UnitUnion3Btn;

        public Button TargetSetButton;
        public Button DeActiveButton;

        public GameObject UnitSetButtonPosition;
        public UnitSetButton UnitSetting;

        private int _union1UID;
        private int _union2UID;
        private int _union3UID;
        // Start is called before the first frame update
        void Start()
        {
        }

        public void Init()
        {
            _unitCount = 0;
            optionDeactiveBtn.OnClickAsObservable().Subscribe(_ => DeActiveOption()).AddTo(this);
            UnitUnion1Btn.OnClickAsObservable().Subscribe(_ => UnionUnit(_union1UID)).AddTo(this);
            UnitUnion2Btn.OnClickAsObservable().Subscribe(_ => UnionUnit(_union2UID)).AddTo(this);
            UnitUnion3Btn.OnClickAsObservable().Subscribe(_ => UnionUnit(_union3UID)).AddTo(this);

            TargetSetButton.OnClickAsObservable().Subscribe(_ => SetTarget()).AddTo(this);
            DeActiveButton.OnClickAsObservable().Subscribe(_ => DeActive()).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {
            if(_unitCount != _unitData.GetCount())
            {
                _unitCount = _unitData.GetCount();
                UnitSetting.gameObject.SetActive(_unitCount != 0);
            }

        }

        void UnionUnit(int unionUID)
        {
            if (_unitData.CheckCanUnion(unionUID))
            {
                Managers.Stage.AddUnionData(Managers.Stage.GetCurrentTick() + (Define.OneSecondTick / 100), _unitData.GetUnitUnionData(unionUID));
            }
            UnitSetting.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        void DeActiveOption()
        {
            UnitSetting.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        void SetTarget()
        {
            var sectionLists = _activeUnitButton.GetUnitObject().GetUnitData().GetCanAttackSections();

            if(sectionLists.Count == 0)
            {
                _activeUnitButton.GetUnitObject().GetUnitData().RemoveTarget();
            }
            else
            {
                gameScene.TargetSetOn(_activeUnitButton.GetUnitObject().GetUnitData().GetCanAttackSections(), _activeUnitButton.GetUnitObject());
            }
            UnitSetting.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        void DeActive()
        {
            if(_activeUnitButton != null)
            {
                _activeUnitButton.DeActiveUnit();
            }

            UnitSetting.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void SetUnitInfo(Logic.UnitData data, Transform iconPosition, SectionButton activeUnitButton = null)
        {
            _unitData = data;
            _activeUnitButton = activeUnitButton;
            transform.position = iconPosition.position;
            UnitSetting.gameObject.transform.position = UnitSetButtonPosition.transform.position;
            UnitSetting.SetData(this, _unitData, activeUnitButton);
            _unitCount = _unitData.GetCount();
            UnitSetting.gameObject.SetActive(_unitCount != 0);
            SetUnionData(data);
            TargetSetButton.gameObject.SetActive(activeUnitButton != null);
            DeActiveButton.gameObject.SetActive(activeUnitButton != null);
        }

        public void SetUnionData(Logic.UnitData data)
        {
            UnitUnion1Btn.gameObject.SetActive(false);
            UnitUnion2Btn.gameObject.SetActive(false);
            UnitUnion3Btn.gameObject.SetActive(false);

            _union1UID = 0;
            _union2UID = 0;
            _union3UID = 0;

            int index = 0;

            foreach (var unionInfo in data.GetUnitUnionDatas())
            {
                switch (index)
                {
                    case 0:
                        _union1UID = unionInfo.Key;
                        UnitUnion1Btn.gameObject.SetActive(true);
                        break;
                    case 1:
                        _union2UID = unionInfo.Key;
                        UnitUnion1Btn.gameObject.SetActive(true);
                        break;
                    case 2:
                        _union3UID = unionInfo.Key;
                        UnitUnion1Btn.gameObject.SetActive(true);
                        break;
                }
                index++;
            }
        }
    }
}
