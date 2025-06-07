using UnityEngine;

namespace Ingame
{
    public class EntitySpawnable : ISpawnable
    {
        public void Spawn(EntityModel entityModel)
        {
            entityModel.isSpanwed = true;
            entityModel.onSpawn.Invoke();
        }
        public void DeSpawn(EntityModel entityModel)
        {
            entityModel.isSpanwed = false;
            entityModel.onDeSpawn.Invoke();
        }
    }
}