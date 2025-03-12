using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return Util.GetOrAddComponent<T>(gameObject);
    }

    public static void AddUIEventHandler(this GameObject gameObject, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.AddUIEventHandler(gameObject, action, type);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }
}