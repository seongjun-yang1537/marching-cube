using UnityEngine;

namespace Ingame
{
    public class EntityDamageable : IDamageable
    {
        public float Damaged(EntityModel entity, EntityModel target, float damage)
        {
            if (!IsDamageableState(entity))
                return 0;

            entity.life -= damage;
            entity.onDamaged.Invoke(target, damage);

            if (entity.life <= 0f)
                Dead(target);

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