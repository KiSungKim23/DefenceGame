using Best.SignalR;
using Best.SignalR.Encoders;
using Cysharp.Threading.Tasks;
using GameVals;
using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CentralServerClient : MonoBehaviour, SignalRCentralServerAck
{
    HubConnection _hub;

    public HubConnection HubInstance
    {
        get => _hub;
        set => _hub = value;
    }

    private string _path = "/CentralServer";

    public Action _hubConnection;
    public Action _hubClose;
    public Action<string> _hubError;
    public Action<ITransport, TransportEvents> _hubTransportEvent;

    Dictionary<Define.EPGameServer, Subject<Define.Errors>> _ack = new Dictionary<Define.EPGameServer, Subject<Define.Errors>>();

    public static CentralServerClient Server => CentralServerManager.Instance.Server;

    public bool bTryConnectGameServer = false;
    public IObservable<Define.Errors> SubscribeAck(Define.EPGameServer ack)
    {
        Subject<Define.Errors> ret;
        if (!_ack.TryGetValue(ack, out ret))
        {
            ret = new Subject<Define.Errors>();
            _ack.Add(ack, ret);
        }
        return ret.AsObservable();
    }

    public async UniTask<bool> AwaitDelay(Define.EPGameServer ack)
    {
        bool ret = false;
        bool wait = false;
        UniRx.ObservableExtensions.Subscribe(SubscribeAck(ack).Take(1), _1 =>
        {
            ret = _1 == Define.Errors.S_OK;
            wait = true;
        }).AddTo(this);

        await UniTask.WaitUntil(() => wait || bTryConnectGameServer);
        if (bTryConnectGameServer)
            return false;
        return ret;
    }

    public Subject<Define.Errors> OnNextAck(Define.EPGameServer ack)
    {
        Subject<Define.Errors> ret;
        if (false == _ack.TryGetValue(ack, out ret))
        {
            ret = new Subject<Define.Errors>();
            _ack.Add(ack, ret);
        }

        return ret;
    }

    private void Awake()
    {
        MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
        //MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(StandardResolver.Instance);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void OnDestroy()
    {
        if (_hub != null)
        {
            _hub.ReconnectPolicy = null;
            _hub.OnReconnecting -= Hub_Reconnecting;
            _hub.OnConnected -= Hub_OnConnected;
            _hub.OnError -= Hub_OnError;
            _hub.OnClosed -= Hub_OnClosed;
            _hub.OnReconnecting -= Hub_Reconnecting;
            await _hub.CloseAsync();
            _hub = null;
        }
    }
    public void CloseGameServer(bool isUserClose = false)
    {
        if (_hub == null)
            return;
        _hub.ReconnectPolicy = null;
        _hub.OnReconnecting -= Hub_Reconnecting;
        _hub.OnConnected -= Hub_OnConnected;
        _hub.OnError -= Hub_OnError;
        _hub.OnClosed -= Hub_OnClosed;
        _hub.OnReconnecting -= Hub_Reconnecting;
        _hub.StartClose();
    }
    public void ConnectGameServer()
    {
        if (_hub != null && _hub.State == ConnectionStates.Connected)
            return;

        IProtocol protocol = new JsonProtocol(new LitJsonEncoder());
        var serverUri = " https://localhost:7045/CentralServerHub";

        if (serverUri == "")
        {
            return;
        }
        else
        {
            Debug.Log($"게임서버 접속 {serverUri}");
            _hub = new HubConnection(new Uri(serverUri), protocol); //게임서버 연결
        }

        _hub.Options.PingTimeoutInterval = TimeSpan.FromSeconds(300);
        _hub.OnConnected += Hub_OnConnected;
        _hub.OnError += Hub_OnError;
        _hub.OnClosed += Hub_OnClosed;
        _hub.OnReconnecting += Hub_Reconnecting;
        _hub.ReconnectPolicy = new DefaultRetryPolicy();
        this.InitCentralServerCallBack();
        this.CustomInitGameServerCallBack();

        _hub.StartConnect();
    }

    public bool IsConectedSignServer()
    {
        if (_hub == null)
            return false;

        return _hub.State == ConnectionStates.Connected;
    }
    private void Hub_OnConnected(HubConnection hub)
    {
        //연결 성공.
        _hub = hub;
        _hubConnection();
    }

    private void Hub_OnClosed(HubConnection hub)
    {
        _hubClose();
        Debug.Log("Hub_OnClosed");
    }
    private void Hub_OnError(HubConnection hub, string error)
    {
        _hubError(error);
        Debug.Log("Hub_OnError" + error);
    }

    private void Hub_Reconnecting(HubConnection hub, string str)
    {
        if (bTryConnectGameServer)
            return;
        bTryConnectGameServer = true;
        Debug.Log($"게임 서버 재연결 시도......errorReason : {str}");
    }

    public void SignUserAck(Define.Errors error, AccountInfo msgData)
    {

    }

    public void TestAck(Define.Errors error, string msgData, string testmsg, string testmsg2, string testmsg3)
    {
        Debug.Log(msgData + testmsg + testmsg2 + testmsg3);
    }

    public void Test2Ack(Define.Errors error)
    {
        throw new NotImplementedException();
    }
}
