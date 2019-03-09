using UnityEngine;
using System.Collections.Generic;
using System;

public class CubeManager
{
    private static Dictionary<String, Stack<GameObject>> gameObjectPool = new Dictionary<string, Stack<GameObject>>();

    public static bool HasGameObjectOfPrefab(GameObject prefab)
    {
        return gameObjectPool.ContainsKey(prefab.tag) && 
            gameObjectPool[prefab.tag].HasValues();
    }

    public static GameObject GetGameObjectOfFromPool(GameObject prefab)
    {
        if (!HasGameObjectOfPrefab(prefab))
        {
            // Does not have gameobject of prefab
            throw new InvalidOperationException($"CubeManager does not have game object of prefab {prefab.tag}");
        }

        return gameObjectPool[prefab.tag].Pop();
    }

    public static void AddGameObjectToPool(GameObject gameObject)
    {
        if (!gameObjectPool.ContainsKey(gameObject.tag))
        {
            gameObjectPool[gameObject.tag] = new Stack<GameObject>();
        }

        gameObjectPool[gameObject.tag].Push(gameObject);
    }
}