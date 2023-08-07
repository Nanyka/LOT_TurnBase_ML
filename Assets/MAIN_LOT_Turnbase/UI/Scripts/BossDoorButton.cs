using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class BossDoorButton : MonoBehaviour, IPointerClickHandler, IConfirmFunction
    {
        [SerializeField] private string _bossScene;
        [SerializeField] private int _bossIndex;
        [SerializeField] private GameObject _newIcon;
        [SerializeField] private GameObject _lockIcon;
        private BossSelector m_BossSelector;
        private bool _isLocked;

        public void Init(bool isLocked, BossSelector bossSelector)
        {
            m_BossSelector = bossSelector;
            _isLocked = isLocked;
            _lockIcon.SetActive(_isLocked);
        }

        public void ClickYes()
        {
            if (_isLocked)
                Debug.Log("Show the message announcing this stage still be locked");
            else
            {
                Debug.Log($"Move to  boss: {_bossScene}");
                if (_bossScene.IsNullOrEmpty())
                    return;
                
                SavingSystemManager.Instance.SendBossQuestEvent(_bossIndex);
                SceneManager.LoadScene(_bossScene);
            }
        }

        public Entity GetEntity()
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_BossSelector.OpenSelectBossPanel(this);
        }
    }
}