using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class SkinComp : MonoBehaviour, ISkinComp
    {
        [SerializeField] private Transform m_SkinAnchor;
        [SerializeField] private ParticleSystem _appearVfx;
        [SerializeField] private List<Renderer> _factionRenderers = new();
        [SerializeField] private Material _activeMaterial;
        [SerializeField] private Material _disableMaterial;

        private Material _activeCache;
        private Material _disableCache;
        private IAnimateComp _curAnimComp;

        public void Init(string skinAddress)
        {
            if (skinAddress.IsNullOrEmpty())
                return;

            _factionRenderers.Clear();
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor);
            
            ResetMaterial();
        }

        public void Init(string skinAddress, IAnimateComp animateComp)
        {
            if (skinAddress.IsNullOrEmpty())
                return;
            
            // Reset spawnTransform
            for (int i = 0; i < m_SkinAnchor.childCount; i++)
                Destroy(m_SkinAnchor.GetChild(i).gameObject);

            _factionRenderers.Clear();
            _curAnimComp = animateComp;
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this);
        }

        public void SetActiveMaterial()
        {
            if (_factionRenderers == null || _activeCache == null)
                return;

            SetMaterial(_activeCache);
        }

        public void SetDisableMaterial()
        {
            if (_factionRenderers == null || _disableCache == null)
                return;

            SetMaterial(_disableCache);
        }
        
        public void SetCustomMaterial(Material material)
        {
            _activeCache = material;
            _disableMaterial = material;
        }

        public void ReturnSkin(GameObject skinObject)
        {
            _curAnimComp.Init(skinObject);
        }

        private void SetMaterial(Material material)
        {
            foreach (var item in _factionRenderers)
                item.material = material;
        }

        private void ResetMaterial()
        {
            _activeCache = _activeMaterial;
            _disableCache = _disableMaterial;
        }
    }

    public interface ISkinComp
    {
        public void Init(string skinAddress);
        public void Init(string skinAddress, IAnimateComp animateComp);
        public void SetActiveMaterial();
        public void SetCustomMaterial(Material material);
        public void ReturnSkin(GameObject skinObject);
    }
}