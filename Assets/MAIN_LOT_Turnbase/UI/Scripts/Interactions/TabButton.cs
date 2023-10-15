using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private GameObject m_Focus;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private Image m_Icon;
        [SerializeField] private GameObject _mainContent;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color32 deactiveColor = new Color32( 74, 172, 247, 255 );

        public void OnActiveTab()
        {
            if (m_Title != null)
                m_Title.color = Color.white;

            if (m_Icon != null)
            {
                m_Icon.color = activeColor;
                m_Icon.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            
            m_Focus.SetActive(true);
            _mainContent.SetActive(true);
        }

        public void OnDeactiveTab()
        {
            m_Focus.SetActive(false);
            _mainContent.SetActive(false);
            
            if (m_Title != null)
                m_Title.color = deactiveColor;
            
            if (m_Icon != null)
            {
                m_Icon.color = deactiveColor;
                m_Icon.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}