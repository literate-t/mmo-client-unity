public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InventoryUI { get; private set; }
    public override void Init()
    {
        base.Init();
        InventoryUI = GetComponentInChildren<UI_Inventory>();
        StatUI = GetComponentInChildren<UI_Stat>();

        InventoryUI.gameObject.SetActive(false);
        StatUI.gameObject.SetActive(false);
    }
}
