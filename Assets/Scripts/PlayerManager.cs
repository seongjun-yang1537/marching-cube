using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public UnityEvent<Vector3> onPlayerTeleport = new();

        public PlayerModel playerModel;

        private void Awake()
        {
            playerModel = FindPlayer();
        }

        private PlayerModel FindPlayer()
            => GameObject.FindWithTag("Player")?.GetComponent<PlayerModel>();

        public void TeleportPlayer(Vector3 worldPosition)
        {
            playerModel.transform.position = worldPosition;
        }
    }
}