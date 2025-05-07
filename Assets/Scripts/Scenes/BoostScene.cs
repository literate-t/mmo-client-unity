using UnityEngine;
using static Define;

public class BoostScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Scene.Boost;
    }

    public override void Clear()
    {
    }
}
