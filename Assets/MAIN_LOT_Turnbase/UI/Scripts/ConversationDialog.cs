using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class ConversationDialog : MonoBehaviour
    {
        [SerializeField] private GameObject m_Dialog;
        [SerializeField] private TextMeshProUGUI m_Message;
        private bool _isShowing;

        private void Start()
        {
            MainUI.Instance.OnConversationUI.AddListener(TurnDialog);
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

        private void TurnOffDialog()
        {
            _isShowing = false;
            m_Dialog.SetActive(false);
        }
    }
}
