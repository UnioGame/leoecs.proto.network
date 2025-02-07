namespace Game.Ecs.Network.Shared
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Feature/Network/Network Shared Feature", fileName = "Network Shared Feature")]
    public class NetworkSharedFeature : BaseLeoEcsFeature
    {
        public sealed override UniTask InitializeAsync(IProtoSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();

            return UniTask.CompletedTask;
        }
    }

}