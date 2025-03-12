using Google.Protobuf.MyProtocol;

public class ArrowController : BaseController
{
    protected override void Init()
    {
        base.Init();

        switch (Dir)
        {
            case MoveDir.Up:
                transform.Rotate(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.Rotate(0, 0, 180);
                break;
            case MoveDir.Left:
                transform.Rotate(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.Rotate(0, 0, -90);
                break;
        }

        State = EntityState.Moving;
    }

    // 화살은 애니메이션이 필요없다
    protected override void UpdateAnimation() { }
}