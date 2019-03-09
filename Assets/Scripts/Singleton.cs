using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private static readonly object _lock = new object();

    protected static T GetInstance()
    {
        if (instance == null)
        {
            lock (_lock)
            {
                // Looks for the objects in the scene
                instance = (T)FindObjectOfType(typeof(T));

                // If it can't find the object, create and add the component.
                if (instance == null)
                {
                    var gameObject = new GameObject();
                    instance = gameObject.AddComponent<T>();
                    instance.name = $"{typeof(T)} (SingleTone)";
                }
            }
        }
        
        return instance;
    }
}

