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

        private Logic.UnitInfoData _unitData;
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
        // Start is called before the first frame update
        void Start()
        {
        }

        public void Init()
        {
            _unitCount = 0;
            optionDeactiveBtn.OnClickAsObservable().Subscribe(_ => DeActiveOption()).AddTo(this);
            UnitUnion1Btn.OnClickAsObservable().Subscribe(_ => UnionUnit(1)).AddTo(this);
            UnitUnion2Btn.OnClickAsObservable().Subscribe(_ => UnionUnit(2)).AddTo(this);
            UnitUnion3Btn.OnClickAsObservable().Subscribe(_ => UnionUnit(3)).AddTo(this);

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

        void UnionUnit(int index)
        {
            if (_unitData.CheckCanUnion(index))
            {
                Managers.Stage.AddUnionData(Managers.Stage.GetCurrentTick() + (Define.OneSecondTick / 100), _unitData, index);
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

        public void SetUnitInfo(Logic.UnitInfoData data, Transform iconPosition, SectionButton activeUnitButton = null)
        {
            _unitData = data;
            _activeUnitButton = activeUnitButton;
            transform.position = iconPosition.position;
            UnitSetting.gameObject.transform.position = UnitSetButtonPosition.transform.position;
            UnitSetting.SetData(this, _unitData, activeUnitButton);
            _unitCount = _unitData.GetCount();
            UnitSetting.gameObject.SetActive(_unitCount != 0);

            UnitUnion1Btn.gameObject.SetActive(data.CheckUnionButtonActive(1));
            UnitUnion2Btn.gameObject.SetActive(data.CheckUnionButtonActive(2));
            UnitUnion3Btn.gameObject.SetActive(data.CheckUnionButtonActive(3));

            TargetSetButton.gameObject.SetActive(activeUnitButton != null);
            DeActiveButton.gameObject.SetActive(activeUnitButton != null);
        }
    }
}
