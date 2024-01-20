using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerEffectComp : MonoBehaviour, IEffectComp
    {
        [SerializeField] private ParticleSystem _frozenVfx;
    
        private Entity m_Entity;
        private IAnimateComp m_AnimateComp;
        private Coroutine _frozenCoroutine;
        private int _currentDuration;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void Init(Entity entity)
        {
            m_Entity = entity;
            m_AnimateComp = GetComponent<IAnimateComp>();
        }

        public bool CheckSkipTurn()
        {
            throw new System.NotImplementedException();
        }

        public int GetJumpBoost()
        {
            throw new System.NotImplementedException();
        }

        public bool UseJumpBoost()
        {
            throw new System.NotImplementedException();
        }

        public void EffectCountDown()
        {
            throw new System.NotImplementedException();
        }

        public void JumpBoost(int duration, int magnitude)
        {
            throw new System.NotImplementedException();
        }

        public void AdjustStrength(int magnitude, int duration)
        {
            throw new System.NotImplementedException();
        }

        public void RecordFrozen(int duration, Material effectMaterial)
        {
            _currentDuration = duration;
            m_Entity.GetSkin().SetCustomMaterial(effectMaterial);
            m_AnimateComp.SetAnimatorSpeed(0f);
            _frozenVfx.Play();

            if (_frozenCoroutine != null)
                StopCoroutine(_frozenCoroutine);
            _frozenCoroutine = StartCoroutine(WaitForUnFroze());
        }

        private IEnumerator WaitForUnFroze()
        {
            yield return new WaitForSeconds(_currentDuration);
            m_Entity.GetSkin().SetDefaultMaterial();
            m_AnimateComp.SetAnimatorSpeed(1f);
        }
    }
}