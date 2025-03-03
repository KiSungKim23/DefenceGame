using MessagePack;
using System;

namespace GameVals
{
    [MessagePackObject]
    public class AccountInfo
    {
        [Key(0)] public Int64 AccountID { get; set; }
        [Key(1)] public string NickName { get; set; }
        [Key(2)] public DateTime LogoutTime { get; set; }
        [Key(3)] public Int32 clearCount { get; set; }

        public AccountInfo CopyInstance()
        {
            return new AccountInfo()
            {
                AccountID = this.AccountID,
                NickName = this.NickName,
                LogoutTime = this.LogoutTime,
                clearCount = this.clearCount
            };
        }

        public bool CompareKey(Int64 accountid)
        {
            return this.AccountID == accountid;
        }

        public bool CompareKey(AccountInfo rdata)
        {
            return this.AccountID == rdata.AccountID;
        }
    }

    [MessagePackObject]
    public class MonsterCheckData
    {
        [Key(0)] public Int32 objectID { get; set; }
        [Key(1)] public Int32 nowSectionIndexX { get; set; }
        [Key(2)] public Int32 nowSectionIndexY { get; set; }
        [Key(3)] public Int64 currentHP { get; set; }

        public MonsterCheckData CopyInstance()
        {
            return new MonsterCheckData()
            {
                objectID = this.objectID,
                nowSectionIndexX = this.nowSectionIndexX,
                nowSectionIndexY = this.nowSectionIndexY,
                currentHP = this.currentHP
            };
        }

        public bool CompareKey(Int32 objectid)
        {
            return this.objectID == objectid;
        }

        public bool CompareKey(MonsterCheckData rdata)
        {
            return this.objectID == rdata.objectID;
        }
    }
}
