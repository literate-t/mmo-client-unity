using UnityEngine;

public class ResourceManager
{    
    public T Load<T>(string path) where T : Object
    {
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

        GameObject gameObject = Object.Instantiate(original, parent);
        gameObject.name = original.name;

        return gameObject;
    }

    public void Destroy(GameObject gameObject, float time = 0f)
    {
        if (gameObject == null)
            return;
        
        Object.Destroy(gameObject, time);
    }
}
