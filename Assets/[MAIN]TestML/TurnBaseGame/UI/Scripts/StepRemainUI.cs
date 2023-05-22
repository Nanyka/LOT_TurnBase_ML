using System;
using System.Collections;
using System.Collections.Generic;
using LOT_Turnbase;
using TMPro;
using UnityEngine;

public class StepRemainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _remainStepText;

    private void Start()
    {
        UIManager.Instance.OnRemainStep.AddListener(Show);
    }

    public void Show(int info)
    {
        _remainStepText.text = $"StepRemain:{info}";
    }
}
