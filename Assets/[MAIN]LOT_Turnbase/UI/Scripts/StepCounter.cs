using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class StepCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _remainStepText;

        private void Start()
        {
            MainUI.Instance.OnRemainStep.AddListener(Show);
        }

        public void Show(int info)
        {
            _remainStepText.text = $"StepRemain:{info}";
        }
    }
}
