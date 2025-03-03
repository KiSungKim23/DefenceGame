using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Define
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Errors
    {
        S_OK = 0,
        E_LogicError = 1,
        E_CheckConnectService_CheckSignIn = 2,
        E_LoginUser_Process = 3,
        E_LoginUser_SelectSignData = 4,
        E_LoginUser_ServerConnection = 5,
        E_AddUnionData_NoneUnitData = 6,
        E_AddUnionData_NoneUnionData = 7,
        E_AddDeactiveUnit_AlreadyDeactiveUnitData = 8,
        E_AddDeactiveUnit_NoneActive = 9,
        E_SetTarget_NoneActiveUnit = 10,
        E_SetTarget_NoneSectionData = 11,
        E_SetUnit_NontUnitData = 12,
        E_SetUnit_NontActiveUnit = 13,
        E_MoveUnit_WaitMove = 14,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EPGameServer
    {
        TestAck,
        SignUserAck,
    }
}
