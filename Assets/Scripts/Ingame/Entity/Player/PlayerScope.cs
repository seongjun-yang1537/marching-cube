using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Ingame
{
    public class PlayerScope : LifetimeScope
    {
        [SerializeField] private PlayerModel playerModel;
        [SerializeField] private PlayerController playerController;

        protected void Awake()
        {
            playerModel = GetComponent<PlayerModel>();
            playerController = GetComponent<PlayerController>();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerModel)
                   .AsSelf();

            var jetpack = GetComponentInChildren<IJetpackable>();
            builder.RegisterComponent(jetpack)
                   .As<IJetpackable>();

            playerController.Construct(jetpack);

            builder.RegisterComponent(playerController)
                   .AsSelf();
        }
    }
}
