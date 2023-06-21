using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.Samples.IdleClickerGame;
using UnityEngine;

namespace JumpeeIsland
{
    public class JICloudCodeManager : MonoBehaviour
    {
        // Cloud Code SDK status codes from Client
        const int k_CloudCodeRateLimitExceptionStatusCode = 50;
        const int k_CloudCodeMissingScriptExceptionStatusCode = 9002;
        const int k_CloudCodeUnprocessableEntityExceptionStatusCode = 9009;

        // HTTP REST API status codes
        const int k_HttpBadRequestStatusCode = 400;
        const int k_HttpTooManyRequestsStatusCode = 429;

        // Custom status codes
        const int k_UnexpectedFormatCustomStatusCode = int.MinValue;
        const int k_CloudSaveStateMissingCode = 2;
        const int k_SpaceOccupiedScriptStatusCode = 3;
        const int k_VirtualPurchaseFailedStatusCode = 4;
        const int k_WellNotFoundCode = 5;
        const int k_InvalidDragCode = 6;
        const int k_WellsDifferentLevelCode = 7;
        const int k_MaxLevelCode = 8;
        const int k_InvalidLocationCode = 9;
        const int k_WellLevelLockedCode = 10;

        // Unity Gaming Services status codes via Cloud Code
        const int k_EconomyPurchaseCostsNotMetStatusCode = 10504;
        const int k_EconomyValidationExceptionStatusCode = 1007;
        const int k_RateLimitExceptionStatusCode = 50;
        
        #region CLOUDSAVE ENVIRONMENT DATA

        public async Task CallSaveEnvData(EnvironmentData environmentData)
        {
            try
            {
                await CloudCodeService.Instance.CallEndpointAsync(
                    "JumpeeIsland_SaveEnvData",
                    new Dictionary<string, object> { { "EnvData", environmentData } });
            }
            catch (CloudCodeException e)
            {
                HandleCloudCodeException(e);
                throw new CloudCodeResultUnavailableException(e,
                    "Handled exception in CallGetUpdatedStateEndpoint.");
            }
        }
        
        public async Task<EnvironmentData> CallGetUpdatedStateEndpoint()
        {
            try
            {
                var updatedState = await CloudCodeService.Instance.CallEndpointAsync<EnvironmentData>(
                    "JumpeeIsland_GetUpdatedState",
                    new Dictionary<string, object>());

                return updatedState;
            }
            catch (CloudCodeException e)
            {
                HandleCloudCodeException(e);
                throw new CloudCodeResultUnavailableException(e,
                    "Handled exception in CallGetUpdatedStateEndpoint.");
            }
        }
        
        public async Task<EnvironmentData> CallResetStateEndpoint()
        {
            try
            {
                var resetData = await CloudCodeService.Instance.CallEndpointAsync<EnvironmentData>(
                    "JumpeeIsland_ResetGame",
                    new Dictionary<string, object>());
                
                return resetData;
            }
            catch (CloudCodeException e)
            {
                HandleCloudCodeException(e);
                throw new CloudCodeResultUnavailableException(e,
                    "Handled exception in CallResetDataEndpoint.");
            }
        }

        #endregion

        #region CURRENCY COMMAND BATCH

        public async Task CallProcessBatchEndpoint(string[] commands)
        {
            Debug.Log($"Number of commands at cloudCodeManager: {commands.Length}");
            if (commands is null || commands.Length <= 0)
                return;

            try
            {
                Debug.Log("Processing command batch via Cloud Code...");

                await CloudCodeService.Instance.CallEndpointAsync(
                    "JumpeeIsland_ProcessBatch",
                    new Dictionary<string, object> { { "commands", commands } });

                Debug.Log("Cloud Code successfully processed batch.");
            }
            catch (CloudCodeException e)
            {
                HandleCloudCodeException(e);
            }
            catch (Exception e)
            {
                Debug.Log("Problem calling cloud code endpoint: " + e.Message);
                Debug.LogException(e);
            }
        }

        #endregion

        #region LEADERBOARD

        public async Task<EnvironmentData> CallGetEnemyEnvironment(string enemyId)
        {
            try
            {
                var enemyEnv = await CloudCodeService.Instance.CallEndpointAsync<EnvironmentData>(
                    "JumpeeIsland_GetMapById",
                    new Dictionary<string, object>{{"enemyId",enemyId}});

                return enemyEnv;
            }
            catch (CloudCodeException e)
            {
                HandleCloudCodeException(e);
                throw new CloudCodeResultUnavailableException(e,
                    "Handled exception in CallGetEnemyEnvironment.");
            }
        }

        #endregion

        #region HANDLE EXCEPTIONs

