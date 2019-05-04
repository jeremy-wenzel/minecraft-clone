using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory
    {
        private readonly List<GameObject> inventoryItems = new List<GameObject>();
        private int currentIndex = 0;

        public Inventory()
        {
            SetupInventoryHelper(PrefabType.Pickaxe);
            SetupInventoryHelper(PrefabType.Grass);
            SetupInventoryHelper(PrefabType.Sand);
            SetupInventoryHelper(PrefabType.Snow);
            SetupInventoryHelper(PrefabType.Stone);
        }

        private void SetupInventoryHelper(PrefabType prefabType)
        {
            inventoryItems.Add(PrefabManager.GetPrefab(prefabType));
        }

        public GameObject GetCurrentItem()
        {
            return inventoryItems[currentIndex];
        }

        public GameObject GetNextItem()
        {
            currentIndex++;
            if (currentIndex >= inventoryItems.Count)
            {
                currentIndex = 0;
            }

            return GetCurrentItem();

        }

        public GameObject GetPreviousItem()
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = inventoryItems.Count - 1;
            }

            return GetCurrentItem();

        }
    }
}
