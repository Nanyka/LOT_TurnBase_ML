using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class EndGameComp : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<Entity>().OnUnitDie.AddListener(EndGame);
        }

        private void EndGame(Entity arg0)
        {
            GameFlowManager.Instance.OnGameOver.Invoke(2000);
        }
    }
}