using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public UnityEvent<Vector3> onPlayerTeleport = new();

        private PlayerModel _playerModel;
        public PlayerModel PlayerModel { get => _playerModel ??= FindPlayer(); }

        private PlayerModel FindPlayer()
            => GameObject.FindWithTag("Player")?.GetComponent<PlayerModel>();

        public void TeleportPlayer(Vector3 worldPosition)
        {
            PlayerModel.transform.position = worldPosition;
        }
    }
}