        static CloudCodeCustomError ConvertToActionableError(CloudCodeException e)
        {
            try
            {
                // extract the JSON part of the exception message
                var trimmedMessage = e.Message;
                trimmedMessage = trimmedMessage.Substring(trimmedMessage.IndexOf('{'));
                trimmedMessage = trimmedMessage.Substring(0, trimmedMessage.LastIndexOf('}') + 1);

                // Convert the message string ultimately into the Cloud Code Custom Error object which has a
                // standard structure for all errors.
                return JsonUtility.FromJson<CloudCodeCustomError>(trimmedMessage);
            }
            catch (Exception exception)
            {
                return new CloudCodeCustomError("Failed to Parse Error", k_UnexpectedFormatCustomStatusCode,
                    "Cloud Code Unprocessable Entity exception is in an unexpected format and " +
                    $"couldn't be parsed: {exception.Message}", e);
            }
        }
        
        // This method does whatever handling is appropriate given the specific error. So for example for an invalid
        // play in the Cloud Ai Mini Game, it shows a popup in the scene to explain the error.
        void HandleCloudCodeScriptError(CloudCodeCustomError cloudCodeCustomError)
        {
            switch (cloudCodeCustomError.status)
            {
                case k_CloudSaveStateMissingCode:
                    Debug.Log("ShowCloudSaveMissingPopup");
                    break;

                case k_SpaceOccupiedScriptStatusCode:
                    Debug.Log("ShowSpaceOccupiedErrorPopup");
                    break;

                case k_VirtualPurchaseFailedStatusCode:
                    Debug.Log("ShowVirtualPurchaseFailedErrorPopup");
                    break;

                case k_WellNotFoundCode:
                    Debug.Log("ShowWellNotFoundPopup");
                    break;

                case k_InvalidDragCode:
                    Debug.Log("ShowInvalidDragPopup");
                    break;

                case k_WellsDifferentLevelCode:
                    Debug.Log("ShowWellsDifferentLevelPopup");
                    break;

                case k_MaxLevelCode:
                    Debug.Log("ShowMaxLevelPopup");
                    break;

                case k_InvalidLocationCode:
                    Debug.Log("ShowInvalidLocationPopup");
                    break;

                case k_WellLevelLockedCode:
                    Debug.Log("ShowWellLockedPopup");
                    break;

                case k_EconomyValidationExceptionStatusCode:
                case k_HttpBadRequestStatusCode:
                    Debug.Log("A bad server request occurred during Cloud Code script execution: " +
                        $"{cloudCodeCustomError.name}: {cloudCodeCustomError.message} : " +
                        $"{cloudCodeCustomError.details}");
                    break;

                case k_EconomyPurchaseCostsNotMetStatusCode:
                    Debug.Log("ShowVirtualPurchaseFailedErrorPopup");
                    break;

                case k_RateLimitExceptionStatusCode:
                    // With this status code, message will include which service triggered this rate limit.
                    Debug.Log($"{cloudCodeCustomError.message}. Wait {cloudCodeCustomError.retryAfter} " +
                        "seconds and try again.");
                    break;

                case k_HttpTooManyRequestsStatusCode:
                    Debug.Log($"Rate Limit has been exceeded. Wait {cloudCodeCustomError.retryAfter} " +
                        "seconds and try again.");
                    break;

                case k_UnexpectedFormatCustomStatusCode:
                    Debug.Log("Cloud Code returned an Unprocessable Entity exception, " +
                        $"but it could not be parsed: {cloudCodeCustomError.message}. " +
                        $"Original error: {cloudCodeCustomError.InnerException?.Message}");
                    break;

                default:
                    Debug.Log($"Cloud code returned error: {cloudCodeCustomError.status}: " +
                        $"{cloudCodeCustomError.name}: {cloudCodeCustomError.message}");
                    break;
            }
        }
        
        void HandleCloudCodeException(CloudCodeException e)
        {
            if (e is CloudCodeRateLimitedException cloudCodeRateLimitedException)
            {
                Debug.Log("Cloud Code rate limit has been exceeded. " +
                          $"Wait {cloudCodeRateLimitedException.RetryAfter} seconds and try again.");
                return;
            }

            switch (e.ErrorCode)
            {
                case k_CloudCodeUnprocessableEntityExceptionStatusCode:
                    var cloudCodeCustomError = ConvertToActionableError(e);
                    HandleCloudCodeScriptError(cloudCodeCustomError);
                    break;

                case k_CloudCodeRateLimitExceptionStatusCode:
                    Debug.Log("Rate Limit Exceeded. Try Again.");
                    break;

                case k_CloudCodeMissingScriptExceptionStatusCode:
                    Debug.Log("Couldn't find requested Cloud Code Script");
                    break;

                default:
                {
                    Debug.Log(e);
                }
                    break;
            }
        }
        
        class CloudCodeCustomError : Exception
        {
            public int status;
            public string name;
            public string message;
            public string retryAfter;
            public string[] details;

            public CloudCodeCustomError(string name, int status, string message = null,
                Exception innerException = null)
                : base(message, innerException)
            {
                this.name = name;
                this.status = status;
                this.message = message;
                retryAfter = null;
                details = new string[] { };
            }
        }

        #endregion
    }
}
