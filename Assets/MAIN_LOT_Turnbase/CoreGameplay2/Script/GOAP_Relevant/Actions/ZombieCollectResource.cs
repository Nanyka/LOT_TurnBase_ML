using System.Threading.Tasks;
using GOAP;
using UnityEngine;

namespace JumpeeIsland
{
    public class ZombieCollectResource : GAction
    {
        [SerializeField] private AoeZombieEntity m_Character;
        [SerializeField] private float _checkDistance = 2f;

        private ICheckableObject _currentPoint;
        private ISensor _sensor;
        
        private void Start()
        {
            _sensor = GetComponent<ISensor>();
        }

        public override bool PrePerform()
        {
            var target = _sensor.ExecuteSensor();

            if (target == null)
                return false;

            var position = target.transform.position;
            if (Vector3.Distance(transform.position, position) > _checkDistance)
                return false;

            m_Character.transform.LookAt(new Vector3(position.x, m_Character.transform.position.y, position.z));
            m_Character.StartHarvest();

            return true;
        }

        public override bool PostPerform()
        {
            return true;
        }
    }
}