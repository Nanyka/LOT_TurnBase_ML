using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class AddressableManager : Singleton<AddressableManager>
    {
        private string m_LogPrefab;

        public Sprite GetAddressableSprite(string objectKey)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            var handle = Addressables.LoadAssetAsync<Sprite>(m_LogPrefab);
            return handle.WaitForCompletion();
        }

        public void GetAddressableGameObject(string objectKey, Transform spawnTransform)
        {
            m_LogPrefab = objectKey;
            
#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            Addressables.InstantiateAsync(m_LogPrefab, spawnTransform);
        }

        // Get skin for animated objects
        public void GetAddressableGameObject(string objectKey, Transform spawnTransform, SkinComp skinComp,
            AnimateComp animateComp)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            var handle = Addressables.InstantiateAsync(m_LogPrefab, spawnTransform);
            var skin = handle.WaitForCompletion();
            animateComp.SetAnimator(skin.GetComponent<Animator>());

            var bodyRenderer = skin.transform.Find("Body");
            if (bodyRenderer != null)
                if (bodyRenderer.TryGetComponent(out SkinnedMeshRenderer body))
                    skinComp.ModifyBodyMesh(body);

            var factionPart = skin.transform.Find("FactionPart");
            if (factionPart != null)
                if (factionPart.TryGetComponent(out Renderer part))
                    skinComp.SetFactionRenderer(part);
        }

        private void AddPrivateToken(UnityWebRequest request)
        {
            string bucketAccessToken = "313a05c186c7fc63e17fc91fc00265a7"; // Add your bucket token here

#if UNITY_STANDALONE_OSX
  bucketAccessToken = "313a05c186c7fc63e17fc91fc00265a7";
#endif

            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($":{bucketAccessToken}"));

            request.SetRequestHeader("Authorization", $"Basic {encodedToken}");
        }
    }
}