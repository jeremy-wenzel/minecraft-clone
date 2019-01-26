using UnityEngine;

public class PrefabManager: Singleton<PrefabManager>
{
    public GameObject CHUNK;
    public GameObject CUBE;
    public GameObject SNOW;

    public static GameObject GetPrefab(PrefabType prefabType)
    {
        switch(prefabType)
        {
            case PrefabType.CHUNK:
                return GetInstance().CHUNK;
            case PrefabType.CUBE:
                return GetInstance().CUBE;
            case PrefabType.SNOW:
                return GetInstance().SNOW;
            default:
                Debug.Log($"Prefab type unknown or not implemented {prefabType}");
                return null;
        }
    }
}

public enum PrefabType
{
    CHUNK,
    CUBE,
    SNOW
}