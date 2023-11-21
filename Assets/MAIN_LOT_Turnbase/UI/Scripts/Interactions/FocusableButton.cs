using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class FocusableButton : MonoBehaviour
    {
        [SerializeField] private RectTransform _creatureButton;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private GameObject _focusFrame;
        [SerializeField] private Sprite _defaultButton;
        [SerializeField] private Sprite _activeButton;
        
        private Vector3 _originalScale;
        protected bool _isActive;
        
        protected virtual void Start()
        {
            MainUI.Instance.OnHideAllMenu.AddListener(DeactiveButton);
            _originalScale = _creatureButton.localScale;
        }
        
        protected void ActiveButton()
        {
            if (_isActive == false)
                return;
            
            _creatureButton.localScale = _originalScale * 1.2f;
            _buttonImage.sprite = _activeButton;
            _focusFrame.SetActive(true);
        }
        
        private void DeactiveButton()
        {
            _creatureButton.localScale = _originalScale;
            _buttonImage.sprite = _defaultButton;
            _focusFrame.SetActive(false);
            _isActive = false;
        }
    }
}