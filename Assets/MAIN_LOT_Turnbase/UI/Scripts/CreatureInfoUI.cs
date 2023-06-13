using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CreatureInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _unitInfoText;

        private void Start()
        {
            MainUI.Instance.OnShowInfo.AddListener(ShowUnitInfo);
        }
    
        private void ShowUnitInfo(IShowInfo infoGetter)
        {
            _unitInfoText.text = infoGetter.ShowInfo();
        }
    }
}
