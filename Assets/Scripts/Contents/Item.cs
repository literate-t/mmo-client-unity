using Data;
using Google.Protobuf.MyProtocol;

public class Item
{
    public ItemInfo Info { get; } = new();
    public int ItemDbId
    {
        get => Info.ItemDbId;
        set => Info.ItemDbId = value;
    }
    public int DataSheetId
    {
        get => Info.DataSheetId;
        set => Info.DataSheetId = value;
    }
    public int Count
    {
        get => Info.Count;
        set => Info.Count = value;
    }

    public int Slot
    {
        get => Info.Slot;
        set => Info.Slot = value;
    }

    public bool Equipped
    {
        get => Info.Equipped;
        set => Info.Equipped = value;
    }

    public ItemType ItemType { get; private set; }
    public bool Stackable { get; protected set; }

    internal Item(ItemType type)
    {
        ItemType = type;
    }

    public static Item MakeItem(ItemInfo itemInfo)
    {
        Item item = null;
        ItemData itemData = null;
        Managers.Data.DictItem.TryGetValue(itemInfo.DataSheetId, out itemData);
        if (itemData == null)
            return null;

        switch (itemData.itemType)
        {
            case ItemType.Weapon:
                item = new Weapon(itemInfo.DataSheetId);
                break;
            case ItemType.Armor:
                item = new Armor(itemInfo.DataSheetId);
                break;
            case ItemType.Consumable:
                item = new Consumable(itemInfo.DataSheetId);
                break;
        }

        item.ItemDbId = itemInfo.ItemDbId;
        item.Count = itemInfo.Count;
        item.Slot = itemInfo.Slot;
        item.Equipped = itemInfo.Equipped;

        return item;
    }
}

public class Weapon : Item
{
    public WeaponType WeaponType { get; private set; }
    public int Damage { get; private set; }
    public Weapon(int dataSheetId) : base(ItemType.Weapon)
    {
        Init(dataSheetId);
    }

    void Init(int dataSheetId)
    {
        ItemData itemData = null;
        Managers.Data.DictItem.TryGetValue(dataSheetId, out itemData);
        if (itemData.itemType != ItemType.Weapon)
            return;

        WeaponData data = (WeaponData)itemData;
        {
            DataSheetId = data.id;
            Count = 1;
            WeaponType = data.weaponTypeEnum;
            Damage = data.damage;
            Stackable = false;
        }
    }
}

public class Armor : Item
{
    public ArmorType ArmorType { get; private set; }
    public int Defence { get; private set; }
    public Armor(int dataSheetId) : base(ItemType.Armor)
    {
        Init(dataSheetId);
    }

    void Init(int dataSheetId)
    {
        ItemData itemData = null;
        Managers.Data.DictItem.TryGetValue(dataSheetId, out itemData);
        if (itemData.itemType != ItemType.Armor)
            return;

        ArmorData data = (ArmorData)itemData;
        {
            DataSheetId = data.id;
            Count = 1;
            ArmorType = data.armorTypeEnum;
            Defence = data.defence;
            Stackable = false;
        }
    }
}

public class Consumable : Item
{
    public ConsumableType ConsumableType { get; private set; }
    public int MaxCount { get; private set; }
    public Consumable(int dataSheetId) : base(ItemType.Consumable)
    {
        Init(dataSheetId);
    }

    void Init(int dataSheetId)
    {
        ItemData itemData = null;
        Managers.Data.DictItem.TryGetValue(dataSheetId, out itemData);
        if (itemData.itemType != ItemType.Consumable)
            return;

        ConsumableData data = (ConsumableData)itemData;
        {
            DataSheetId = data.id;
            Count = 1;
            ConsumableType = data.consumableTypeEnum;
            MaxCount = data.maxCount;
            Stackable = (data.maxCount > 1);
        }
    }
}
