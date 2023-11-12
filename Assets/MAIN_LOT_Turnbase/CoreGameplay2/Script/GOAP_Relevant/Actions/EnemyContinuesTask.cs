using UnityEngine;

namespace GOAP
{
    public class EnemyContinuesTask : GAction
    {
        public override bool PrePerform()
        {
            Debug.Log("Execute attack animation");
            return true;
        }

        public override bool PostPerform()
        {
            Debug.Log("Check to remove (\")targetAvailable(\") state");
            return true;
        }
    }
}