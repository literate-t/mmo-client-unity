using System.Collections;
using Google.Protobuf.MyProtocol;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected virtual void CheckUpdatedFlagAndSend()
    { }

    IEnumerator CoStartPunch()
    {
        // 피격 판정은 서버에서 한다

        _rangedSkill = false;
        State = EntityState.Skill;
        yield return new WaitForSeconds(0.5f);
        State = EntityState.Idle;
        _coSkill = null;
        
        CheckUpdatedFlagAndSend();
    }

    IEnumerator CoStartShootArrow()
    {
        _rangedSkill = true;
        State = EntityState.Skill;
        yield return new WaitForSeconds(0.2f);
        State = EntityState.Idle;
        _coSkill = null;

        CheckUpdatedFlagAndSend();
    }

    internal override void UseSkill(int skillId)
    {
        // 1번이 일단 펀치로 가정
        if (skillId == 1)
        {
            _coSkill = StartCoroutine("CoStartPunch");
        }
        else if (skillId == 2)
        {
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }
}
