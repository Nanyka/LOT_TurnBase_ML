using UnityEngine;

namespace JumpeeIsland
{
    // Custom comparer to sort Vector3 objects by distance to the target point


    public class FireComp : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _bulletFX;
        [SerializeField] private Vector3 _targetPos;
        [SerializeField] private float _angle;
        
        private Vector3 _velocity;
        
        public void PlayCurveFX()
        {
            if (_bulletFX != null)
            {
                var position = _bulletFX.transform.position;
                _velocity = CalcBallisticVelocityVector(position, _targetPos, _angle);
                _bulletFX.transform.LookAt(new Vector3(_targetPos.x, position.y, _targetPos.z));
                _bulletFX.transform.Rotate(Vector3.right, -1f * _angle);
                var main = _bulletFX.main;
                main.startSpeed = _velocity.magnitude;
                _bulletFX.Play();
            }
        }
        
        public void PlayCurveFX(Vector3 targetPos)
        {
            if (_bulletFX != null)
            {
                var position = _bulletFX.transform.position;
                _velocity = CalcBallisticVelocityVector(position, targetPos, _angle);
                _bulletFX.transform.LookAt(new Vector3(targetPos.x, position.y, targetPos.z));
                _bulletFX.transform.Rotate(Vector3.right, -1f * _angle);
                var main = _bulletFX.main;
                main.startSpeed = _velocity.magnitude;
                _bulletFX.Play();
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
}