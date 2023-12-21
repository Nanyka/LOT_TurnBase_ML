using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void Awake()  // protected allow coder to access it from childClass, that extend from thisClass; virtual allow to override it when create childClasses
    {
        if (instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a second instance of a singleton class.");
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    protected virtual void OnDestroy()  // ensure that a new instance will be created when this singleton is destroyed
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
