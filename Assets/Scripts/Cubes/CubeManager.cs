using UnityEngine;
using System.Collections.Generic;
using System;

public class CubeManager
{
    private static Dictionary<String, Stack<GameObject>> GameObjectPool = new Dictionary<string, Stack<GameObject>>();

    public static bool HasGameObjectOfPrefab(GameObject prefab)
    {
        return GameObjectPool.ContainsKey(prefab.tag) && 
            GameObjectPool[prefab.tag].HasValues();
    }

    public static GameObject GetGameObjectOfFromPool(GameObject prefab)
    {
        if (!HasGameObjectOfPrefab(prefab))
        {
            // Does not have gameobject of prefab
            throw new InvalidOperationException($"CubeManager does not have game object of prefab {prefab.tag}");
        }

        return GameObjectPool[prefab.tag].Pop();
    }

    public static void AddGameObjectToPool(GameObject gameObject)
    {
        if (!GameObjectPool.ContainsKey(gameObject.tag))
        {
            GameObjectPool[gameObject.tag] = new Stack<GameObject>();
        }

        GameObjectPool[gameObject.tag].Push(gameObject);
    }
}