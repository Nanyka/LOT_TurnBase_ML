using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class HarvestComp : MonoBehaviour
    {
        public int CurrentStock { get; set; }

        private CharacterEntity m_Character;

        private void Start()
        {
            m_Character = GetComponent<CharacterEntity>();
        }

        public void GetStrength()
        {
            
        }
    }
}