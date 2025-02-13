using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ResourceReleaseSceneInit : MonoBehaviour
{
    public static Subject<Unit> nextEvent = new Subject<Unit>();

    void Start()
    {
        Release();
    }

    private async void Release()
    {
        await UniTask.DelayFrame(1);
        await Resources.UnloadUnusedAssets();
        await UniTask.DelayFrame(1);
        nextEvent.OnNext(Unit.Default);
    }
}
