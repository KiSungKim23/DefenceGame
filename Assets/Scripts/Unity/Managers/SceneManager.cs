using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Client
{
    public class SceneManagerEx
    {
        public bool moveScene = false;
        public Define.Scene CurrentSceneType { set; get; } = Define.Scene.Unknown;
        public BaseScene CurrentScene { get; private set; }

        public async void LoadScene(Define.Scene sceneType)
        {
            if (moveScene)
                return;
            DOTween.KillAll();

            moveScene = true;
            CurrentSceneType = sceneType;

            bool isRelease = false;
            ResourceReleaseSceneInit.nextEvent.Subscribe(_ => { isRelease = true; });
            SceneManager.LoadScene(GetSceneName(Define.Scene.ResourceRelease));
            await UniTask.WaitWhile(() => !isRelease);
            await UniTask.Delay(100);
            SceneManager.LoadScene(GetSceneName(sceneType));
            moveScene = false;
        }

        public string GetSceneName(Define.Scene _sceneType)
        {
            switch (_sceneType)
            {
                case Define.Scene.GameScene:
                    return "GameScene";
                case Define.Scene.ResourceRelease:
                    return "ResourceReleaseScene";
                case Define.Scene.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_sceneType), _sceneType, null);
            }

            return "";
        }
        public void SetCurrentScene(BaseScene scene)
        {
            CurrentScene = scene;
        }
        public void Clear()
        {
            if (CurrentScene)
                CurrentScene.Clear();
        }
    }
}
