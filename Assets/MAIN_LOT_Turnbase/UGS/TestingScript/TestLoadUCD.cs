using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TestLoadUCD : MonoBehaviour
{
    [SerializeField] private string m_LogPrefab;
    [SerializeField] private Transform m_SkinAnchor;
    
    private AsyncOperationHandle m_UCDObjectLoadingHandle;

    private void OnEnable()
    {
        CheckUCD();
    }

    private void CheckUCD()
    {
        //Add private token to addressable web request header
        Addressables.WebRequestOverride = AddPrivateToken;
        
        m_UCDObjectLoadingHandle = Addressables.InstantiateAsync(m_LogPrefab, m_SkinAnchor, false);
        m_UCDObjectLoadingHandle.Completed += OnObjectInstantiated;
    }
    
    private void AddPrivateToken(UnityWebRequest request)
    {
        string bucketAccessToken = "313a05c186c7fc63e17fc91fc00265a7"; // Add your bucket token here
    
        var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($":{bucketAccessToken}"));
    
        request.SetRequestHeader("Authorization", $"Basic {encodedToken}");
    }
    
    private void OnDisable()
    {
        m_UCDObjectLoadingHandle.Completed -= OnObjectInstantiated;
    }
    
    private void OnObjectInstantiated(AsyncOperationHandle obj)
    {
        // We can check for the status of the InstantiationAsync operation: Failed, Succeeded or None
        if(obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Object instantiated successfully");
        }
    
        if (obj.Status == AsyncOperationStatus.Failed)
        {
            Debug.Log("Fail to load UCD:\n" + obj.Result);
            m_UCDObjectLoadingHandle = Addressables.GetDownloadSizeAsync("BuildingTest");
            m_UCDObjectLoadingHandle.Completed += OnCheckSize;
        }
    }
    
    private void OnCheckSize(AsyncOperationHandle obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Download size: {obj.Result}");
            m_UCDObjectLoadingHandle = Addressables.DownloadDependenciesAsync("BuildingTest");
            m_UCDObjectLoadingHandle.Completed += OnDownloadSuccess;
        }
    }
    
    private void OnDownloadSuccess(AsyncOperationHandle obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Success to download file");
        }
    }
}
