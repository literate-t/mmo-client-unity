using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;

    protected override void Init()
    {
        base.Init();

        SceneType = Scene.Game;

        Managers.Map.LoadMap(2);

        Screen.SetResolution(1080, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
    }

    public override void Clear()
    {
    }
}
