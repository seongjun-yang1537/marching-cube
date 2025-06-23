using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class EntityModel : MonoBehaviour
    {
        public UnityEvent<float> onLife = new();
        public UnityEvent<float> onLifeMax = new();

        public UnityEvent onSpawn = new();
        public UnityEvent onDeSpawn = new();
        public UnityEvent<EntityModel, float> onDamaged = new();
        public UnityEvent<EntityModel> onDead = new();

        public float life { get; private set; }
        public float lifeMax { get; private set; }
        public float lifeRatio { get => Mathf.Approximately(0f, lifeMax) ? 0f : life / lifeMax; }

        public bool isDead { get => isSpanwed && life <= 0; }

        public bool isSpanwed;
        public bool isInvincible;

        public void SetLife(float newLife)
        {
            life = newLife;
            onLife.Invoke(newLife);
        }

        public void SetLifeMax(float newLifeMax)
        {
            lifeMax = newLifeMax;
            onLifeMax.Invoke(newLifeMax);
        }

        public void TakeDamge(float damage, EntityModel other)
        {
            SetLife(life - damage);
            if (life <= 0f)
                Dead(other);
        }

        public void Dead(EntityModel target = null)
        {
            onDead.Invoke(target);
        }
    }
}
