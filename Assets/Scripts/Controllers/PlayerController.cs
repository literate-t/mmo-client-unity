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

    protected override void UpdateAnimation()
    {
        if (_animator == null || _spriteRenderer == null)
            return;

        if (State == EntityState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _spriteRenderer.flipX = false;
                    _animator.Play("IDLE_BACK");
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _spriteRenderer.flipX = false;
                    _animator.Play("IDLE_RIGHT");
                    break;
                case MoveDir.Down:
                    _spriteRenderer.flipX = false;
                    _animator.Play("IDLE_FRONT");
                    break;
            }
        }

        else if (State == EntityState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _spriteRenderer.flipX = false;
                    break;
            }
        }
        else if (State == EntityState.Skill)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _spriteRenderer.flipX = false;
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _spriteRenderer.flipX = false;
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    break;
                case MoveDir.Down:
                    _spriteRenderer.flipX = false;
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    break;
            }
        }
        else
        {
            // TODO : Dead
        }
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
        // 외부에서 MyPlayerController 인스턴스로 접근했을 때만 전송하도록
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
