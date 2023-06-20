using UnityEngine;

namespace GOAP
{
    [ExecuteInEditMode]
    public class GAgentVisual : MonoBehaviour
    {
        public GAgent thisAgent;

        // Start is called before the first frame update
        void Start()
        {
            thisAgent = this.GetComponent<GAgent>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
