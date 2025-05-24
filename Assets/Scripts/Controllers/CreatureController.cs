using Google.Protobuf.MyProtocol;
using TMPro;
using UnityEngine;

public class CreatureController : BaseController
{
    protected int _objectId;
    HpBar _hpBar;
    TextMeshProUGUI _nameText;

    public string NameText 
    { 
        get => _nameText.text;
        set
        {
            if (_nameText == null)
                AddNameText();
            _nameText.text = value;
            UpdateNameText();
        }
    }

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
        go.transform.localPosition = new Vector3(0, 0.7f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    protected void AddNameText()
    {
        GameObject go = Managers.Resource.Instantiate("UI/NameTag", transform);
        go.transform.localPosition = new Vector3(-0.04f, 1f, 0);
        go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300f, 50f);
        _nameText = go.GetComponentInChildren<TextMeshProUGUI>();
        _nameText.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 50f);
    }

    void UpdateNameText()
    {
        _nameText.name = NameText;
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
        AddNameText();
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
        Status = EntityStatus.StatusDead;
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

    public void OnDeathAnimEnd()
    {
        C_AnimEnd C_AnimEnd = new C_AnimEnd();
        C_AnimEnd.ObjectId = _objectId;
        Managers.Network.Send(C_AnimEnd);
    }
}
