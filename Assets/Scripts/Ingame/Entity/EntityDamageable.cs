using UnityEngine;

namespace Ingame
{
    // TODO: Refactoring
    public class EntityDamageable : IDamageable
    {
        public float Damaged(EntityModel entity, EntityModel target, float damage)
        {
            return damage;
        }

        public void Dead(EntityModel entity, EntityModel target = null)
        {
            entity.onDead.Invoke(target);
        }

        public bool IsDamageableState(EntityModel entity)
        {
            return entity.isSpanwed && !entity.isInvincible;
        }
    }
}