using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public class ItemStack
    {
        public int count;
        public ItemData itemData;
        public ItemID itemID { get => itemData.itemID; }

        public bool IsEmpty { get => itemData == null || count == 0; }

        public ItemStack(ItemData itemData, int count = 1)
        {
            this.itemData = itemData;
            this.count = count;
        }

        public IEnumerator<ItemData> ToItemData()
        {
            for (int i = 0; i < count; i++)
                yield return itemData;
        }

        public List<ItemStack> SplitInto(int parts)
        {
            var result = new List<ItemStack>(parts);
            if (parts <= 0) return result;

            int baseAmount = count / parts;
            int remainder = count % parts;

            for (int i = 0; i < parts; i++)
            {
                int stackCount = baseAmount + (i < remainder ? 1 : 0);
                result.Add(new ItemStack(itemData, stackCount));
            }

            return result;
        }

        public int Add(int amount)
        {
            if (itemData == null || amount <= 0)
                return 0;

            int maxAddable = itemData.maxStackable - count;
            int toAdd = Mathf.Min(maxAddable, amount);
            count += toAdd;
            return toAdd;
        }

        public int Remove(int amount)
        {
            if (amount <= 0)
                return 0;

            int toRemove = Mathf.Min(count, amount);
            count -= toRemove;
            return toRemove;
        }
    }
}