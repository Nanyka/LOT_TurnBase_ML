using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class SkinComp : MonoBehaviour
    {
        [SerializeField] private Transform m_SkinAnchor;
        [SerializeField] private Mesh m_BodyMesh;

        [SerializeField] private Renderer _factionRenderer;

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

        public void ModifyBodyMesh(SkinnedMeshRenderer renderer)
        {
            if (renderer == null)
                return;
            
            renderer.sharedMesh = m_BodyMesh;
        }

        public void SetFactionRenderer(Renderer renderer)
        {
            if (renderer == null)
                return;
            
            _factionRenderer = renderer;
        }

        public void SetMaterial(Material material)
        {
            if (_factionRenderer == null)
                return;

            _factionRenderer.material = material;
        }
    }
}