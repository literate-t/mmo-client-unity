using System.Collections.Generic;
using UnityEngine;

class Pool
{
    public GameObject Original { get; private set; }
    public GameObject Root { get; set; }

    Stack<Poolable> _poolStack = new();

    public void Init(GameObject original, int count = 5)
    {
        // prefab으로 생성한 원본을 받는다
        Original = original;
        Root = new GameObject();
        Root.name = $"{original.name}_Root";

        for (int i = 0; i < count; ++i)
        {
            Push(Create());
        }
    }

    Poolable Create()
    {
        // 원본으로 복사본을 생성해 씬에 올린다
        GameObject gameObject = Object.Instantiate(Original);
        gameObject.name = Original.name;

        // 풀링 객체를 식별하기 위해 Poolable 컴포넌트를 추가
        return gameObject.GetOrAddComponent<Poolable>();
    }

    public void Push(Poolable poolable)
    {
        if (poolable == null)
            return;

        poolable.transform.SetParent(Root.transform);
        poolable.gameObject.SetActive(false);
        poolable.isPooled = true;

        _poolStack.Push(poolable);
    }
    
    public Poolable Pop(Transform parent = null)
    {
        Poolable poolable;
        if (_poolStack.Count > 0)
        {
            poolable = _poolStack.Pop();
        }
        else
        {
            poolable = Create();
        }

        // DontDestroyOnLoad 해제
        if (parent == null)
        {
            poolable.transform.SetParent(Managers.Scene.CurrentScene.transform);
        }

        poolable.gameObject.SetActive(true);
        poolable.transform.SetParent(parent);
        poolable.isPooled = false;

        return poolable;
    }
}

public class PoolManager
{       
    Dictionary<string, Pool> _pool = new();
    GameObject _root;
    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" };
            Object.DontDestroyOnLoad(_root);
        }
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.name;
        // PoolManager를 통하지 않았는데 반납을 하는 건
        // 편집기에서 드래그 앤 드롭으로 넣었을 때의 처리이기 때문에
        // PoolManager로 돌려보내지 않는다
        if (_pool.ContainsKey(name) == false)
        {
            Object.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }
    
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
        {
            CreatePool(original);
        }

        return _pool[original.name].Pop(parent);
    }

    private void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new();
        pool.Init(original, count);
        pool.Root.transform.SetParent(_root.transform);

        _pool.Add(original.name, pool);
    }

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    // 씬마다 쓰는 오브젝트가 너무 다르다면 날려줘야 함
    public void Clear()
    {
        // 자식을 순회할 수 있는 Enumerator을 가지고 있다
        foreach (Transform child in _root.transform)
        {
            // child 밑에 child tree가 있어도 가장 상위 child를 해제하면
            // 하위 child도 전부 해제된다
            Object.Destroy(child.gameObject);
        }

        _pool.Clear();
    }
}
