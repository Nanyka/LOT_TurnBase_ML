using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class TimelineClicker : MonoBehaviour
    {
        [SerializeField] private ButtonRequire _buttonRequire;

        public void OnClick()
        {
            TimelineManager.Instance.ResumeTimeline(_buttonRequire);
        }
    }
}
