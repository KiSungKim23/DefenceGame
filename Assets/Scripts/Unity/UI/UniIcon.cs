using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class UnitIcon : MonoBehaviour
    {
        private Logic.UnitData _unitData;

        public Button optionActiveBtn;
        public TextMeshProUGUI countText;
        public TextMeshProUGUI activeCountText;

        public UnitOptionButton option;

        int _count = 0;
        int _activeCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            optionActiveBtn.OnClickAsObservable().Subscribe(_ => ActiveOption()).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {
            int count = _unitData.GetCount();
            int activeCount = _unitData.GetActiveCount();

            if (count != _count)
            {
                _count = count;
                countText.text = "x" + count.ToString();
            }
            if (activeCount != _activeCount)
            {
                _activeCount = activeCount;
                activeCountText.text = "x" + activeCount.ToString();
            }
        }

        public void SetUnitInfo(Logic.UnitData data)
        {
            _unitData = data;

            countText.text = "x" + _unitData.GetCount().ToString();
            activeCountText.text = "x" + _unitData.GetActiveCount().ToString();
        }

        void ActiveOption()
        {
            option.SetUnitInfo(_unitData, transform);
            option.gameObject.SetActive(true);
        }

        public int GetUnitUID()
        {
            return _unitData.GetUID();
        }

        public void CheckUnitInfo(bool activeInit)
        {
            bool active = false;

            if (_unitData.GetCount() > 0) active = activeInit;

            gameObject.SetActive(active);
        }

        public int GetUnitCount()
        {
            return _unitData.GetCount();
        }


    }
}