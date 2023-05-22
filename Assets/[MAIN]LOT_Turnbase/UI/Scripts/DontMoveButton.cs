using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    public class DontMoveButton : MonoBehaviour
    {
        public void OnClickButton()
        {
            MainUI.Instance.OnClickIdleButton.Invoke();
        }
    }
}
