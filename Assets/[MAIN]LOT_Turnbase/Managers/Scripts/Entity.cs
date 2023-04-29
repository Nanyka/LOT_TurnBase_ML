using System;
using UnityEngine;
using UnityEngine.Events;

namespace LOT_Turnbase
{
    public abstract class Entity: MonoBehaviour
    {
        [NonSerialized] public UnityEvent OnUnitDie = new(); 
        
        [Header("Default components")] 
        [SerializeField] protected Transform m_Transform;
        [SerializeField] protected Animator m_Animator;
    }
}