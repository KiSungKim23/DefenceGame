using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class GamePannal : MonoBehaviour
    {

        public Button unitPannalBtn;
        public Button upgradePannalBtn;
        public Button drawPannalBtn;

        public Image pannalBackground;

        public GameObject unitButtonContent;
        public UnitIcon unitIconButtonObject;

        public UnitOptionButton optionButton;
        public UnitSetButton unitSetButton;

        #region UnitPannal
        private Dictionary<int, UnitIcon> _unitInfos = new Dictionary<int, UnitIcon>();
        #endregion
        #region UpgradePannal
        public Button upgardeBtn1; 
        public Button upgardeBtn2;
        public Button upgardeBtn3;
        #endregion
        #region DrawPannal
        public Button drawHeroBtn;
        #endregion


        void Start()
        {
        }

        void Update()
        {

        }

        public void Init()
        {
            Managers.Stage.UnitCardAdd = CreateUnitButton;

            unitPannalBtn.OnClickAsObservable().Subscribe(_ => ChangePannal(Define.PannalType.Unit));
            upgradePannalBtn.OnClickAsObservable().Subscribe(_ => ChangePannal(Define.PannalType.Upgrade));
            drawPannalBtn.OnClickAsObservable().Subscribe(_ => ChangePannal(Define.PannalType.Draw));

            upgardeBtn1.OnClickAsObservable().Subscribe(_ => UpgradeUnit(1));
            upgardeBtn2.OnClickAsObservable().Subscribe(_ => UpgradeUnit(2));
            upgardeBtn3.OnClickAsObservable().Subscribe(_ => UpgradeUnit(3));

            drawHeroBtn.OnClickAsObservable().Subscribe(_ => DrawHero());

            optionButton.Init();
            unitSetButton.Init();
        }

        void ChangePannal(Define.PannalType pannalType)
        {
            switch (pannalType)
            {
                case Define.PannalType.Unit:
                    SetUnitPannal(true);
                    SetUpgradePannal(false);
                    SetDrawPannal(false);
                    break;
                case Define.PannalType.Upgrade:
                    SetUnitPannal(false);
                    SetUpgradePannal(true);
                    SetDrawPannal(false);
                    break;
                case Define.PannalType.Draw:
                    SetUnitPannal(false);
                    SetUpgradePannal(false);
                    SetDrawPannal(true);
                    break;
            }
        }

        void SetUnitPannal(bool active)
        {
            if(active)
            {
                pannalBackground.color = unitPannalBtn.image.color;
            }

            foreach(var btnObj in _unitInfos)
            {
                btnObj.Value.CheckUnitInfo(active);
            }

        }

        void SetUpgradePannal(bool active)
        {
            if (active)
            {
                pannalBackground.color = upgradePannalBtn.image.color;
            }
            upgardeBtn1.gameObject.SetActive(active);
            upgardeBtn2.gameObject.SetActive(active);
            upgardeBtn3.gameObject.SetActive(active);
        }

        void SetDrawPannal(bool active)
        {
            if (active)
            {
                pannalBackground.color = drawPannalBtn.image.color;
            }
            drawHeroBtn.gameObject.SetActive(active);
        }

        void UpgradeUnit(int upgradeGrade)
        {
            Managers.Stage.UpgradeGrade(upgradeGrade);
        }

        void DrawHero()
        {

        }

        public void CreateUnitButton(Logic.UnitInfoData unitInfo)
        {
            GameObject monsterObject = Managers.Resource.Instantiate(unitIconButtonObject.gameObject, unitButtonContent.transform);

            var unitInfoComponent = monsterObject.GetComponent<UnitIcon>();
            if (unitInfoComponent != null)
            {
                unitInfoComponent.SetUnitInfo(unitInfo);
                _unitInfos.Add(unitInfoComponent.GetUnitUID(), unitInfoComponent);
            }

            foreach (var unit in _unitInfos)
            {
                if (unit.Value.GetUnitCount() > 0)
                    unit.Value.gameObject.SetActive(true);
                else
                    unit.Value.gameObject.SetActive(false);
                
            }
        }

    }
}                                          
