using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using TMPro;
using UnityEngine;

namespace LOT_Turnbase
{
    public class CreatureInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _unitInfoText;

        private void Start()
        {
            MainUI.Instance.OnShowCreatureInfo.AddListener(ShowUnitInfo);
        }
    
        private void ShowUnitInfo(IGetCreatureInfo unitInfoGetter)
        {
            ShowInfo(unitInfoGetter.GetCreatureInfo());
        }

        private void ShowInfo((string name, int health, int damage, int power) info)
        {
            _unitInfoText.text = $"{info.name}\nHealth: {info.health}\nDamage:{info.damage}\nPower:{info.power}";
        }
    }
}
