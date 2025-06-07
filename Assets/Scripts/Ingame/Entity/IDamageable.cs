namespace Ingame
{
    public interface IDamageable
    {
        public bool IsDamageableState(EntityModel entity);

        public float Damaged(EntityModel entity, EntityModel target, float damage);
        public void Dead(EntityModel entity, EntityModel target = null);
    }
}