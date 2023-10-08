using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Samples;
using Unity.Services.Samples.InGameMailbox;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class MailBoxView : MonoBehaviour
    {
        public GameObject MailBoxContainer;
        
        [FormerlySerializedAs("sceneManager")] [Space]
        public MailBoxPanelManager mailboxManager;

        [Header("Message List")]
        public GameObject messageListItemPrefab;
        public Transform messageListContainer;

        [Header("Message Detail")]
        public MessageView messageView;
        public Button claimAttachmentButton;
        public Button deleteMessageButton;

        [Header("Inbox Status")]
        public TextMeshProUGUI messageCountText;
        public TextMeshProUGUI messageFullAlert;
        public GameObject fetchingNewMessagesSpinner;
        public Color inboxDefaultTextColor;
        public Color inboxFullTextColor;

        [Header("Inbox Action Buttons")]
        public Button deleteAllReadButton;
        public Button claimAllButton;

        [Header("Popups")]
        public RewardPopupView claimAllSucceededPopup;
        public MessagePopup messageListEmptyPopup;
        public MessagePopup claimFailedPopup;

        [Header("Inventory View")]
        public Button inventoryButton;
        public InventoryPopupView inventoryPopupView;

        [Header("Inbox Debug Game Objects")]
        public TMP_Dropdown audienceDropdown;
        public Button resetInboxButton;

        private bool m_IsSceneInteractable;
        private bool m_IsFetchingNewMessages;
        List<Button> m_SceneButtons;
        List<MessageListItemView> m_MessageListItems = new List<MessageListItemView>();

        public void Init()
        {
            MailBoxContainer.SetActive(true);
            
            m_SceneButtons = new List<Button>
            {
                deleteAllReadButton,
                claimAllButton,
                claimAttachmentButton,
                deleteMessageButton,
                inventoryButton,
                resetInboxButton
            };
            
            SetInteractable(true);

            messageFullAlert.color = inboxFullTextColor;

            InitializeMessageListItemViews();
            RefreshView();
        }

        public void SetInteractable(bool isInteractable)
        {
            m_IsSceneInteractable = isInteractable;

            foreach (var button in m_SceneButtons)
            {
                SetButtonInteractable(button, m_IsSceneInteractable);
            }

            audienceDropdown.interactable = isInteractable;
        }

        void SetButtonInteractable(Button button, bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable && m_IsSceneInteractable;
            }
        }

        void InitializeMessageListItemViews()
        {
            if (messageListItemPrefab != null && messageListContainer != null)
            {
                for (var i = 0; i < mailboxManager.maxInboxSize; i++)
                {
                    var messageListItem = Instantiate(messageListItemPrefab, messageListContainer);
                    var view = messageListItem.GetComponent<MessageListItemView>();
                    view.messageSelected += OnMessageListItemSelected;

                    messageListItem.SetActive(false);
                    m_MessageListItems.Add(view);
                }
            }
        }

        public void RefreshView()
        {
            ClearOpenMessageViewContainer();

            if (messageListEmptyPopup != null && mailboxManager.GetCloudSaveManager().inboxMessages.Count == 0)
            {
                messageListEmptyPopup.Show();
            }

            var enableDeleteAllReadButton = false;
            var enableClaimAllButton = false;

            for (var i = 0; i < mailboxManager.maxInboxSize; i++)
            {
                if (mailboxManager.GetCloudSaveManager().inboxMessages.Count > i)
                {
                    var message = mailboxManager.GetCloudSaveManager().inboxMessages[i];

                    SetMessageListItemData(m_MessageListItems[i], message);
                    ShowMessageDetailViewIfSelected(message);

                    if (message.metadata.isRead && !message.metadata.hasUnclaimedAttachment)
                    {
                        enableDeleteAllReadButton = true;
                    }

                    if (message.metadata.hasUnclaimedAttachment)
                    {
                        enableClaimAllButton = true;
                    }
                }
                else
                {
                    m_MessageListItems[i].gameObject.SetActive(false);
                }
            }

            SetButtonInteractable(deleteAllReadButton, enableDeleteAllReadButton);
            SetButtonInteractable(claimAllButton, enableClaimAllButton);
            UpdateInboxCounts();
        }

        void ClearOpenMessageViewContainer()
        {
            messageView.SetData(null);
        }

        void SetMessageListItemData(MessageListItemView view, InboxMessage message)
        {
            view.SetData(message, isCurrentlySelected: string.Equals(message.messageId, mailboxManager.selectedMessageId));
            view.gameObject.SetActive(true);
        }

        void ShowMessageDetailViewIfSelected(InboxMessage message)
        {
            if (string.Equals(message.messageId, mailboxManager.selectedMessageId))
            {
                messageView.SetData(message);
            }
        }

        void UpdateInboxCounts()
        {
            // TODO: Adjust this

            if (messageCountText != null)
            {
                messageCountText.text = $"{JICloudSaveManager.instance.inboxMessages.Count} / {mailboxManager.maxInboxSize}";
            }

            if (JICloudSaveManager.instance.inboxMessages.Count == mailboxManager.maxInboxSize)
            {
                messageCountText.color = inboxFullTextColor;
                messageFullAlert.gameObject.SetActive(!m_IsFetchingNewMessages);
            }
            else
            {
                messageCountText.color = inboxDefaultTextColor;
                messageFullAlert.gameObject.SetActive(false);
            }
        }

        public void StartFetchingNewMessagesSpinner()
        {
            if (fetchingNewMessagesSpinner == null || messageFullAlert == null)
            {
                return;
            }

            m_IsFetchingNewMessages = true;
            fetchingNewMessagesSpinner.SetActive(true);
            messageFullAlert.gameObject.SetActive(false);
        }

        public async void StopFetchingNewMessagesSpinner()
        {
            try
            {
                if (fetchingNewMessagesSpinner == null || messageFullAlert == null)
                {
                    return;
                }

                // Want to ensure that even if the spinner is started and stopped very quickly, that it appears
                // long enough that it is still visible to the user. So we'll delay the stopping for one second.
                await Task.Delay(1000);
                if (this == null) return;

                m_IsFetchingNewMessages = false;
                fetchingNewMessagesSpinner.SetActive(false);
                messageFullAlert.gameObject.SetActive(JICloudSaveManager.instance.inboxMessages.Count ==
                    mailboxManager.maxInboxSize);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void OnMessageListItemSelected(InboxMessage selectedMessage)
        {
            mailboxManager.SelectMessage(selectedMessage.messageId);
            RefreshView();
        }

        public async Task ShowInventoryPopup()
        {
            await inventoryPopupView.Show();
        }

        public void ShowClaimAllSucceededPopup(List<RewardDetail> rewards)
        {
            claimAllSucceededPopup.Show(rewards);
        }

        public void ShowClaimFailedPopup(string title, string message)
        {
            claimFailedPopup.Show(title, message);
        }
    }
}