using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialController : MonoBehaviour, ITutorialControl
    {
        public void Init(string currentTutorial)
        {
            Debug.Log($"TODO: Load tutorial {currentTutorial} and load timeline base on the current tutorial");
        }
    }

    public interface ITutorialControl
    {
        public void Init(string currentTutorial);
    }
}