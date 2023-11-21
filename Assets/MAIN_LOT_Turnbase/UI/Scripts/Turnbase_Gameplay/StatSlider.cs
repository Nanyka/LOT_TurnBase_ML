using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class StatSlider : MonoBehaviour
    {
        [SerializeField] private Slider statSlider;
        [SerializeField] private TextMeshProUGUI statText;

        public void SetSlider(int value)
        {
            statSlider.value = value;
            statText.text = value.ToString();
        }
    }
}