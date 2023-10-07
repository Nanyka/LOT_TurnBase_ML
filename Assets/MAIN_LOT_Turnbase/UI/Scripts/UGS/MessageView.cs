using System.Collections.Generic;
using TMPro;
using Unity.Services.Samples;
using Unity.Services.Samples.InGameMailbox;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class MessageView : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI contentText;
        public RewardDisplayView rewardDisplayView;
        public Button claimAttachmentButton;
        public Color rewardItemViewClaimedColor;

        InboxMessage m_Message;
        string m_Title;
        string m_Content;
        bool m_HasAttachment;
        bool m_HasUnclaimedAttachment;
        List<RewardDetail> m_RewardDetails = new();

        public void SetData(InboxMessage message)
        {
            m_Message = message;

            m_Title = message?.messageInfo?.title ?? "";
            m_Content = message?.messageInfo?.content ?? "";
            m_HasAttachment = !string.IsNullOrEmpty(message?.messageInfo?.attachment);
            m_HasUnclaimedAttachment = message?.metadata?.hasUnclaimedAttachment ?? false;

            if (message != null && message.messageInfo != null)
            {
                GetRewardDetails(message.messageInfo.attachment);
            }

            UpdateView();
        }

        void GetRewardDetails(string virtualPurchaseId)
        {
            m_RewardDetails.Clear();

            if (JIEconomyManager.instance.virtualPurchaseTransactions.TryGetValue(virtualPurchaseId, out var virtualPurchase))
            {
                foreach (var reward in virtualPurchase.rewards)
                {
                    m_RewardDetails.Add(new RewardDetail
                    {
                        id = reward.id,
                        quantity = reward.amount,
                        sprite = AddressableManager.Instance.GetAddressableSprite(
                            SavingSystemManager.Instance.GetCurrencySprite(reward.id))
                    });
                }
            }
        }

        void UpdateView()
        {
            gameObject.SetActive(m_Message != null);

            if (titleText != null)
            {
                titleText.text = m_Title;
            }

            if (contentText != null)
            {
                contentText.text = m_Content;
            }

            UpdateAttachmentViewAndClaimButton();
        }

        void UpdateAttachmentViewAndClaimButton()
        {
            if (m_HasAttachment)
            {
                if (rewardDisplayView != null)
                {
                    // The message only has a claimed attachment if it both has an attachment, and that
                    // attachment is not unclaimed.
                    if (m_HasAttachment && !m_HasUnclaimedAttachment)
                    {
                        rewardDisplayView.PopulateView(m_RewardDetails, rewardItemViewClaimedColor);
                    }
                    else
                    {
                        rewardDisplayView.PopulateView(m_RewardDetails);
                    }

                    rewardDisplayView.gameObject.SetActive(true);
                }

                if (claimAttachmentButton != null)
                {
                    claimAttachmentButton.gameObject.SetActive(true);
                    claimAttachmentButton.interactable = m_HasUnclaimedAttachment;
                }
            }
            else
            {
                if (rewardDisplayView != null)
                {
                    rewardDisplayView.gameObject.SetActive(false);
                }

                if (claimAttachmentButton != null)
                {
                    claimAttachmentButton.gameObject.SetActive(false);
                }
            }
        }

        public async void OnClaimReward()
        {
            foreach (var rewardDetail in m_RewardDetails)
            {
                Debug.Log($"TODO: Add to local currency {rewardDetail.id}: {rewardDetail.quantity}");
                m_Message.metadata.hasUnclaimedAttachment = false;
                m_HasUnclaimedAttachment = m_Message?.metadata?.hasUnclaimedAttachment ?? false;
                UpdateView();
                
                await JICloudSaveManager.instance.SavePlayerInboxInCloudSave();
            }
        }
    }
}