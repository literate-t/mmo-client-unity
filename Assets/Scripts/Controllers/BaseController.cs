using Google.Protobuf.MyProtocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }
    protected Grid _grid;
    protected Animator _animator;
    internal bool _updated = false;
    protected SpriteRenderer _spriteRenderer;

    public PositionInfo _positionInfo = new PositionInfo();
    StatusInfo StatusInfo = new StatusInfo();
    StatInfo _stat = new StatInfo();

    public virtual StatInfo Stat
    {
        get => _stat;
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.MergeFrom(value);
        }
    }

    public float Speed
    {
        get => Stat.Speed;
        set => Stat.Speed = value;
    }

    public virtual int Hp
    {
        get => Stat.Hp;
        set => Stat.Hp = value;
    }

    public PositionInfo PositionInfo
    {
        get => _positionInfo;
        set
        {
            Dir = value.MoveDir;
            CellPosition = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
        }
    }

    public Vector3Int CellPosition
    {
        get => new Vector3Int(PositionInfo.PosX, PositionInfo.PosY, 0);
        set
        {
            if (PositionInfo.PosX == value.x && PositionInfo.PosY == value.y)
                return;

            PositionInfo.PosX = value.x;
            PositionInfo.PosY = value.y;
            UpdateAnimation();
            UpdateStatus();
            _updated = true;
        }
    }

    [SerializeField]
    public virtual EntityState State
    {
        get => PositionInfo.State;
        set
        {
            PositionInfo.State = value;
            UpdateAnimation();
            UpdateStatus();
            _updated = true;
        }
    }

    public virtual EntityStatus Status
    {
        get => StatusInfo.Status;
        set
        {
            if (StatusInfo.Status == value)
                return;

            StatusInfo.Status = value;

            UpdateAnimation();
            UpdateStatus();
            _updated = true;
        }
    }

    public MoveDir Dir
    {
        get => PositionInfo.MoveDir;
        set
        {
            if (PositionInfo.MoveDir == value)
                return;

            PositionInfo.MoveDir = value;

            UpdateAnimation();
            UpdateStatus();
            _updated = true;
        }
    }

    public MoveDir GetDirectionFormVector(Vector3Int dir)
    {
        MoveDir result;
        if (dir.x < 0)
            result = MoveDir.Left;
        else if (dir.x > 0)
            result = MoveDir.Right;
        else if (dir.y < 0)
            result = MoveDir.Down;
        else
            result = MoveDir.Up;

        return result;
    }

    public Vector3Int GetFrontCellPosition()
    {
        Vector3Int cellPostion = CellPosition;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPostion += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPostion += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPostion += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPostion += Vector3Int.right;
                break;
        }

        return cellPostion;
    }

    protected virtual void UpdateAnimation()
    {
        if (_animator == null || _spriteRenderer == null)
            return;
        
        if (State == EntityState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _spriteRenderer.flipX = false;
                    _animator.Play("IDLE_UP");
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
                    _animator.Play("IDLE_DOWN");
                    break;
            }
        }

        else if (State == EntityState.Moving)
        {
            switch (PositionInfo.MoveDir)
            {
                case MoveDir.Up:
                    _animator.Play("RUN_UP");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("RUN_DOWN");
                    _spriteRenderer.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("RUN_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("RUN_RIGHT");
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
                    _animator.Play("ATTACK_UP");
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _spriteRenderer.flipX = true;
                    break;
                case MoveDir.Right:
                    _spriteRenderer.flipX = false;
                    _animator.Play("ATTACK_RIGHT");
                    break;
                case MoveDir.Down:
                    _spriteRenderer.flipX = false;
                    _animator.Play("ATTACK_DOWN");
                    break;
            }
        }
        else
        {
            _animator.Play("DEATH");
        }
    }

    void UpdateStatus()
    {
        if (State == EntityState.Dead || _animator == null || _spriteRenderer == null)
            return;

        if (Status == EntityStatus.Damaged)
        {
            switch (Dir)
            {                
                case MoveDir.Up:
                    _spriteRenderer.flipX = false;
                    _animator.Play("HURT_UP");
                    break;
                case MoveDir.Down:
                    _spriteRenderer.flipX = false;
                    _animator.Play("HURT_DOWN");
                    break;
                case MoveDir.Left:
                    _spriteRenderer.flipX = true;
                    _animator.Play("HURT_RIGHT");
                    break;
                case MoveDir.Right:
                    _spriteRenderer.flipX = false;
                    _animator.Play("HURT_RIGHT");
                    break;
            }

            StatusInfo.Status = EntityStatus.Normal;
        }
    }

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _grid = Managers.Map.CurrentGrid;
        Vector3 position = _grid.CellToWorld(CellPosition) + new Vector3(0.5f, 0.5f);
        transform.position = position;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case EntityState.Idle:
                UpdateIdle();
                break;
            case EntityState.Moving:
                UpdateMoving();
                break;
            case EntityState.Skill:
                UpdateSkill();
                break;
            case EntityState.Dead:
                break;
        }
    }
    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMoving()
    {
        // 셀 중간에 들어올 수 있도록 하드코딩으로 new Vector3(0.5f, 0.5f)을 더해줌
        Vector3 destination = _grid.CellToWorld(CellPosition) + new Vector3(0.5f, 0.5f);
        Vector3 moveDirection = destination - transform.position;

        // 도착 여부
        float dist = moveDirection.magnitude;
        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destination;
            MoveToNextPosition();
        }
        else
        {
            transform.position += moveDirection.normalized * Time.deltaTime * Speed;
        }
    }

    protected virtual void MoveToNextPosition()
    {
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }
}
