using System.Collections.Generic;

namespace Logic
{

    public interface IDataManager 
    {
            public Dictionary<int , Logic.BuffInfoScript> GetBuffInfoScriptDictionaryAll();
            public BuffInfoScript GetBuffInfoScriptDictionary(int keyData);
            public Dictionary<int , Logic.MonsterInfoScript> GetMonsterInfoScriptDictionaryAll();
            public MonsterInfoScript GetMonsterInfoScriptDictionary(int keyData);
            public Dictionary<Define.SkillType , Logic.SkillInfoScript> GetSkillInfoScriptDictionaryAll();
            public SkillInfoScript GetSkillInfoScriptDictionary(Define.SkillType keyData);
            public List<SkillBuffInfoScript> GetSkillBuffInfoScriptListAll();
            public Dictionary<int , Logic.UnitInfoScript> GetUnitInfoScriptDictionaryAll();
            public UnitInfoScript GetUnitInfoScriptDictionary(int keyData);
            public Dictionary<int , Logic.UnitUnionInfoScript> GetUnitUnionInfoScriptDictionaryAll();
            public UnitUnionInfoScript GetUnitUnionInfoScriptDictionary(int keyData);
    }
}
