using System;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class AttackMovableObj : GAction
    {
        [SerializeField] private CharacterEntity m_Character;
        [SerializeField] private float _checkDistance = 1f;

        private ISensor _detectPlayerTroop;

        private void Start()
        {
            _detectPlayerTroop = GetComponent<ISensor>();
        }

        public override bool PrePerform()
        {
            var target = _detectPlayerTroop.ExecuteSensor();
            if (target == null)
                return false;

            var position = target.transform.position;
            m_Character.transform.LookAt(new Vector3(position.x, m_Character.transform.position.y, position.z));
            m_Character.StartAttack();

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}