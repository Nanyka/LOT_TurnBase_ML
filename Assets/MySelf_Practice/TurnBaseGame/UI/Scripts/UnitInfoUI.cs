using System;
using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitInfoText;

    private void Start()
    {
        UIManager.Instance.OnShowUnitInfo.AddListener(ShowUnitInfo);
    }
    
    private void ShowUnitInfo(IGetUnitInfo unitInfoGetter)
    {
        ShowInfo(unitInfoGetter.GetUnitInfo());
    }

    private void ShowInfo((string name, int health, int damage, int power) info)
    {
        _unitInfoText.text = $"{info.name}\nHealth: {info.health}\nDamage:{info.damage}\nPower:{info.power}";
    }
}