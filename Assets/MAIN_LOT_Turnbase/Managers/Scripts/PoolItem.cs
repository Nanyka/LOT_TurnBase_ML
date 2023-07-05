using UnityEngine;

[System.Serializable]
public class PoolItem{
    public GameObject prefab;
    public Transform prefCollector;
    public int amount;
    public bool expandable;
}