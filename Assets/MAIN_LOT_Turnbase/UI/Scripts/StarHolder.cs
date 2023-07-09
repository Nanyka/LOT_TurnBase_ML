using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions.CasualGame;

namespace JumpeeIsland
{
    public class StarHolder : MonoBehaviour
    {
        [SerializeField] private GameObject _starIcon;
        [SerializeField] private GameObject _starFX;

        public void EnableStar()
        {
            _starIcon.SetActive(true);
            _starFX.SetActive(true);
        }
    }
}
