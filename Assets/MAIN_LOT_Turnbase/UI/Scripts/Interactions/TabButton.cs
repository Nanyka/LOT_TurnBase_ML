using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private GameObject m_Focus;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private GameObject _mainContent;
        
        private Color32 deactiveColor = new Color32( 74, 172, 247, 255 );

        public void OnActiveTab()
        {
            m_Focus.SetActive(true);
            _mainContent.SetActive(true);
            m_Title.color = Color.white;
        }

        public void OnDeactiveTab()
        {
            m_Focus.SetActive(false);
            _mainContent.SetActive(false);
            m_Title.color = deactiveColor;
        }
    }
}