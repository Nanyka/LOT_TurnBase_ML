using UnityEngine;

namespace GOAP
{
    public class GStateMonitor : MonoBehaviour
    {
        public string State;
        public float StateStrength;
        public float StateDecayRate;
        public WorldStates Beliefs;
        public GameObject ResourcePrefab;
        public string QueueName;
        public string WorldState;
        public GAction Action;
    
        private bool stateFound = false;
        private float initialStrength;
    
        private void Awake()
        {
            Beliefs = GetComponent<GAgent>().Beliefs;
            initialStrength = StateStrength;
        }
    
        private void LateUpdate()
        {
            if (Action.running)
            {
                stateFound = false;
                StateStrength = initialStrength;
            }
    
            if (!stateFound && Beliefs.HasState(State))
                stateFound = true;
    
            if (stateFound)
            {
                StateStrength -= StateDecayRate * Time.deltaTime;
                if (StateStrength <= 0)
                {
                    Vector3 location = new Vector3(transform.position.x, ResourcePrefab.transform.position.y,
                        transform.position.z);
                    GameObject puddle = GameObject.Instantiate(ResourcePrefab, location, ResourcePrefab.transform.rotation);
                    
                    GWorld.Instance.GetQueue(QueueName).AddResource(puddle,WorldState,GWorld.Instance.GetWorld());
                    
                    stateFound = false;
                    StateStrength = initialStrength;
                    Beliefs.RemoveState(State);
                }
            }
        }
    }
}
