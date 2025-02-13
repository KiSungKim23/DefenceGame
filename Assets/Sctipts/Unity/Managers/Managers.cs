using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class Managers : MonoBehaviour
    {
        static Managers s_instance; // 유일성이 보장된다
        public static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

        ResourceManager resource = new ResourceManager();
        SceneManagerEx scene = new SceneManagerEx();

        public static ResourceManager Resource => Instance.resource;
        public static SceneManagerEx Scene => Instance.scene;
        public static Logic.StageLogic Stage => Logic.StageLogic.Instance;

        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        static void Init()
        {
            if (s_instance == null)
            {
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                s_instance = go.GetComponent<Managers>();

            }
        }
    }
}
