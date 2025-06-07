namespace Ingame
{
    public interface ISpawnable
    {
        public void Spawn(EntityModel entity);
        public void DeSpawn(EntityModel entity);
    }
}