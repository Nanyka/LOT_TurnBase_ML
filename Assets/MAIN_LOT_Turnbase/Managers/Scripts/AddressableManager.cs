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

            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;

            var handle = Addressables.LoadAssetAsync<Sprite>(m_LogPrefab);
            return handle.WaitForCompletion();
        }
        
        public void GetAddressableGameObject(string objectKey, Transform spawnTransform)
        {
            m_LogPrefab = objectKey;

            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;

            Addressables.InstantiateAsync(m_LogPrefab, spawnTransform);
        }
        
        // Get skin for animated objects
        public void GetAddressableGameObject(string objectKey, Transform spawnTransform, SkinComp skinComp, AnimateComp animateComp)
        {
            m_LogPrefab = objectKey;

            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;

            var handle = Addressables.InstantiateAsync(m_LogPrefab, spawnTransform);
            var skin = handle.WaitForCompletion();
            animateComp.SetAnimator(skin.GetComponent<Animator>());
            //TODO: refactor renderer directory to something more general
            skinComp.SetRenderer(skin.transform.Find("Bodyparts").Find("Bodypart01").GetComponent<Renderer>());
        }

        private void AddPrivateToken(UnityWebRequest request)
        {
            string bucketAccessToken = "313a05c186c7fc63e17fc91fc00265a7"; // Add your bucket token here

            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($":{bucketAccessToken}"));

            request.SetRequestHeader("Authorization", $"Basic {encodedToken}");
        }
    }
}