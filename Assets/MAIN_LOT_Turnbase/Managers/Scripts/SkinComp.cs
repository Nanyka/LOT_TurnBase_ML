using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class SkinComp : MonoBehaviour
    {
        [SerializeField] private Transform m_SkinAnchor;
        [SerializeField] private List<Renderer> _factionRenderers = new();
        [SerializeField] private Material _activeMaterial;
        [SerializeField] private Material _disableMaterial;

        public void Init(string skinAddress)
        {
            if (skinAddress.IsNullOrEmpty())
                return;

            _factionRenderers.Clear();
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor);
        }

        public void Init(string skinAddress, AnimateComp animateComp)
        {
            if (skinAddress.IsNullOrEmpty())
                return;

            _factionRenderers.Clear();
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this, animateComp);
        }

        public void SetFactionRenderer(Renderer renderer)
        {
            _factionRenderers.Add(renderer);
        }

        public void SetActiveMaterial()
        {
            if (_factionRenderers == null || _activeMaterial == null)
                return;

            foreach (var item in _factionRenderers)
                item.material = _activeMaterial;
        }

        public void SetDisableMaterial()
        {
            if (_factionRenderers == null || _disableMaterial == null)
                return;

            foreach (var item in _factionRenderers)
                item.material = _disableMaterial;
        }
    }
}