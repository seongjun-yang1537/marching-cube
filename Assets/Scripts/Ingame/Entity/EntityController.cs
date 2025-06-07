using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(EntityModel))]
    public class EntityController : MonoBehaviour
    {
        protected EntityModel entityModel;

        protected ISpawnable spawnable = new EntitySpawnable();
        protected IDamageable damageable = new EntityDamageable();

        protected virtual void Awake()
        {
            entityModel = GetComponent<EntityModel>();
        }

        public virtual void Spawn()
        {
            spawnable?.Spawn(entityModel);
        }

        public virtual void DeSpawn()
        {
            spawnable?.DeSpawn(entityModel);
        }

        public virtual bool IsDamageableState()
        {
            return damageable?.IsDamageableState(entityModel) ?? false;
        }

        public virtual float Damaged(EntityModel target, float damage)
        {
            return damageable?.Damaged(entityModel, target, damage) ?? -1;
        }

        public virtual void Dead(EntityModel target = null)
        {
            damageable?.Dead(entityModel, target);
            DeSpawn();
        }
    }
}