using UnityEngine;
using UnityEditor;

public class PrefabManager: MonoBehaviour
{
    public GameObject CHUNK;
    public GameObject CUBE;

    private static PrefabManager _instance;

    private static readonly object _lock = new object();

    public void Awake()
    {
        if (_instance == null)
        {
            lock(_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                }
            }
        }
    }

    public static GameObject GetPrefab(PrefabType prefabType)
    {
        switch(prefabType)
        {
            case PrefabType.CHUNK:
                return _instance.CHUNK;
            case PrefabType.CUBE:
                return _instance.CUBE;
            default:
                Debug.Log($"Prefab type unknown or not implemented {prefabType}");
                return null;
        }
    }
}

public enum PrefabType
{
    CHUNK,
    CUBE
}