using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class SkinComp : MonoBehaviour
    {
        [SerializeField] private Transform m_SkinAnchor;

        public void Initiate(string skinAddress)
        {
            if (skinAddress.IsNullOrEmpty())
                return;
            
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor);
        }
    }
}