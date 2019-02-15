using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object _lock = new object();

    protected static T GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                // Looks for the objects in the scene
                _instance = (T)FindObjectOfType(typeof(T));

                // If it can't find the object, create and add the component.
                if (_instance == null)
                {
                    var gameObject = new GameObject();
                    _instance = gameObject.AddComponent<T>();
                    _instance.name = $"{typeof(T)} (SingleTone)";
                }
            }
        }
        

        return _instance;
    }
}

