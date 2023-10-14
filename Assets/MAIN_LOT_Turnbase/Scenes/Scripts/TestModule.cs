using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class TestModule : MonoBehaviour
{
    private async void Start()
    {
        // Initialize the Unity Services Core SDKvar options = new InitializationOptions();
        var options = new InitializationOptions();
        options.SetEnvironmentName("dev");
        await UnityServices.InitializeAsync(options);

        // Authenticate by logging into an anonymous account
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try
        {
            // Call the function within the module and provide the parameters we defined in there
            string result = await CloudCodeService.Instance.CallModuleEndpointAsync("TestCSharpModule", "SayHello", new Dictionary<string, object> {{"name", "BinzLai"}});

            Debug.Log(result);
        }
        catch (CloudCodeException exception)
        {
            Debug.LogException(exception);
        }
    }
}
