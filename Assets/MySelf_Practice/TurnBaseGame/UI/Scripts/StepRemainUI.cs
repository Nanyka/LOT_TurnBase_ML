using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepRemainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _remainStepText;
    
    public void Show(int info)
    {
        _remainStepText.text = $"StepRemain:{info}";
    }
}
