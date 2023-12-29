using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class DialogUI : MonoBehaviour
    {
        [SerializeField] private Image characterIcon;
        [SerializeField] private TextMeshProUGUI dialogLine;
        [SerializeField] private GameObject dialogBox;
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform dialogText;
        [SerializeField] private Image sideButton;

        public void ShowDialogLine(string dialog, int width, int height, bool isSkippable)
        {
            dialogLine.text = dialog;
            background.sizeDelta = new Vector2(width, height);
            dialogText.sizeDelta = new Vector2(width - 100f, height - 20f);
            sideButton.gameObject.SetActive(isSkippable);
            ToggleDialogBox(true);
        }

        public void SetCharacterIcon(Sprite icon)
        {
            characterIcon.sprite = icon;
        }

        public void ToggleDialogBox(bool isOn)
        {
            dialogBox.SetActive(isOn);
        }
    }
}