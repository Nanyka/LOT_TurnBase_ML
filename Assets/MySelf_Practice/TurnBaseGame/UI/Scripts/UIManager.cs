using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    [HideInInspector] public UnityEvent<IGetUnitInfo> OnShowUnitInfo; // send to UIManager; invoke at Units
    [HideInInspector] public UnityEvent<int> OnRemainStep; // send to UIManager; invoke at UnitManagers
    [HideInInspector] public UnityEvent OnClickIdleButton; //send to PlayerFactionManager; invoke at IdleButton & MovingPath

    [SerializeField] private UnitInfoUI m_UnitInfoUI;
    [SerializeField] private StepRemainUI m_StepRemainUI;

    private void Start()
    {
        OnShowUnitInfo.AddListener(ShowUnitInfo);
        OnRemainStep.AddListener(ShowRemainStep);
    }

    private void ShowRemainStep(int remainStep)
    {
        m_StepRemainUI.Show(remainStep);
    }

    private void ShowUnitInfo(IGetUnitInfo unitInfoGetter)
    {
        m_UnitInfoUI.ShowInfo(unitInfoGetter.GetUnitInfo());
    }
}
