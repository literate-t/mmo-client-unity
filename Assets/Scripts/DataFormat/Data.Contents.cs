using System;
using System.Collections.Generic;
using Google.Protobuf.MyProtocol;
using UnityEngine;

namespace Data
{
    #region Skill
    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float coolDown;
        public int damage;
        public SkillType skillType;
        public Projectile projectile;
    }

    public class Projectile
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;
    }

    [Serializable]
    class SkillData : IConvertibleToDictionary<int, Skill>
    {
        public List<Skill> skills = new();

        public Dictionary<int, Skill> ToDictionary()
        {
            Dictionary<int, Skill> dic = new();
            foreach (Skill skill in skills)
            {
                dic.Add(skill.id, skill);
            }

            return dic;
        }
    }
    #endregion

    #region Item
    [Serializable]
    public class ItemData
    {
        public int id;
        public string name;
        public ItemType itemType;
        public string iconPath;
    }

    [Serializable]
    public class WeaponData : ItemData, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public WeaponType weaponTypeEnum;
        public string weaponType;
        public int damage;

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(weaponType))
            {
                if (!Enum.TryParse(weaponType, true, out weaponTypeEnum))
                {
                    throw new FormatException($"Enum parsing error : {weaponType}");
                }
            }
        }

        public void OnBeforeSerialize()
        {

        }
    }

    [Serializable]
    public class ArmorData : ItemData, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public ArmorType armorTypeEnum;
        public string armorType;
        public int defence;

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(armorType))
            {
                if (!Enum.TryParse(armorType, true, out armorTypeEnum))
                {
                    Debug.Log($"Parse error : {armorType}");
                }
            }
        }

        public void OnBeforeSerialize()
        {

        }
    }

    [Serializable]
    public class ConsumableData : ItemData, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public ConsumableType consumableTypeEnum;
        public string consumableType;
        public int maxCount;

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(consumableType))
            {
                if (!Enum.TryParse(consumableType, true, out consumableTypeEnum))
                {
                    Debug.Log($"Parse error : {consumableType}");
                }
            }
        }

        public void OnBeforeSerialize()
        {

        }
    }

    class Item : IConvertibleToDictionary<int, ItemData>
    {
        public List<WeaponData> weapons = new();
        public List<ArmorData> armors = new();
        public List<ConsumableData> consumables = new();

        // 아이템이 필드에 있을 때 어떤 타입인지 알 수 없으므로
        // Dictionary에서 보관할 때는 인터페이스 타입으로 보관한다
        public Dictionary<int, ItemData> ToDictionary()
        {
            Dictionary<int, ItemData> dic = new();
            foreach (WeaponData item in weapons)
            {
                item.itemType = ItemType.Weapon;
                dic.Add(item.id, item);
            }
            foreach (ArmorData item in armors)
            {
                item.itemType = ItemType.Armor;
                dic.Add(item.id, item);
            }
            foreach (ConsumableData item in consumables)
            {
                item.itemType = ItemType.Consumable;
                dic.Add(item.id, item);
            }

            return dic;
        }
    }
    #endregion

    #region Monster
    [Serializable]
    public class MonsterData
    {
        public int id;
        public string name;
        //public StatInfo stat;
        public string prefabPath;
    }

    class MonsterInfo : IConvertibleToDictionary<int, MonsterData>
    {
        public List<MonsterData> monsters = new();

        public Dictionary<int, MonsterData> ToDictionary()
        {
            Dictionary<int, MonsterData> dic = new();
            foreach (MonsterData item in monsters)
            {
                dic.Add(item.id, item);
            }

            return dic;
        }
    }
    #endregion
}