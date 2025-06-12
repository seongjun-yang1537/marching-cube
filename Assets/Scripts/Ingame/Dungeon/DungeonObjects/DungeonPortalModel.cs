using Corelib.Utils;
using UnityEngine;

namespace Ingame
{

    public class DungeonPortalModel : MonoBehaviour
    {
        private DungeonManager dungeonManager { get => DungeonManager.Instance; }

        public void OnTriggerEnter(Collider other)
        {
            GameObject go = other.gameObject;
            if (go.HasComponent<PlayerModel>())
            {
                dungeonManager.EnterNextFloor();
            }
        }
    }
}