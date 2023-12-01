using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW.Demos
{
    public class BlinkingRevealer : MonoBehaviour
    {
        public float blinkCycleTime = 5;

        public bool randomOffset = true;
        private void Awake()
        {
            if (randomOffset)
                blinkCycleTime += Random.Range(0, blinkCycleTime * .5f);
        }
        private void Update()
        {
            if (Time.time % blinkCycleTime < blinkCycleTime / 2)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}