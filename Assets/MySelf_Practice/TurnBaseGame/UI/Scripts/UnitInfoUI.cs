using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitInfoText;

    public void ShowInfo((string name, int health, int damage, int power) info)
    {
        _unitInfoText.text = $"{info.name}\nHealth: {info.health}\nDamage:{info.damage}\nPower:{info.power}";
    }
}