using MessagePack;
using GameVals;

namespace Vals
{
    [MessagePackObject]
    public class PTestReq
    {
       [Key(0)] public string msgData;
       [Key(1)] public string testmsg;
       [Key(2)] public string testmsg2;
       [Key(3)] public string testmsg3;
    }
    [MessagePackObject]
    public class PSignUserReq
    {
       [Key(0)] public Define.AuthType authType;
       [Key(1)] public string userGUID;
    }
    [MessagePackObject]
    public class PTestAck
    {
       [Key(0)] public string msgData;
       [Key(1)] public string testmsg;
       [Key(2)] public string testmsg2;
       [Key(3)] public string testmsg3;
    }
}
