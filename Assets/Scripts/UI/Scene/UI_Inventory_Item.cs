using Google.Protobuf.MyProtocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
    [SerializeField]
    Image _icon;

    [SerializeField]
    Image _frame;

    public int ItemDbId { get; private set; }

    public int Slot {  get; private set; }
    public int DataSheetId { get; private set; }
    public int Count { get; private set; }
    public bool Equipped { get; private set; }

    public override void Init()
    {
        _icon.gameObject.AddUIEventHandler((evt) =>
        {
            Debug.Log("Click Item");

            Data.ItemData itemData = GetItem(DataSheetId);

            // TODO : C_USE_ITEM 아이템 사용 패킷
            if (itemData != null && itemData.itemType == ItemType.Consumable)
                return;

            C_EquipItem equipPacket = new();
            equipPacket.Slot = Slot;
            equipPacket.Equipped = !Equipped;

            Managers.Network.Send(equipPacket);
        });
    }

    internal void SetItem(Item item)
    {

        if (item == null)
        {
            ItemDbId = -1;
            DataSheetId = -1;
            Slot = -1;
            Equipped = false;

            Transform background = transform.Find("Background");
            Image backImage = background.GetComponent<Image>();

            _icon.sprite = backImage.sprite;
            _frame.gameObject.SetActive(false);
        }
        else
        {
        ItemDbId = item.ItemDbId;
        DataSheetId = item.DataSheetId;
        Count = item.Count;
        Slot = item.Slot;
        Equipped = item.Equipped;

        Data.ItemData itemData = GetItem(DataSheetId);

        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        _icon.sprite = icon;
        _frame.gameObject.SetActive(Equipped);
    }
    }

    Data.ItemData GetItem(int dataSheetId)
    {
        Data.ItemData itemData = null;
        Managers.Data.DictItem.TryGetValue(DataSheetId, out itemData); ;

        return itemData;
    }
}
