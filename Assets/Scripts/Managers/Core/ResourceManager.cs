using UnityEngine;
using static UnityEngine.UI.Image;

public class ResourceManager
{    
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string objectName = path;
            int index = objectName.LastIndexOf("/");
            if (index >= 0)
                objectName = objectName.Substring(index + 1);

            GameObject gameObject = Managers.Pool.GetOriginal(objectName);
            if (gameObject != null)
                return gameObject as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        // 이 객체가 원본에 해당한다
        // 아직 씬에 올리지 않고 메모리에 들고 있기만 한다        
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab: {path}");
            return null;
        }

        // 씬에 등록한다        
        if (original.GetComponent<Poolable>() != null)
        {
            return Managers.Pool.Pop(original, parent).gameObject;
        }

        GameObject gameObject = Object.Instantiate(original, parent);
        gameObject.name = original.name;

        return gameObject;
    }

    public void Destroy(GameObject gameObject, float time = 0f)
    {
        if (gameObject == null)
            return;

        // 풀링 객체면 파괴하지 않고 PoolManager로 돌려보낸다
        Poolable poolable = gameObject.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        
        Object.Destroy(gameObject, time);
    }
}
