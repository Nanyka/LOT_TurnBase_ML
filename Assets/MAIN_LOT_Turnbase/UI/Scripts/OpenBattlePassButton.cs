using UnityEngine;

namespace JumpeeIsland
{
    public class OpenBattlePassButton : MonoBehaviour
    {
        [SerializeField] private GameObject _battlePassMenu;

        public void OnOpenBattlePass()
        {
            _battlePassMenu.SetActive(true);
            GameFlowManager.Instance.OnOpenBattlePass.Invoke();
        }
    }
}