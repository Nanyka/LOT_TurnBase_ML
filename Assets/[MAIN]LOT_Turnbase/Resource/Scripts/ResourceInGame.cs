using UnityEngine;

namespace LOT_Turnbase
{
    public class ResourceInGame : MonoBehaviour
    {
        [Header("Resource definition")] 
        [SerializeField] private ResourceType m_ResourceType;
        [SerializeField] private CurrencyType m_CurrencyType;
        [SerializeField] private int m_Level;

        public Vector3 Position { get; private set; }
        private int _currentHealth;
        private int _currentTurn;

        public void Init()
        {
            Debug.Log("Initiate this resource at " + Position);
        }

        #region SET

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        #endregion
    }
}