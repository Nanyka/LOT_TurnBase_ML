using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<PoolItem> items;
    public List<GameObject> pooledItems;

    // Use this for initialization
    void Start()
    {
        pooledItems = new List<GameObject>();
        foreach (PoolItem item in items)
        {
            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = Instantiate(item.prefab, item.prefCollector);
                obj.SetActive(false);
                pooledItems.Add(obj);
            }
        }
    }
    
    public GameObject GetObject()
    {
        for (int i = 0; i < pooledItems.Count; i++)
            if (!pooledItems[i].activeInHierarchy)
                return pooledItems[i];

        foreach (PoolItem item in items)
        {
            if (item.expandable)
            {
                GameObject obj = Instantiate(item.prefab, item.prefCollector);
                obj.SetActive(false);
                pooledItems.Add(obj);
                return obj;
            }
        }

        return null;
    }
    
    public GameObject GetObject(string objectName)
    {
        for (int i = 0; i < pooledItems.Count; i++)
        {
            if (!pooledItems[i].activeInHierarchy && Regex.Replace(pooledItems[i].name, @"Clone|\W","") == objectName)
            {
                return pooledItems[i];
            }
        }

        foreach (PoolItem item in items)
        {
            if (Regex.Replace(item.prefab.name, @"Clone|\W","") == objectName && item.expandable)
            {
                GameObject obj = Instantiate(item.prefab, item.prefCollector);
                obj.SetActive(false);
                pooledItems.Add(obj);
                return obj;
            }
        }

        return null;
    }

    public List<GameObject> GetActiveItemList()
    {
        List<GameObject> activeObjects = new List<GameObject>();
        foreach (var item in pooledItems)
        {
            if (item.activeInHierarchy)
                activeObjects.Add(item);
        }

        return activeObjects;
    }

    public int CountActiveItem()
    {
        int numberActive = 0;
        foreach (var item in pooledItems)
        {
            if (item.activeInHierarchy)
                numberActive++;
        }
        return numberActive;
    }

    public GameObject GetRandomActiveItem()
    {
        List<GameObject> activeObjects = GetActiveItemList();
        int randomIndex = Random.Range(0, activeObjects.Count);
        if (activeObjects.Count > 0)
            return activeObjects[randomIndex];
        else
            return null;
    }
}