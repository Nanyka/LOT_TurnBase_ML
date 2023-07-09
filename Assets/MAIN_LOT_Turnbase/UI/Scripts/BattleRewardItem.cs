using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class BattleRewardItem : MonoBehaviour
    {
        [SerializeField] private GameObject _itemContainer;
        [SerializeField] private Image _itemBG;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmount;
        [SerializeField] private GameObject _rewardFX;
        [SerializeField] private Sprite _basicReward;
        [SerializeField] private Sprite _specialReward;

        public void ShowReward(string iconAddress, string rewardAmount, bool isSpecial)
        {
            _itemBG.sprite = isSpecial ? _specialReward : _basicReward;
            _rewardFX.SetActive(isSpecial);
            _rewardIcon.sprite = AddressableManager.Instance.GetAddressableSprite(iconAddress);
            _rewardAmount.text = rewardAmount;
            _itemContainer.SetActive(true);
        }
    }
}