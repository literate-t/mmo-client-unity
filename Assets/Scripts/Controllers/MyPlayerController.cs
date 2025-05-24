using System.Collections;
using Google.Protobuf.MyProtocol;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    bool _moveKeyPressed = false;

    public int WeaponDamage { get; private set; }
    public int ArmorDefence { get; private set; }

    protected override void Init()
    {
        base.Init();
        RefreshAdditionalStat();
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    protected override void UpdateController()
    {
        GetUIKeyInput();
        switch (State)
        {
            case EntityState.Idle:
                InputKeyboard();
                break;
            case EntityState.Moving:
                InputKeyboard();
                break;
        }

        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        // 이동 상태 확인
        if (_moveKeyPressed)
        {
            State = EntityState.Moving;
            return;
        }

        // 스킬 상태 확인
        if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
        {
            C_Skill skill = new() { Info = new() };
            skill.Info.SkillId = 2; // arrow
            //skill.Info.SkillId = 1; // punch
            Managers.Network.Send(skill);

            _coSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
        }
    }

    Coroutine _coSkillCooltime;
    IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    protected override void MoveToNextPosition()
    {

        if (_moveKeyPressed == false)
        {
            State = EntityState.Idle;
            CheckUpdatedFlagAndSend();
            return;
        }

        Vector3Int destination = CellPosition;
        switch (PositionInfo.MoveDir)
        {
            case MoveDir.Up:
                destination += Vector3Int.up;
                break;
            case MoveDir.Down:
                destination += Vector3Int.down;
                break;
            case MoveDir.Left:
                destination += Vector3Int.left;
                break;
            case MoveDir.Right:
                destination += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destination))
        {
            if (Managers.Object.FindCreature(destination) == null)
            {
                CellPosition = destination;
            }
        }

        //EntityState prevState = State;
        //Vector3Int prevCellpostion = CellPosition;

        //base.MoveToNextPosition();

        //if (prevState == State && prevCellpostion == CellPosition)
        //    return;
        CheckUpdatedFlagAndSend();
    }

    protected override void CheckUpdatedFlagAndSend()
    {
        if (_updated == true && Status != EntityStatus.StatusDead)
        {
            C_Move movePacket = new();
            movePacket.PosInfo = PositionInfo;
            Managers.Network.Send(movePacket);

            _updated = false;
        }
    }

    void GetUIKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Inventory inventoryUI = gameSceneUI.InventoryUI;

            if (inventoryUI.gameObject.activeSelf)
                inventoryUI.gameObject.SetActive(false);
            else
            {
                inventoryUI.gameObject.SetActive(true);
                inventoryUI.RefreshUI();
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            UI_Stat statUI = gameSceneUI.StatUI;

            if (statUI.gameObject.activeSelf)
                statUI.gameObject.SetActive(false);
            else
            {
                statUI.gameObject.SetActive(true);
                statUI.RefreshUI();
            }
        }
    }

    void InputKeyboard()
    {
        _moveKeyPressed = true;
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            _moveKeyPressed = false;
        }
    }

    // TODO : 필요없다고 판단되면 제거해도 된다
    public void RefreshAdditionalStat()
    {
        WeaponDamage = 0;
        ArmorDefence = 0;

        foreach (Item item in Managers.Inventory.Items.Values)
        {
            if (!item.Equipped)
                continue;

            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    WeaponDamage += ((Weapon)item).Damage;
                    break;
                case ItemType.Armor:
                    ArmorDefence += ((Armor)item).Defence;
                    break;
            }
        }
    }
}
