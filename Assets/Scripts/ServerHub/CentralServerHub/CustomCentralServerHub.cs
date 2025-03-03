using GameVals;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial interface SignalRCentralServerAck
{
    void SignUserAck(Define.Errors error, AccountInfo accountInfo);
}

public static class CustomCentralServerHub
{
    public static void CustomInitGameServerCallBack(this SignalRCentralServerAck hubClient)
    {
        hubClient.HubInstance.On<Define.Errors, string>("SignUserAck", (error, packet) =>
        {
            PSignUserAck result = null;
            try
            {
                Debug.Log(MessagePackSerializer.DefaultOptions.Resolver.GetType().FullName);
                var conv = Convert.FromBase64String(packet);
                if (conv == null)
                {
                    return;
                }
                result = MessagePackSerializer.Deserialize<PSignUserAck>(conv);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
            hubClient.SignUserAck(error, result.accountInfo);
        });

    }

}

[MessagePackObject]
public class PSignUserAck
{
    [Key(0)] public AccountInfo accountInfo;
}