using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using UnityEngine;

public class TestLoadSO : MonoBehaviour
{
    [SerializeField] private string SOAddress;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetSO());
    }

    private IEnumerator GetSO()
    {
        yield return new WaitUntil(() => Input.anyKey);
        
        var getSO = AddressableManager.Instance.GetAddressableSO(SOAddress) as ResourceStats;
        Debug.Log($"Get Scriptable object: {getSO.Commands.ToString()}");
    }
}
