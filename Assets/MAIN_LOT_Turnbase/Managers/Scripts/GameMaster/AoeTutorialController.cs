using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialController : MonoBehaviour, ITutorialControl
    {
        public void Init(string currentTutorial)
        {
            Debug.Log($"Load tutorial {currentTutorial}");
        }
    }

    public interface ITutorialControl
    {
        public void Init(string currentTutorial);
    }
}