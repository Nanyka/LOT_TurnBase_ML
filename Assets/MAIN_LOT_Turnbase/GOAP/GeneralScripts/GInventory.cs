using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class GInventory
    {
        public List<GameObject> items = new List<GameObject>();

        public void AddItem(GameObject item)
        {
            items.Add(item);
        }

        public GameObject FindItemWithTag(string targettag)
        {
            foreach (var item in items)
            {
                if (item == null)  // check if the object is destroyed while AICharacter try to access it
                    break;

                if (item.CompareTag(targettag))
                    return item;
            }

            return null;
        }

        public void RemoveItem(GameObject targetItem)
        {
            int indexToRemove = -1;
            foreach (var item in items)
            {
                indexToRemove++;
                if (item == targetItem)
                    break;
            }

            if (indexToRemove > -1)
            {
                items.RemoveAt(indexToRemove);
            }
        }

        public void ClearInventory()
        {
            items.Clear();
        }

        public bool IsEmpty()
        {
            return items.Count == 0;
        }
    }
}