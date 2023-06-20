using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleButton : MonoBehaviour
{
    public void OnClickIdleButton()
    {
        UIManager.Instance.OnClickIdleButton.Invoke();
    }
}
