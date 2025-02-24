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

        void Start()
        {
            startButton.OnClickAsObservable().Subscribe(_ => GameStart());

            startButton.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GameStart()
        {
            Managers.Scene.LoadScene(Define.Scene.GameScene);
        }
    }
}
