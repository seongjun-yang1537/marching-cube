using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public static class ItemDB
    {
        private const string PATH_ICON_PREFIX = "Items/Icons";

        private static Dictionary<ItemID, Sprite> sprites = new();

        public static Sprite GetIconSprite(ItemID itemID)
        {
            if (!sprites.ContainsKey(itemID))
                sprites.Add(itemID, Resources.Load<Sprite>($"{PATH_ICON_PREFIX}/{itemID}"));
            if (sprites[itemID] == null)
            {
                Debug.LogError($"Not Have Icon Sprite {itemID}");
                sprites[itemID] = Resources.Load<Sprite>($"{PATH_ICON_PREFIX}/{itemID}");
            }
            return sprites[itemID];
        }
    }
}