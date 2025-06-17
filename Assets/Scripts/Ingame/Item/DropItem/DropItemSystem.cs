using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public class DropItemSystem : Singleton<DropItemSystem>
    {
        public static DropItemController Spawn(DropItemSpawnContext context)
        {
            GameObject go = Instantiate(ItemDB.GetDropItem(context.itemID));
            go.name = $"[DropItem]{context.itemID}";

            Transform tr = go.transform;
            tr.SetParent(Instance.transform);
            tr.position = context.position;

            DropItemModel model = go.GetComponent<DropItemModel>();
            model.itemID = context.itemID;
            model.count = context.count;

            DropItemController controller = go.GetComponent<DropItemController>();

            return controller;
        }

        public static DropItemController Spawn(Vector3 position, ItemID itemID, int count = 1)
            => Spawn(DropItemSpawnContext.Builder()
                .SetPosition(position)
                .SetItemID(itemID)
                .SetCount(count)
                .Build()
                );

        public static DropItemController Spawn(Vector3 position, ItemStack itemStack)
            => Spawn(position, itemStack.itemID, itemStack.count);
    }

    public struct DropItemSpawnContext
    {
        public Vector3 position;
        public ItemID itemID;
        public int count;
        public GameObject owner;

        public DropItemSpawnContext(Vector3 position, ItemID itemID, int count, GameObject owner = null)
        {
            this.position = position;
            this.itemID = itemID;
            this.count = count;
            this.owner = owner;
        }

        public static DropItemSpawnContextBuilder Builder() => new DropItemSpawnContextBuilder();
    }

    public class DropItemSpawnContextBuilder
    {
        private Vector3 _position;
        private ItemID _itemID;
        private int _count = 1;
        private GameObject _owner;

        private bool _hasPosition = false;
        private bool _hasItemID = false;

        public DropItemSpawnContextBuilder SetPosition(Vector3 position)
        {
            _position = position;
            _hasPosition = true;
            return this;
        }

        public DropItemSpawnContextBuilder SetItemID(ItemID itemID)
        {
            _itemID = itemID;
            _hasItemID = true;
            return this;
        }

        public DropItemSpawnContextBuilder SetCount(int count)
        {
            _count = count;
            return this;
        }

        public DropItemSpawnContextBuilder SetOwner(GameObject owner)
        {
            _owner = owner;
            return this;
        }

        public DropItemSpawnContext Build()
        {
            if (!_hasPosition)
                throw new System.InvalidOperationException("DropItemSpawnContextBuilder: position is required.");
            if (!_hasItemID)
                throw new System.InvalidOperationException("DropItemSpawnContextBuilder: itemID is required.");

            return new DropItemSpawnContext(_position, _itemID, _count, _owner);
        }
    }
}