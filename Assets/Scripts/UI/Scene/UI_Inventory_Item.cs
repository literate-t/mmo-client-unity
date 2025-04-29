using Google.Protobuf.MyProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private RectTransform _itemRectTransform;
    private CanvasGroup _itemCanvasGroup;
    private Canvas _rootCanvas;
    private Transform _itemOriginalParent;
    private Vector3 _origin_position;

    public override void Init()
    {
        _icon.gameObject.AddUIEventHandler(OnDoubleClick, Define.UIEvent.DoubleClick);
        _icon.gameObject.AddUIEventHandler(OnBeginDrag, Define.UIEvent.BeginDrag);
        _icon.gameObject.AddUIEventHandler(OnDrag, Define.UIEvent.Drag);
        _icon.gameObject.AddUIEventHandler(OnEndDrag, Define.UIEvent.EndDrag);

        _itemRectTransform = transform.Find("ItemIcon").GetComponent<RectTransform>();
        _origin_position = _itemRectTransform.anchoredPosition;
        _itemCanvasGroup = transform.Find("ItemIcon").GetComponent<CanvasGroup>();
        _rootCanvas = GetComponentInParent<Canvas>();
        _itemOriginalParent = _itemRectTransform.parent;
    }

    private void OnDoubleClick(PointerEventData pointerEventData)
        {
            Debug.Log("Click Item");

            Data.ItemData itemData = GetItem(DataSheetId);
            
            if (itemData != null && itemData.itemType == ItemType.Consumable)
            {
                C_UseItem useItem = new C_UseItem();
                useItem.Slot = Slot;

                Managers.Network.Send(useItem);
            }
            else
            {
                C_EquipItem equipPacket = new();
                equipPacket.Slot = Slot;
                equipPacket.Equipped = !Equipped;

                Managers.Network.Send(equipPacket);
            }
    }

    private void OnBeginDrag(PointerEventData pointerEventData)
    {
        // transparent
        _itemCanvasGroup.alpha = 0.5f;        
        _itemCanvasGroup.blocksRaycasts = false;

        _itemRectTransform.SetParent(_rootCanvas.transform, false);
    }

    private void OnDrag(PointerEventData pointerEventData)
    {
        if (_rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _itemRectTransform.position = pointerEventData.position;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.transform as RectTransform,
                pointerEventData.position,
                _rootCanvas.worldCamera,
                out Vector2 localPoint
                );

            _itemRectTransform.anchoredPosition = localPoint;
        }
    }
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
