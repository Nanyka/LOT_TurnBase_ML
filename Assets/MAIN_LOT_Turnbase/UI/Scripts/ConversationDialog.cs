using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class ConversationDialog : MonoBehaviour
    {
        [SerializeField] private GameObject m_Dialog;
        [SerializeField] private TextMeshProUGUI m_Message;
        [SerializeField] private Image m_TutorialImage;
        private bool _isShowing;

        private void Start()
        {
            MainUI.Instance.OnConversationUI.AddListener(TurnDialog);
            MainUI.Instance.OnImageTutorial.AddListener(ImageTutorial);
        }

        private void TurnDialog(string message, bool showDialog)
        {
            m_Message.text = message;
            m_Dialog.SetActive(showDialog);
            
            if (_isShowing == false)
            {
                _isShowing = true;
                Invoke(nameof(TurnOffDialog),5f);
            }
        }

        private void ImageTutorial(string message, string imageAddress, bool showDialog)
        {
            m_TutorialImage.sprite = AddressableManager.Instance.GetAddressableSprite(imageAddress);
            m_TutorialImage.gameObject.SetActive(true);
            TurnDialog(message,showDialog);
        }

        private void TurnOffDialog()
        {
            _isShowing = false;
            m_TutorialImage.gameObject.SetActive(false);
            m_Dialog.SetActive(false);
        }
    }
}
