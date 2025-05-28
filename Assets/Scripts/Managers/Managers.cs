using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _s_instance;
    static Managers Instance { get { Init(); return _s_instance; } }

    #region Contents
    InventoryManager _inventory = new();
    MapManager _map = new();
    ObjectManager _object = new();
    NetworkManager _network = new();

    public static InventoryManager Inventory => Instance._inventory;
    public static MapManager Map => Instance._map;
    public static ObjectManager Object => Instance._object;
    public static NetworkManager Network => Instance._network;
    #endregion

    #region Core
    DataManager _data = new();
    ResourceManager _resource = new();
    SceneManagerEx _scene = new();
    SoundManager _sound = new();
    UIManager _ui = new();
    public static DataManager Data => Instance._data;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;
    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        _network.Update();
    }

    static void Init()
    {
        if (_s_instance == null)
        {
            GameObject ob = GameObject.Find("@Managers");
            if (ob == null)
            {
                ob = new GameObject { name = "@Managers" };
                ob.AddComponent<Managers>();
            }

            DontDestroyOnLoad(ob);
            _s_instance = ob.GetComponent<Managers>();

            _s_instance._network.Init();
            _s_instance._data.Init();
            _s_instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
    }
}