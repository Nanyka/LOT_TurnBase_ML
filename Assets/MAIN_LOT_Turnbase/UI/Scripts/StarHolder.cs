using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.CasualGame;

namespace JumpeeIsland
{
    public class StarHolder : MonoBehaviour
    {
        [SerializeField] private bool _isUseForGuide;
        [SerializeField] private GameObject _starIcon;

        [ShowIf("@_isUseForGuide == false")] [SerializeField]
        private GameObject _starFX;

        [ShowIf("@_isUseForGuide == true")] [SerializeField]
        private Sprite _starSprite;

        [ShowIf("@_isUseForGuide == true")] [SerializeField]
        private TextMeshProUGUI _guideMessage;

        public void EnableStar()
        {
            _starIcon.SetActive(true);
            _starFX.SetActive(true);
        }

        public void EnableStar(string guideMessage, bool isCompleted)
        {
            if (_starIcon.TryGetComponent(out Image starImage) && isCompleted)
            {
                starImage.color = GameFlowManager.Instance.GameMode == GameMode.BOSS
                    ? new Color32(150, 150, 150, 255)
                    : Color.white;
                starImage.sprite = _starSprite;
            }

            _guideMessage.text = guideMessage;
        }
    }
}