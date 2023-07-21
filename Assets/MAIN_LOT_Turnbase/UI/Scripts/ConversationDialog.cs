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

        private void Start()
        {
            MainUI.Instance.OnConversationUI.AddListener(TurnDialog);
        }

        private void TurnDialog(string message, bool showDialog)
        {
            m_Message.text = message;
            m_Dialog.SetActive(showDialog);
        }
    }
}
