using System.Collections.Generic;
using Data;
using UnityEngine;

public interface IConvertibleToDictionary<Key, Value>
{
    public Dictionary<Key, Value> ToDictionary();
}

public class DataManager
{
    internal Dictionary<int, Skill> DictSkill { get; private set; }
    internal Dictionary<int, ItemData> DictItem { get; private set; }
    internal Dictionary<int, MonsterData> DictMonster { get; private set; }

    public void Init()
    {
        DictSkill = LoadJson<SkillData, int, Skill>("SkillData").ToDictionary();
        DictItem = LoadJson<Data.Item, int, ItemData>("ItemData").ToDictionary();
        DictMonster = LoadJson<MonsterInfo, int, MonsterData>("MonsterData").ToDictionary();
    }

    public ItemData GetItemData(int dataSheetId)
    {
        ItemData itemData = null;
        DictItem.TryGetValue(dataSheetId, out itemData);

        return itemData;
    }

    public Skill GetSkillData(int dataSheetId)
    {
        Skill skillData = null;
        DictSkill.TryGetValue(dataSheetId, out skillData);

        return skillData;
    }

    public MonsterData GetMonsterData(int dataSheetId)
    {
        MonsterData monsterData = null;
        DictMonster.TryGetValue(dataSheetId, out monsterData);

        return monsterData;
    }

    DataType LoadJson<DataType, Key, Value>(string path) where DataType : IConvertibleToDictionary<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<DataType>(textAsset.text);
    }
}
