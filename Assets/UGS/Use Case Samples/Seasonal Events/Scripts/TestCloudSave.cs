using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using Unity.Services.Samples.SeasonalEvents;
using UnityEngine;

public class TestCloudSave : MonoBehaviour
{
    [Serializable]
    public class TestObject
    {
        public string SophisticatedString;
        public int SparklingInt;
        public float AmazingFloat;
    }

    public async void OnClickUpButton()
    {
        // await CallTestCloudCodeWithoutParam();
        await TestSaveDataOnCloud();
    }

    public async void OnClickDownButton()
    {
        TestObject incomingSample = await RetrieveSpecificData<TestObject>("object_key");
        Debug.Log($"Loaded sample object: {incomingSample.AmazingFloat}, {incomingSample.SparklingInt}, {incomingSample.SophisticatedString}");
    }

    private async Task CallTestCloudCodeWithoutParam()
    {
        TestObject outgoingSample = new TestObject()
        {
            AmazingFloat = 13.37f,
            SparklingInt = 1337,
            SophisticatedString = "hi there!"
        };
        
        try
        {
            Debug.Log("Collecting test via Cloud Code...");

            var testResult = await CloudCodeService.Instance.CallEndpointAsync(
                "TestCloudWithoutParam",
                new Dictionary<string, object>());

            // Check that scene has not been unloaded while processing async wait to prevent throw.
            if (this == null) return;

            Debug.Log($"Collect test result: {testResult}");
        }
        catch (CloudCodeException e)
        {
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            Debug.Log("Problem calling cloud code endpoint: " + e.Message);
            Debug.LogException(e);
        }
    }

    private async Task TestSaveDataOnCloud()
    {
        TestObject outgoingSample = new TestObject()
        {
            AmazingFloat = 13.37f,
            SparklingInt = 1337,
            SophisticatedString = "hi there!"
        };
        await ForceSaveObjectData("object_key", outgoingSample);
    }
    
    private async Task ForceSaveObjectData(string key, TestObject value)
    {
        try
        {
            // Although we are only saving a single value here, you can save multiple keys
            // and values in a single batch.
            Dictionary<string, object> oneElement = new Dictionary<string, object>
            {
                { key, value }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

            Debug.Log($"Successfully saved {key}:{value}");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }
    
    private async Task<T> RetrieveSpecificData<T>(string key)
    {
        try
        {
            var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {key});

            if (results.TryGetValue(key, out string value))
            {
                return JsonUtility.FromJson<T>(value);
            }
            else
            {
                Debug.Log($"There is no such key as {key}!");
            }
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }

        return default;
    }
}