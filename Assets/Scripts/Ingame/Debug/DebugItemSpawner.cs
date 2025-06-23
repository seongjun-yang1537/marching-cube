using Corelib.Utils;
using UnityEngine;

namespace Ingame
{
    public class DebugItemSpawner : MonoBehaviour
    {
        public ItemID itemID;
        public int count;

        public float force;

        private MT19937 rng;
        protected void Awake()
        {
            rng = MT19937.Create();
        }

        public void Spawn()
        {
            ItemStack itemStack = new ItemStack(itemID, count);

            ItemController controller = ItemSystem.SpawnDropItem(transform.position, itemStack);
            Rigidbody rigidbody = controller.GetComponent<Rigidbody>();

            Vector3 upVector = (Random.insideUnitSphere + Vector3.up * 5).normalized;
            float randomForce = rng.NextFloat(force, force * 1.5f);
            rigidbody.AddForce(upVector * randomForce, ForceMode.Impulse);
        }
    }
}