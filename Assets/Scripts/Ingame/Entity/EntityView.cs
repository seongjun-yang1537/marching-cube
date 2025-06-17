using UnityEngine;

namespace Ingame
{
    [RequireComponent(typeof(EntityView))]
    public class EntityView : MonoBehaviour
    {
        protected EntityModel entityModel;

        protected void Awake()
        {
            entityModel = GetComponent<EntityModel>();
        }
    }
}