using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject gameObject) where T : UnityEngine.Component
    {
        T component = gameObject.GetComponent<T>();
        if (null == component)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    // T: Button, Text etc
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (null == go)
        {
            return null;
        }

        if (false == recursive)
        {
            
            Transform transform =  go.transform.GetChild(0);
            if (/* string.IsNullOrEmpty(name) ||*/ transform.name == name)
            {
                T component = transform.GetComponent<T>();
                if (null != component)
                {
                    return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (/* string.IsNullOrEmpty(name) ||*/component.name == name)
                {
                    return component;
                }              
            }
        }

        return null;
    }
}
