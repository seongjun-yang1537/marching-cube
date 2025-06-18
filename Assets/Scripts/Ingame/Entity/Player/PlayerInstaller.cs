using UnityEngine;
using Zenject;

namespace Ingame
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>()
                     .FromComponentInHierarchy()
                     .AsSingle();

            Container.Bind<IJetpackable>()
                .FromComponentInChildren()
                .AsSingle();

            Container.Bind<PlayerController>()
                .FromComponentOnRoot()
                .AsSingle();
        }
    }
}