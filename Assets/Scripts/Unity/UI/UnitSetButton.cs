using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class UnitSetButton : MonoBehaviour
    {
        public GameScene gameScene;

        private UnitOptionButton _parent;
        public Button UnitSetBtn;
        public Canvas canvas;
        public RectTransform rectTransform;
        private bool isHolding = false;

        private Logic.UnitData _unitInfo;
        private SectionButton _activeUnitButton;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void Init()
        {
            UnitSetBtn.OnPointerDownAsObservable()
                .Subscribe(_ =>
                {
                    _parent.gameObject.SetActive(false);
                    isHolding = true;
                }).AddTo(this);

            UnitSetBtn.OnPointerUpAsObservable()
                .Subscribe(_ =>
                {
                    isHolding = false;
                    DeActiveOption();
                }).AddTo(this);
        }

        public void SetData(UnitOptionButton parent, Logic.UnitData unitInfo, SectionButton activeUnitData = null)
        {
            _parent = parent;
            _unitInfo = unitInfo;
            _activeUnitButton = activeUnitData;
        }

        // Update is called once per frame
        void Update()
        {
            if(isHolding == true)
            {
#if UNITY_EDITOR
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                    Input.mousePosition, canvas.worldCamera, out localPoint);

                rectTransform.anchoredPosition = localPoint; // UI 좌표로 이동

                int sectionX = (int)((transform.position.x - (Define.SectionUISize / 2)) / Define.SectionUISize);
                int sectionY = (int)((transform.position.y - (Define.SectionUISize * (Define.SectionCount / 2)) - (Define.SectionUISize / 2)) / Define.SectionUISize);

                if ((sectionX >= 1 || sectionX <= Define.SectionCount - 1) && (sectionY >= 1 && sectionY <= Define.SectionCount - 1))
                {
                    gameScene.CheckSectionTarget((float)(sectionX * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2))
                        , (float)(sectionY * Define.SectionSize) - (Define.SectionSize * (Define.SectionCount / 2) - (Define.SectionSize / 2))
                        , Managers.Data.GetUnitInfoScriptDictionary(_unitInfo.GetUID()).attackRange);
                }
                else
                {
                    gameScene.ResetTargetBtn();
                }

#else
                if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0f;
            transform.position = touchPosition;
        }
#endif
            }
        }

        void DeActiveOption()
        {
            var sectionIndex = GetUnitSectionIndex();

            if ((sectionIndex.Item1 >= 1 || sectionIndex.Item1 <= Define.SectionCount - 1) && (sectionIndex.Item2 >= 1 && sectionIndex.Item2 <= Define.SectionCount - 1))
            {

                if (_activeUnitButton == null)
                {
                    if (Managers.Stage.unitManager.SetUnit(_unitInfo, GetUnitSectionIndex(), Managers.Stage.GetCurrentTick()) == false)
                    {
                        Debug.LogError("이미 유닛 있음 나중에 팝업 띄우는 식으로 ㄱ");
                    }
                }
                else
                {
                    var data = _activeUnitButton.GetUnitObject();
                    _activeUnitButton.ResetUnitData();
                    Managers.Stage.unitManager.MoveUnit(data.GetUnitData(), GetUnitSectionIndex());
                    gameScene.SetUnitDataInUnitButton(GetUnitSectionIndex(), data);
                    data.MoveUnitPosition();
                }
                gameScene.ResetTargetBtn();
            }
            gameObject.SetActive(false);
        }

        public (int, int) GetUnitSectionIndex()
        {
            int sectionX = (int)((transform.position.x - (Define.SectionUISize / 2)) / Define.SectionUISize);
            int sectionY = (int)((transform.position.y - (Define.SectionUISize * (Define.SectionCount / 2)) - (Define.SectionUISize / 2)) / Define.SectionUISize);

            return (sectionX, sectionY);
        }
    }
}