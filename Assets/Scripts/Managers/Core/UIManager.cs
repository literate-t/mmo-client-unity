using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    // 0 ~ 9는 예약으로 남겨두자
    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    public UI_Scene SceneUI { get; private set; } = null;

    public GameObject Root
    {
        get
        {
            GameObject uiRoot = GameObject.Find("@UI_root");
            if (null == uiRoot)
                uiRoot = new GameObject { name = "@UI_root" };

            return uiRoot;
        }
    }

    public void SetCanvas(GameObject gameObject, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
            canvas.sortingOrder = _order++;
        else
            canvas.sortingOrder = 0;
    }

    public T MakeWorldUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject gameObject = Managers.Resource.Instantiate($"UI/World/{name}");

        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }

        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return gameObject.GetOrAddComponent<T>();
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject gameObject = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }

        return gameObject.GetOrAddComponent<T>();
    }

    /// <summary>
    /// name: prefab 이름 /
    /// T: 스크립트(ex. UI_Button.cs)
    /// </summary>
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject gameObject = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T scene = Util.GetOrAddComponent<T>(gameObject);
        SceneUI = scene;

        // 폴더가 없으므로 하나의 오브젝트를 생성해 하위에 둔다
        gameObject.transform.SetParent(Root.transform);

        return scene;
    }

    /// <summary>
    /// name: prefab 이름 /
    /// T: 스크립트(ex. UI_Button.cs)
    /// </summary>
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject gameObject = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(gameObject);
        _popupStack.Push(popup);

        // 폴더가 없으므로 하나의 오브젝트를 생성해 하위에 둔다}
        gameObject.transform.SetParent(Root.transform);

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (0 == _popupStack.Count)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("ClosePopupUI() Failed");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (0 == _popupStack.Count)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;

        --_order;
    }

    public void CloseAllPopupUI()
    {
        while (0 < _popupStack.Count)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        SceneUI = null;
    }
}
