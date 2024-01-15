using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTutorialConfirmButton : MonoBehaviour, IConfirmFunction
    {
        [SerializeField] private string _sentString;
        
        public void ClickYes()
        {
            Debug.Log($"Click on confirmButton to send: {_sentString}");
            TimelineManager.Instance.OnStringCheck.Invoke(_sentString, ButtonRequire.NONE);
        }

        public GameObject GetGameObject()
        {
            throw new System.NotImplementedException();
        }
    }
}