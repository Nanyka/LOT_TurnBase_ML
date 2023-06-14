using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class SkinComp : MonoBehaviour
    {
        [SerializeField] private Transform m_SkinAnchor;

        private Renderer _agentRenderer;

        public void Initiate(string skinAddress)
        {
            if (skinAddress.IsNullOrEmpty())
                return;

            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor);
        }

        public void Initiate(string skinAddress, AnimateComp animateComp)
        {
            if (skinAddress.IsNullOrEmpty())
                return;

            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this, animateComp);
        }

        public void SetRenderer(Renderer renderer)
        {
            _agentRenderer = renderer;
        }

        public void SetMaterial(Material material)
        {
            if (_agentRenderer == null)
                return;

            _agentRenderer.material = material;
        }
    }
}