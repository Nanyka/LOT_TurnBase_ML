using System;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class MailBoxPanelManager : MonoBehaviour
    {
        public MailBoxView sceneView;
        public readonly int maxInboxSize = 5;
        
        public string selectedMessageId { get; private set; }

        private JICloudSaveManager _cloudSaveManager;

        public async Task Init()
        {
            try
            {
                Debug.Log("Test why this function go into loop");
                _cloudSaveManager = FindObjectOfType<JICloudSaveManager>();
                
                await FetchUpdatedInboxData();
                if (this == null) return;

                
                // sceneView.SetInteractable(true);
                // sceneView.RefreshView();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        async Task FetchUpdatedInboxData()
        {
            await Task.WhenAll(
                JIRemoteConfigManager.instance.FetchMailboxConfigs(),
                _cloudSaveManager.FetchPlayerInbox()
            );
            if (this == null) return;

            _cloudSaveManager.DeleteExpiredMessages();
            _cloudSaveManager.CheckForNewMessages();
            await _cloudSaveManager.SavePlayerInboxInCloudSave();
        }

        // async void Update()
        // {
        //     try
        //     {
        //         // Note a more optimized implementation would only ask CloudSaveManager to delete expired messages
        //         // once a minute.
        //         if (_cloudSaveManager.DeleteExpiredMessages() > 0)
        //         {
        //             _cloudSaveManager.CheckForNewMessages();
        //             await _cloudSaveManager.SavePlayerInboxInCloudSave();
        //             if (this == null) return;
        //
        //             sceneView.RefreshView();
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //     }
        // }

        public async void SelectMessage(string messageId)
        {
            try
            {
                selectedMessageId = messageId;
                _cloudSaveManager.MarkMessageAsRead(messageId);
                await _cloudSaveManager.SavePlayerInboxInCloudSave();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void OnDeleteOpenMessageButtonPressed()
        {
            try
            {
                _cloudSaveManager.DeleteMessage(selectedMessageId);
                sceneView.RefreshView();
                await _cloudSaveManager.SavePlayerInboxInCloudSave();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        // TODO grant reward locally instead of using CloudCode
        public async void OnClaimOpenMessageAttachmentButtonPressed()
        {
            try
            {
                await JICloudCodeManager.instance.CallClaimMessageAttachmentEndpoint(selectedMessageId);
                if (this == null) return;

                await UpdateSceneAfterClaim();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Task UpdateSceneAfterClaim()
        {
            // TODO update local currency
            await Task.WhenAll(
                JIEconomyManager.instance.RefreshCurrencyBalances(),
                _cloudSaveManager.FetchPlayerInbox()
            );
            if (this == null) return;

            sceneView.RefreshView();
        }

        public async void OnClaimAllButtonPressed()
        {
            try
            {
                await JICloudCodeManager.instance.CallClaimAllMessageAttachmentsEndpoint();
                if (this == null) return;

                await UpdateSceneAfterClaim();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void OnDeleteAllReadAndClaimedButtonPressed()
        {
            try
            {
                if (_cloudSaveManager.DeleteAllReadAndClaimedMessages() > 0)
                {
                    sceneView.RefreshView();
                    await _cloudSaveManager.SavePlayerInboxInCloudSave();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void OnOpenInventoryButtonPressed()
        {
            try
            {
                await sceneView.ShowInventoryPopup();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async void OnResetInbox()
        {
            try
            {
                sceneView.SetInteractable(false);
                selectedMessageId = "";
                var desiredAudience = (JIRemoteConfigManager.SampleAudience)sceneView.audienceDropdown.value;
                JIRemoteConfigManager.instance.UpdateAudienceType(desiredAudience);
                await _cloudSaveManager.ResetCloudSaveData();
                if (this == null) return;

                await FetchUpdatedInboxData();
                if (this == null) return;

                sceneView.SetInteractable(true);
                sceneView.RefreshView();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public JICloudSaveManager GetCloudSaveManager()
        {
            return _cloudSaveManager;
        }

        public void StartFetchingNewMessagesSpinner()
        {
            sceneView.StartFetchingNewMessagesSpinner();
        }

        public void StopFetchingNewMessagesSpinner()
        {
            sceneView.StopFetchingNewMessagesSpinner();
        }
    }
}