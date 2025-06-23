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

        private PlayerModel _playerModel;
        public PlayerModel PlayerModel { get => _playerModel ??= FindPlayerModel(); }


        private PlayerController FindPlayerController()
            => GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        private PlayerModel FindPlayerModel()
            => GameObject.FindWithTag("Player")?.GetComponent<PlayerModel>();

        public void TeleportPlayer(Vector3 worldPosition)
        {
            PlayerModel.transform.position = worldPosition;
        }
    }
}