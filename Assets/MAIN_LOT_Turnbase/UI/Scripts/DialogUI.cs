using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class DialogUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogLine;
        [SerializeField] private GameObject dialogBox;
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform dialogText;
        [SerializeField] private Image sideButton;

        public void ShowDialogLine(string dialog, int size, bool isSkippable)
        {
            dialogLine.text = dialog;
            // background.sizeDelta = new Vector2(size, 100);
            dialogText.sizeDelta = new Vector2(size - 100f, 80);
            sideButton.gameObject.SetActive(isSkippable);
            ToggleDialogBox(true);
        }

        public void ToggleDialogBox(bool isOn)
        {
            dialogBox.SetActive(isOn);
        }
    }
}