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
            MainUI.Instance.OnShowCreatureInfo.AddListener(ShowUnitInfo);
        }
    
        private void ShowUnitInfo(IGetCreatureInfo infoGetter)
        {
            ShowInfo(infoGetter.InfoToShow());
        }

        private void ShowInfo((string name, int health, int damage, int power) info)
        {
            _unitInfoText.text = $"{info.name}\nHealth: {info.health}\nDamage:{info.damage}\nPower:{info.power}";
        }
    }
}
