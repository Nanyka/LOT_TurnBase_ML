using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class DontMoveButton : MonoBehaviour
    {
        [SerializeField] private GameObject m_Button;
        
        private void Start()
        {
            MainUI.Instance.OnEnableInteract.AddListener(ShowButton);
        }

        private void ShowButton()
        {
            m_Button.SetActive(true);
        }

        public void OnClickButton()
        {
            MainUI.Instance.OnClickIdleButton.Invoke();
        }
    }
}
