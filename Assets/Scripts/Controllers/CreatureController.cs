using Google.Protobuf.MyProtocol;
using UnityEngine;

public class CreatureController : BaseController
{
    protected int _objectId;
    HpBar _hpBar;
    public override StatInfo Stat
    {
        get => base.Stat;
        set
        {
            base.Stat = value;
            UpdateHpBar();
        }
    }

    public override int Hp
    {
        get => Stat.Hp;
        set
        {
            base.Hp = value;
            UpdateHpBar();
        }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.4f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        float ratio = (float)Hp / Stat.MaxHp;
        if (_hpBar == null)
            return;

        _hpBar.SetHpBar(ratio);
    }


    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }

    void Update()
    {
        UpdateController();
    }

    public virtual void OnDamaged()
    {

    }

    internal virtual void OnDead(int? objectId)
    {
        State = EntityState.Dead;
        if (objectId.HasValue)
            _objectId = objectId.Value;        
    }

    internal virtual void OnDamaged(int? objectId)
    {
        Status = EntityStatus.Damaged;
        if (objectId.HasValue)
            _objectId = objectId.Value;
    }

    internal virtual void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            State = EntityState.Skill;
        }
    }
}
