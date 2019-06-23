using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError("No GameObject have " + typeof(T) + " component.");
                }
            }

            return instance;
        }
    }


    virtual protected void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            Debug.LogError(typeof(T) + " is already attached to the other GameObject, so the existing component was removed.");
            return;
        }
        //DontDestroyOnLoad(this.gameObject);
    }

}