using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    public class FireComp : MonoBehaviour, IFireComp
    {
        [SerializeField] private ParticleSystem _bulletFX;
        [SerializeField] private float _angle;
        [SerializeField] private int _reloadDuration;
        [SerializeField] private bool _isInPlaceFire;

        private Vector3 _targetPos;
        private Vector3 _velocity;

        public void PlayCurveFX(Vector3 targetPos)
        {
            if (_isInPlaceFire)
            {
                PlayerPointFX();
                return;
            }

            if (_bulletFX != null)
            {
                _targetPos = targetPos + Vector3.up * 0.5f;
                Fire();
            }
        }

        public void PlayCurveFX(IEnumerable<Vector3> targetPos)
        {
            if (_isInPlaceFire)
            {
                PlayerPointFX();
                return;
            }

            if (_bulletFX != null)
            {
                foreach (var target in targetPos)
                {
                    _targetPos = target;
                    Fire();
                }
            }
        }

        public void PlayCurveFX(IEnumerable<Vector3> targetPos, AttackVisual attackVisual)
        {
            if (_isInPlaceFire)
            {
                PlayerPointFX();
                return;
            }

            if (_bulletFX != null)
            {
                foreach (var target in targetPos)
                {
                    attackVisual.RotateTowardTarget(target);
                    Fire();
                }
            }
        }

        private void PlayerPointFX()
        {
            if (_bulletFX != null)
                _bulletFX.Play();
        }

        private void Fire()
        {
            var position = _bulletFX.transform.position;
            _velocity = CalcBallisticVelocityVector(position, _targetPos, _angle);
            _bulletFX.transform.LookAt(new Vector3(_targetPos.x, position.y, _targetPos.z));
            _bulletFX.transform.Rotate(Vector3.right, -1f * _angle);
            var main = _bulletFX.main;
            main.startSpeed = _velocity.magnitude;
            _bulletFX.Play();
        }

        private Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
        {
            Vector3 direction = target - source;
            if (direction.magnitude < 1.5f)
            {
                return transform.forward * 3f;
            }

            float h = direction.y;
            direction.y = 0;
            float distance = direction.magnitude;
            float a = angle * Mathf.Deg2Rad;
            direction.y = distance * Mathf.Tan(a);
            distance += h / Mathf.Tan(a);

            // calculate velocity
            float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            return velocity * direction.normalized;
        }

        public int GetReloadDuration()
        {
            return _reloadDuration;
        }

        public void SetBulletFX(ParticleSystem bulletFX)
        {
            _bulletFX = bulletFX;
        }
    }
}