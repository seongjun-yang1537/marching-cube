using System.Collections.Generic;
using System.Net.Security;
using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public static class ItemDB
    {
        private const string PATH_ICON_PREFIX = "Items/Icons";
        private const string PATH_MODELS_PREFIX = "Items/Models";

        private static Dictionary<ItemID, Sprite> sprites = new();
        private static Dictionary<ItemID, Texture2D> editorIconTextures = new();
        private static Dictionary<ItemID, GameObject> models = new();

        public static Sprite GetIconSprite(ItemID itemID)
        {
            if (itemID == ItemID.None)
                return null;
            if (!sprites.ContainsKey(itemID))
                sprites.Add(itemID, Resources.Load<Sprite>($"{PATH_ICON_PREFIX}/{itemID}"));
            if (sprites[itemID] == null)
            {
                Debug.Log($"Not Have Icon Sprite {itemID}");
                sprites[itemID] = Resources.Load<Sprite>($"{PATH_ICON_PREFIX}/{itemID}");
            }
            return sprites[itemID];
        }

        public static Texture2D GetEditorIconTexture(ItemID itemID)
        {
            if (itemID == ItemID.None)
                return null;

            if (!editorIconTextures.ContainsKey(itemID))
                editorIconTextures.Add(itemID, GetIconSprite(itemID).ToTexture2D().ResizeTexture(32, 32));
            return editorIconTextures[itemID];
        }

        public static GameObject GetItemModel(ItemID itemID)
        {
            if (!models.ContainsKey(itemID))
                models.Add(itemID, Resources.Load<GameObject>($"{PATH_MODELS_PREFIX}/{itemID}"));

            return models[itemID];
        }
    }
}