using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class DontMoveButton : MonoBehaviour
    {
        public void OnClickButton()
        {
            MainUI.Instance.OnClickIdleButton.Invoke();
        }
    }
}
