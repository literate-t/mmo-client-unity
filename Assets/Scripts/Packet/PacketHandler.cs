using Google.Protobuf;
using Google.Protobuf.MyProtocol;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterPacket = packet as S_EnterGame;
        Managers.Object.Add(enterPacket.Player, true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leavePacket = packet as S_LeaveGame;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (ObjectInfo objectInfo in spawnPacket.Objects)
        {
            Managers.Object.Add(objectInfo);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;

        BaseController creature = go.GetComponent<BaseController>();

        creature.PositionInfo = movePacket.PosInfo;
    }

    internal static void S_SkillHandler(PacketSession session, IMessage packet)
    {
         S_Skill skillPacket = packet as S_Skill;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
            return;

        CreatureController creature = go.GetComponent<CreatureController>();
        creature.UseSkill(skillPacket.Info.SkillId);
    }

    internal static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp hpPacket = packet as S_ChangeHp;

        GameObject go = Managers.Object.FindById(hpPacket.ObjectId);
        if (go == null)
            return;

        CreatureController entity = go.GetComponent<CreatureController>();
        if (entity != null)
            entity.Hp = hpPacket.Hp;
    }

    internal static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        CreatureController entity = go.GetComponent<CreatureController>();
        if (entity != null)
            entity.OnDead(diePacket.ObjectId);
    }

    internal static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("S_ConnectedHandler");
        C_Login login = new();
        string dataPath = Application.dataPath;
        login.UniqueId = dataPath.GetHashCode().ToString();
        Managers.Network.Send(login);
    }

    // login ok + player list
    internal static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        S_Login loginPacket = (S_Login)packet;
        Debug.Log($"S_LoginHandler({loginPacket.LoginOk})");

        // TODO : show character, and choose character in lobby
        if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            C_CreatePlayer createPacket = new();
            if (Launcher.Id == null)
            createPacket.Name = MakeRandomName();
            else
                createPacket.Name = Launcher.Id;

            Managers.Network.Send(createPacket);
        }
        else
        {
            // login with first player
            LobbyPlayerInfo playerInfo = loginPacket.Players[0];
            C_EnterGame enterPacket = new();
            enterPacket.Name = playerInfo.Name;
            Managers.Network.Send(enterPacket);
        }
    }

    internal static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createPacket = (S_CreatePlayer)packet;
        if (createPacket.Player == null)
        {
            C_CreatePlayer newCreatePacket = new();
            newCreatePacket.Name = MakeRandomName();
            Managers.Network.Send(newCreatePacket);
        }
        else
        {
            C_EnterGame enterPacket = new();
            enterPacket.Name = createPacket.Player.Name;
            Managers.Network.Send(enterPacket);
        }
    }

    static string MakeRandomName()
    {
        return $"Player_{Random.Range(0, 10000):D4}";
    }

    internal static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemListPacket = (S_ItemList)packet;

        Managers.Inventory.Clear();

        // Caching in memory
        foreach (ItemInfo itemInfo in itemListPacket.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inventory.Add(item);
        }

        if (Managers.Object.MyPlayer != null)
            Managers.Object.MyPlayer.RefreshAdditionalStat();
    }

    internal static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemListPacket = (S_AddItem)packet;

        // Caching in memory
        foreach (ItemInfo itemInfo in itemListPacket.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inventory.Add(item);
        }

        UI_GameScene uiGameScene = (UI_GameScene)Managers.UI.SceneUI;
        uiGameScene.InventoryUI.RefreshUI();

        Managers.Object.MyPlayer.RefreshAdditionalStat();

        Debug.Log("Item 획득!");
    }

    internal static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        S_EquipItem equipItem = (S_EquipItem)packet;

        // 메모리에 아이템 정보 적용
        Item findItem = Managers.Inventory.Get(equipItem.Slot);
        if (findItem == null)
            return;

        findItem.Equipped = equipItem.Equipped;

        UI_GameScene uiGameScene = (UI_GameScene)Managers.UI.SceneUI;
        uiGameScene.InventoryUI.RefreshUI();
        uiGameScene.StatUI.RefreshUI();

        Managers.Object.MyPlayer.RefreshAdditionalStat();

        Debug.Log("Item equip changed!");
    }

    internal static void S_UseItemHandler(PacketSession session, IMessage message)
    {
        S_UseItem useItem = (S_UseItem)message;
        if (false == Managers.Inventory.Erase(useItem.Slot))
            return;

        UI_GameScene uiGameScene = (UI_GameScene)Managers.UI.SceneUI;
        uiGameScene.InventoryUI.RefreshUI(useItem.Slot);

        Debug.Log("Use item!");
    }

    internal static void S_DropItemHandler(PacketSession session, IMessage message)
    {
        S_DropItem dropItem = (S_DropItem)message;
        if (false == Managers.Inventory.Erase(dropItem.Slot))
            return;

        UI_GameScene uiGameScene = (UI_GameScene)Managers.UI.SceneUI;
        uiGameScene.InventoryUI.RefreshUI(dropItem.Slot);

        Debug.Log("Drop item!");
    }

    internal static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        throw new System.NotImplementedException();
    }

    internal static void S_PingHandler(PacketSession session, IMessage message)
    {
        C_Pong pong = new();
        Debug.Log("PingCheck");
        Managers.Network.Send(pong);
    }

    internal static void S_OnDamagedHandler(PacketSession session, IMessage packet)
    {
        S_OnDamaged damagedPacket = packet as S_OnDamaged;

        GameObject go = Managers.Object.FindById(damagedPacket.ObjectId);
        if (go == null)
            return;

        CreatureController entity = go.GetComponent<CreatureController>();
        if (entity != null)
            entity.OnDamaged(damagedPacket.ObjectId);
    }
}
