using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class ResourceQueue
    {
        public Queue<GameObject> que = new Queue<GameObject>();
        public string tag;
        public string modState;

        public ResourceQueue(string t, string ms, WorldStates w)
        {
            tag = t;
            modState = ms;
            if (tag != "")
            {
                GameObject[] resources = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject r in resources)
                    que.Enqueue(r);
            }

            if (modState != "")
            {
                w.ModifyState(modState, que.Count);  // input a ms key for resource to manage via global worldState that apply for reusable resources
            }
        }

        public void AddResource(GameObject r)
        {
            que.Enqueue(r);
        }
        
        public void AddResource(GameObject r, string ms, WorldStates w)
        {
            if (modState != "")
            {
                que.Enqueue(r);
                w.ModifyState(modState, +1);
            }
        }
        
        // Remove an identified object in queue
        public void RemoveResource(GameObject r)
        {
            que = new Queue<GameObject>(que.Where(p => p != r));  // return a queue without the r
        }
        
        public void RemoveResource(GameObject r, string ms, WorldStates w)
        {
            if (modState != "")
            {
                if (que.Count == 0) return;
                que = new Queue<GameObject>(que.Where(p => p != r));
                w.ModifyState(modState, -1);
            }
        }

        // Remove the bottom object of the queue
        public GameObject RemoveResource()
        {
            if (que.Count == 0) return null;
            return que.Dequeue();
        }
        
        public GameObject RemoveResource(string ms, WorldStates w)
        {
            if (que.Count == 0) return null;
            w.ModifyState(modState, -1);
            return que.Dequeue();
        }

    }
}