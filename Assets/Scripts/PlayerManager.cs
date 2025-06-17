using Corelib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Ingame
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        public UnityEvent<Vector3> onPlayerTeleport = new();

        private PlayerController _playerController;
        public PlayerController PlayerController { get => _playerController ??= FindPlayerController(); }

        public PlayerModel PlayerModel { get => PlayerController.playerModel; }

        private PlayerController FindPlayerController()
            => GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();

        public void TeleportPlayer(Vector3 worldPosition)
        {
            PlayerModel.transform.position = worldPosition;
        }
    }
}