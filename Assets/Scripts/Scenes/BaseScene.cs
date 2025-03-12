using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

// 모든 Scene에 대한 최상위 부모
public abstract class BaseScene : MonoBehaviour
{
    public Scene SceneType { get; protected set; } = Scene.Unknown;

    // 편집기에서 스크립트가 꺼져 있어도 호출된다
    // 초기화 루틴은 상위 클래스에서 Awake()에 넣어놓으면 하위 클래스에서 안 넣어줘도 된다
    void Awake()
    {
        Init();
    }
    
    protected virtual void Init()
    {
        Object findObject = FindObjectOfType(typeof(EventSystem));
        if (findObject == null)
        {
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        }
    }

    public abstract void Clear();
}
