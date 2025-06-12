using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    public static class ItemDB
    {
        private const string PATH_PREFIX = "Items";

        private static Dictionary<ItemID, Sprite> sprites = new();

        public static Sprite GetSprite(ItemID itemID)
        {
            if (!sprites.ContainsKey(itemID) || sprites[itemID] == null)
                sprites.Add(itemID, Resources.Load<Sprite>($"{PATH_PREFIX}/{itemID}"));
            return sprites[itemID];
        }
    }
}