using UnityEngine;

public class PrefabManager: Singleton<PrefabManager>
{
    public GameObject Chunk;
    public GameObject Grass;
    public GameObject Snow;
    public GameObject Tree;

    public static GameObject GetPrefab(PrefabType prefabType)
    {
        switch(prefabType)
        {
            case PrefabType.CHUNK:
                return GetInstance().Chunk;
            case PrefabType.Grass:
                return GetInstance().Grass;
            case PrefabType.Snow:
                return GetInstance().Snow;
            case PrefabType.Tree:
                return GetInstance().Tree;
            default:
                Debug.Log($"Prefab type unknown or not implemented {prefabType}");
                return null;
        }
    }
}

public enum PrefabType
{
    CHUNK,
    Grass,
    Snow,
    Tree
}