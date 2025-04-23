using System;
using System.Collections.Generic;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; } = new();

    internal void Add(Item item)
    {
        Items.Add(item.Slot, item);
    }

    internal Item Get(int itemDbId)
    {
        Item item = null;
        Items.TryGetValue(itemDbId, out item);
        return item;
    }

    internal bool Erase(int slot)
    {
        return Items.Remove(slot);
    }

    internal Item Find(Func<Item, bool> predicate)
    {
        foreach (Item item in Items.Values)
        {
            if (predicate(item)) return item;
        }

        return null;
    }

    internal void Clear()
    {
        Items.Clear();
    }
}
