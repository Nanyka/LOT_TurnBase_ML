using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class SkillIcon : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private GameObject _lockIcon;

        private Color32 _deactivateColor = new(57,57,57,255);

        public void Active()
        {
            m_Icon.color = Color.white;
        }

        public void Deactivate()
        {
            m_Icon.color = _deactivateColor;
        }

        public void LockState(bool isLock)
        {
            _lockIcon.SetActive(isLock);
        }
    }
}