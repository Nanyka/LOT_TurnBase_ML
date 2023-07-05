using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class ButtonPointer : MonoBehaviour
    {
        [SerializeField] private GameObject _buttonPointer;

        private void Start()
        {
            MainUI.Instance.OnSwitchButtonPointer.AddListener(SwitchPointer);
        }

        private void SwitchPointer(Vector3 position, bool isShow)
        {
            _buttonPointer.transform.position = position;
            _buttonPointer.SetActive(isShow);
        }
    }
}