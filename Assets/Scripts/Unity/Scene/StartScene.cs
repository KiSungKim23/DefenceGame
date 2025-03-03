using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class StartScene : MonoBehaviour
    {
        // Start is called before the first frame update
        public Button startButton;

        private void Awake()
        {
            CentralServerManager.Instance.Init();
        }

        void Start()
        {
            startButton.OnClickAsObservable().Subscribe(_ => GameStart());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GameStart()
        {
            CentralServerManager.Instance.Login();
        }
    }
}
