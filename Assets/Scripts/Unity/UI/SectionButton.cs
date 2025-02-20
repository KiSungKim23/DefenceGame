using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class SectionButton : MonoBehaviour
    {
        private Define.SectionButtonType _sectionButtonType;

        private Vector3 _sectionLogicWorldPosition;

        private (int, int) buttonIndex;
        private UnitObject _unitData;

        public GameScene gameScene;

        public Button sectionBtn;
        public UnitOptionButton option;
        public Image buttonImage;

        private bool isActive;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void Init()
        {
            sectionBtn.OnClickAsObservable().Subscribe(_ => Active()).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Active()
        {
            switch(_sectionButtonType)
            {
                case Define.SectionButtonType.Monster:
                    _unitData.GetUnitData().SetTarget(buttonIndex, Managers.Stage.GetCurrentTick());
                    gameScene.ResetTargetBtn();
                    break;
                case Define.SectionButtonType.Unit:
                    if (_unitData != null)
                    {
                        option.SetUnitInfo(_unitData.GetUnitData().GetUnitInfoData(), transform, this);
                        isActive = true;
                        option.gameObject.SetActive(true);
                    }
                    break;
            }
        }

        public void SetSection((int, int) sectionIndex)
        {
            buttonIndex = sectionIndex;

            if (sectionIndex.Item1 == 0 || sectionIndex.Item1 == Define.SectionCount - 1 || sectionIndex.Item2 == 0 || sectionIndex.Item2 == Define.SectionCount - 1)
            {
                _sectionButtonType = Define.SectionButtonType.Monster;
                buttonImage.color = new Color(0, 0, 0, 0.5f);
                isActive = false;
                gameObject.SetActive(false);
            }
            else
            {
                _sectionButtonType = Define.SectionButtonType.Unit;
                isActive = true;
                gameObject.SetActive(true);
            }

            SetSectionButtonPosition(sectionIndex);
            SetSectionPosition(sectionIndex);
        }


        private void SetSectionButtonPosition((int, int) sectionIndex)
        {
            float positionX = (sectionIndex.Item1 * Define.SectionUISize) + Define.SectionUISize + (Define.SectionUISize / 8);
            float positionY = (sectionIndex.Item2 * Define.SectionUISize) + (Define.SectionUISize * (Define.SectionCount / 2) + (Define.SectionUISize / 2)) + (Define.SectionUISize / 2);

            transform.position = new Vector3(positionX, positionY, 0);
        }

        private void SetSectionPosition((int, int) sectionIndex)
        {
            float positionX = (sectionIndex.Item1 * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2));
            float positionY = (sectionIndex.Item2 * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2));

            _sectionLogicWorldPosition = new Vector3(positionX, positionY, 0);
        }

        public Define.SectionButtonType GetSectionButtonType()
        {
            return _sectionButtonType;
        }

        public void SetUnitData(UnitObject unit)
        {
            _unitData = unit;
        }

        public UnitObject GetUnitObject()
        {
            return _unitData;
        }

        public void DeActiveUnit()
        {
            Managers.Stage.unitManager.RemoveActiveUnit(_unitData.GetUnitData());
            ResetUnitData();
        }

        public void ResetUnitData()
        {
            _unitData = null;
        }

        public bool CheckIsActive(float positionX, float positionY, float attackRange)
        {
            return Vector3.Distance(new Vector3(positionX, positionY , 0), _sectionLogicWorldPosition) < attackRange;
        }


        public void SetActive(bool active)
        {
            if (isActive != active)
            {
                isActive = active;
                gameObject.SetActive(active);
            }
        }
    }
}
