using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    Dictionary<Type, UnityEngine.Object[]> _unityObjects = new Dictionary<Type, UnityEngine.Object[]>();

    private void Awake()
    {
        Init();
    }

    public abstract void Init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _unityObjects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = Util.FindChild<Transform>(gameObject, names[i], true)?.gameObject;
            }
            else
            {
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }

            if (null == objects[i])
            {
                Debug.Log($"Failed to bind: {names[i]}");
            }
        }
    }

    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (false == _unityObjects.TryGetValue(typeof(T), out objects))
        {
            return null;
        }

        return (T)objects[index];
    }

    protected GameObject GetGameObject(int index)
    {
        return Get<GameObject>(index);
    }

    protected Text GetText(int index)
    {
        return Get<Text>(index);
    }

    protected Button GetButton(int index)
    {
        return Get<Button>(index);
    }

    protected Image GetImage(int index)
    {
        return Get<Image>(index);
    }

    public static void AddUIEventHandler(GameObject gameObject, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler eventHandler = Util.GetOrAddComponent<UI_EventHandler>(gameObject);
        switch (type)
        {
            case Define.UIEvent.Click:
                eventHandler.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                eventHandler.OnDragHandler += action;
                break;
        }
    }
}
