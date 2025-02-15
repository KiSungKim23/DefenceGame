using MessagePack;
using System;

namespace Client
{

    public partial class DataManager : Logic.IDataManager
    {
        public void Init()
        {
            LoadBuffInfoScript();
            LoadMonsterInfoScript();
            LoadSkillInfoScript();
            LoadSkillBuffInfoScript();
            LoadUnitInfoScript();
            LoadUnitUnionInfoScript();
        }
    }
}
