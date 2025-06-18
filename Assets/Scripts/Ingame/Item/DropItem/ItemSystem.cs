using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public class ItemSystem : Singleton<ItemSystem>
    {
        public static ItemController SpawnHeldItem(ItemSpawnContext context)
        {
            GameObject go = Instantiate(ItemDB.GetItemModel(context.itemID));
            go.name = $"[HeldItem]{context.itemID}";

            Transform tr = go.transform;
            tr.SetParent(Instance.transform);
            tr.position = context.position;

            ItemModel model = go.GetComponent<ItemModel>();
            model.itemStack = context.itemStack;

            ItemController controller = go.GetComponent<ItemController>();
            controller.ChangeItemState(ItemModelState.Held);

            return controller;
        }

        public static ItemController SpawnHeldItem(Vector3 position, ItemStack itemStack)
            => SpawnHeldItem(ItemSpawnContext.Builder()
                .SetPosition(position)
                .SetItemStack(itemStack)
                .Build()
            );

        public static ItemController SpawnDropItem(ItemSpawnContext context)
        {
            GameObject go = Instantiate(ItemDB.GetItemModel(context.itemID));
            go.name = $"[DropItem]{context.itemID}";

            Transform tr = go.transform;
            tr.SetParent(Instance.transform);
            tr.position = context.position;

            ItemModel model = go.GetComponent<ItemModel>();
            model.itemStack = context.itemStack;

            ItemController controller = go.GetComponent<ItemController>();
            controller.ChangeItemState(ItemModelState.Dropped);

            return controller;
        }

        public static ItemController SpawnDropItem(Vector3 position, ItemStack itemStack)
            => SpawnDropItem(ItemSpawnContext.Builder()
                .SetPosition(position)
                .SetItemStack(itemStack)
                .Build()
            );
    }

    public struct ItemSpawnContext
    {
        public Vector3 position;
        public ItemStack itemStack;
        public ItemID itemID { get => itemStack.itemID; }
        public int count { get => itemStack.count; }
        public GameObject owner;

        public ItemSpawnContext(Vector3 position, ItemStack itemStack, GameObject owner = null)
        {
            this.position = position;
            this.itemStack = itemStack;
            this.owner = owner;
        }

        public static ItemSpawnContextBuilder Builder() => new ItemSpawnContextBuilder();
    }

    public class ItemSpawnContextBuilder
    {
        private Vector3 _position;
        private ItemStack _itemStack;
        private GameObject _owner;

        private bool _hasPosition = false;
        private bool _hasItem = false;

        public ItemSpawnContextBuilder SetPosition(Vector3 position)
        {
            _position = position;
            _hasPosition = true;
            return this;
        }

        public ItemSpawnContextBuilder SetItemStack(ItemStack itemStack)
        {
            _itemStack = itemStack;
            _hasItem = true;
            return this;
        }

        public ItemSpawnContextBuilder SetOwner(GameObject owner)
        {
            _owner = owner;
            return this;
        }

        public ItemSpawnContext Build()
        {
            if (!_hasPosition)
                throw new System.InvalidOperationException("DropItemSpawnContextBuilder: position is required.");
            if (!_hasItem)
                throw new System.InvalidOperationException("DropItemSpawnContextBuilder: itemID is required.");

            return new ItemSpawnContext(_position, _itemStack, _owner);
        }
    }
}