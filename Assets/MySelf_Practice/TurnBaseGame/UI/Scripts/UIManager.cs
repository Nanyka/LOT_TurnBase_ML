using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    [HideInInspector] public UnityEvent<IGetUnitInfo> OnShowUnitInfo; // send to UIManager; invoke at Units

    [SerializeField] private UnitInfoUI m_UnitInfoUI;

    private void Start()
    {
        OnShowUnitInfo.AddListener(ShowUnitInfo);
    }

    private void ShowUnitInfo(IGetUnitInfo unitInfoGetter)
    {
        m_UnitInfoUI.ShowInfo(unitInfoGetter.GetUnitInfo());
    }
}
