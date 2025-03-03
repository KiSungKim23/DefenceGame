using Best.HTTP.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Collections;
using UnityEngine;

public class CentralServerManager : Singleton<CentralServerManager>
{
    [ReadOnly]
    public bool isConnect = false;

    public bool bTryReLogin = false;

    CentralServerClient connectServer;
    public CentralServerClient Server => connectServer;

    public Subject<bool> OnReConnectComplete = new Subject<bool>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        StartConnect();
        int a = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartConnect()
    {
        HTTPManager.ResetSetup();
        HTTPManager.Setup();

        // #if DEV_BUILD
        //         HTTPManager.Logger.Level = Loglevels.All;
        // #endif
        if (connectServer == null)
        {
            connectServer = gameObject.AddComponent<CentralServerClient>();

            //연결 시작
            connectServer._hubConnection = Hub_OnConnected;
            connectServer._hubClose = Hub_OnClosed;
            connectServer._hubError = Hub_OnError;
        }

        connectServer.ConnectGameServer();
    }
    public void StartClose(bool isUserClose = false)
    {
        if (connectServer)
        {
            connectServer.CloseGameServer();
        }

        HTTPManager.OnQuit();
        Destroy(connectServer);
        connectServer = null;
        isConnect = false;
    }

    private void Destroy()
    {
        HTTPManager.OnQuit();
        // connectServer.CloseGameServer();
    }
    private void Hub_OnConnected()
    {
        isConnect = true;

        if (Instance.Server.bTryConnectGameServer)
        {
            TryReLogin();
            Instance.Server.bTryConnectGameServer = false;
        }
    }
    private void Hub_OnClosed()
    {
        Debug.Log("Hub_OnClosed");
        isConnect = false;
        //연결 해제완료
    }
    private void Hub_OnError(string error)
    {
        Debug.Log("Hub_OnError");
        isConnect = false;
        //에러 스트링
    }

    public void Login()
    {
        var userID = PlayerPrefs.GetString("USER_ID", "");

        if (userID == "")
        {
            userID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("USER_ID", userID);
        }

        Server.SignUserReq(Define.AuthType.Guest, userID);
    }
    public async void TryReLogin()
    {
        if (isConnect && bTryReLogin)
            return;

        Instance.Login();
        bTryReLogin = true;
        var bResult = await Server.AwaitDelay(Define.EPGameServer.SignUserAck);
        bTryReLogin = false;

        if (bResult)
        {
            Debug.Log("재접속 성공");

            OnReConnectComplete.OnNext(true);
        }
        else
        {
            Debug.Log("재접속 실패");
            OnReConnectComplete.OnNext(false);
        }
    }
    public bool IsConnectGameServer()
    {
        if (connectServer == null)
            return false;
        if (connectServer.bTryConnectGameServer)
            return false;
        if (bTryReLogin)
            return false;
        return true;
    }
}
