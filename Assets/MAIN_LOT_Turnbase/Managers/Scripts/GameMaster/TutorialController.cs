using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private string m_TestFirstTutorial;

        [SerializeField] private Tutorial _currentTutorial;
        private EnvironmentManager _environmentManager;
        private int _currentStepIndex;
        [SerializeField] private EntityData _waitingFor;
        private bool _isLockAction;
        private bool _isTutorialCompleted;

        private void Start()
        {
            _environmentManager = FindObjectOfType<EnvironmentManager>();
            _environmentManager.OnChangeFaction.AddListener(StartTutorial);
            GameFlowManager.Instance.OnSelectEntity.AddListener(DetectEntity);
            
            LoadStartupTutorial();
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                StepCompletedDecide();
            }
        }

        private void LoadStartupTutorial()
        {
            _currentTutorial = AddressableManager.Instance.GetAddressableSO(m_TestFirstTutorial) as Tutorial;
        }

        #region TUTORIAL PROCESS
        
        private void StartTutorial()
        {
            if (_environmentManager.GetCurrFaction() == FactionType.Enemy)
                return;
            
            // Check condition and execute tutorial
            if (_currentTutorial.CheckExecute() == false)
                return;

            _currentStepIndex = 0;
            _isTutorialCompleted = false;

            ExecuteStep();
        }

        private void ExecuteStep()
        {
            HideAllTool();
            _waitingFor = null;
            
            var currentStep = _currentTutorial.GetStep(_currentStepIndex);
            if (currentStep.Pointer)
            {
                switch (currentStep.EntityType)
                {
                    case EntityType.PLAYER:
                        SelectPlayer();
                        break;
                }
            }
        }

        private void SelectPlayer()
        {
            _waitingFor = SavingSystemManager.Instance.GetEnvironmentData().PlayerData[0];
            ShowPointerAt(_waitingFor.Position);
        }

        private void DetectEntity(EntityData entityData)
        {
            if (_waitingFor == null)
            {
                HideAllTool();
                return;
            }

            if (_waitingFor == entityData)
                StepCompletedDecide();
        }

        private void StepCompletedDecide()
        {
            // Move to the next step if current step completed
            _currentStepIndex++;
            if (_currentTutorial.CheckRunOutOfStep(_currentStepIndex))
                Debug.Log($"Load next tutorial: {_currentTutorial.GetNextTutorial()}");
            else
                ExecuteStep();
            // If running out of step, load next tutorial
        }

        #endregion

        #region TUTORIAL TOOLS

        private void ShowPointerAt(Vector3 position)
        {
            Debug.Log($"Show pointer at {position}");
        }

        private void HideAllTool()
        {
            Debug.Log("Hide all tutorial tools");
        }

        #endregion
    }
}
