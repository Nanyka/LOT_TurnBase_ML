using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class CurrencyButton : MonoBehaviour
    {
        public Image m_Icon;
        public TextMeshProUGUI m_Balance;
        public string m_Currency;
        public GameObject m_badge;

        private bool _isInitiated;

        public void UpdateCurrency(string balance, string iconAddress)
        {
            if (_isInitiated == false)
                m_Icon.sprite = AddressableManager.Instance.GetAddressableSprite(iconAddress);

            m_Balance.text = balance;
        }
    }
}
