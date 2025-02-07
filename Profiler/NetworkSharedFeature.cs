namespace Game.Ecs.Network.Profiler
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;

    [Serializable]
    public sealed class NetworkEcsProfilerFeature : EcsFeature
    {
        protected override UniTask OnInitializeAsync(IProtoSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();

            ecsSystems.Add(new UpdateEcsNetworkTrafficCounterSystem());

            return UniTask.CompletedTask;
        }
    }

}