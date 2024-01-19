using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeFireComp : MonoBehaviour, IFireComp
    {
        [SerializeField] private ParticleSystem[] _bulletFXs;
        [SerializeField] private float _angle;
        [SerializeField] private bool _isInPlaceFire;

        private IComboReceiver _comboReceiver;
        private Vector3 _targetPos;
        private Vector3 _velocity;

        private void Start()
        {
            _comboReceiver = GetComponent<IComboReceiver>();
        }

        public void PlayCurveFX(Vector3 targetPos)
        {
            
            var bulletFX = _bulletFXs[_comboReceiver.GetAttackIndex()];
            if (bulletFX != null)
            {
                if (_isInPlaceFire)
                {
                    bulletFX.transform.LookAt(targetPos);
                    bulletFX.Play();
                    return;
                }
                
                _targetPos = targetPos + Vector3.up*0.5f;
                var position = bulletFX.transform.position;
                _velocity = CalcBallisticVelocityVector(position, _targetPos, _angle);
                bulletFX.transform.LookAt(new Vector3(_targetPos.x, position.y, _targetPos.z));
                bulletFX.transform.Rotate(Vector3.right, -1f * _angle);
                var main = bulletFX.main;
                main.startSpeed = _velocity.magnitude;
                bulletFX.Play();
            }
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
    }

    public interface IFireComp
    {
        public void PlayCurveFX(Vector3 targetPos);
    }
}