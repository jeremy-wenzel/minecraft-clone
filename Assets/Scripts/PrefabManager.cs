using UnityEngine;

/// <summary>
/// Holds all the Prefabs for a particular scene. Must be attached to a Empty Game object
/// and be in the Scene. Also each Prefab in the Manager must have a reference to a prefab.
/// </summary>
public class PrefabManager: Singleton<PrefabManager>
{
    public GameObject Chunk;
    public GameObject Grass;
    public GameObject Snow;
    public GameObject Tree;

    /// <summary>
    /// Get a particular prefab.
    /// </summary>
    /// <param name="prefabType"></param>
    /// <returns></returns>
    public static GameObject GetPrefab(PrefabType prefabType)
    {
        switch(prefabType)
        {
            case PrefabType.Chunk:
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

/// <summary>
/// The Prefabs that we can make.
/// </summary>
public enum PrefabType
{
    Chunk,
    Grass,
    Snow,
    Tree
}