using Best.SignalR;
using GameVals;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vals;


public partial interface SignalRCentralServerAck
{
    HubConnection HubInstance { get; set; }

    void TestAck(Define.Errors error, string msgData, string testmsg, string testmsg2, string testmsg3);

}


public static class AutoCentralServerHub
{
    public static void InitCentralServerCallBack(this SignalRCentralServerAck hubClient)
    {
        hubClient.HubInstance.On<Define.Errors, string>("TestAck", (error, packet) =>
        {
            PTestAck result = null;
            try
            {
                Debug.Log(MessagePackSerializer.DefaultOptions.Resolver.GetType().FullName);
                var conv = Convert.FromBase64String(packet);
                if (conv == null)
                {
                    return;
                }
                result = MessagePackSerializer.Deserialize<PTestAck>(conv);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
            hubClient.TestAck(error, result.msgData, result.testmsg, result.testmsg2, result.testmsg3);
        });

    }

    public static void TestReq(this SignalRCentralServerAck hubClient, string msgData, string testmsg, string testmsg2, string testmsg3)
        {
        PTestReq packetObj = new PTestReq();
        packetObj.msgData = msgData;
        packetObj.testmsg = testmsg;
        packetObj.testmsg2 = testmsg2;
        packetObj.testmsg3 = testmsg3;
        string msg = string.Empty;
        try
        {
            var conv = MessagePackSerializer.Serialize(packetObj);
            if (conv == null)
                return;
            msg = Convert.ToBase64String(conv);
        }
        catch (Exception e)
        {
            return;
        }
        hubClient.HubInstance.SendAsync("TestReq", msg);
    }

    public static void SignUserReq(this SignalRCentralServerAck hubClient, Define.AuthType authType, string userGUID)
        {
        PSignUserReq packetObj = new PSignUserReq();
        packetObj.authType = authType;
        packetObj.userGUID = userGUID;
        string msg = string.Empty;
        try
        {
            var conv = MessagePackSerializer.Serialize(packetObj);
            if (conv == null)
                return;
            msg = Convert.ToBase64String(conv);
        }
        catch (Exception e)
        {
            return;
        }
        hubClient.HubInstance.SendAsync("SignUserReq", msg);
    }



}
