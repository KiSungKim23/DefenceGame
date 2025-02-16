using System.Collections.Generic;

namespace Logic
{

    public interface IDataManager 
    {
            public Dictionary<int , Logic.BuffInfoScript> GetBuffInfoScriptDictionaryAll();
            public BuffInfoScript GetBuffInfoScriptDictionary(int keyData);
            public Dictionary<int , Logic.MonsterInfoScript> GetMonsterInfoScriptDictionaryAll();
            public MonsterInfoScript GetMonsterInfoScriptDictionary(int keyData);
            public Dictionary<int , Logic.SkillInfoScript> GetSkillInfoScriptDictionaryAll();
            public SkillInfoScript GetSkillInfoScriptDictionary(int keyData);
            public List<SkillBuffInfoScript> GetSkillBuffInfoScriptListAll();
            public Dictionary<int , Logic.UnitInfoScript> GetUnitInfoScriptDictionaryAll();
            public UnitInfoScript GetUnitInfoScriptDictionary(int keyData);
            public List<UnitUnionInfoScript> GetUnitUnionInfoScriptListAll();
    }
}
