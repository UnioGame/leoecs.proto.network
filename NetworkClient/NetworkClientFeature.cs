namespace Game.Ecs.Network.NetworkClient
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using UniGame.LeoEcs.Bootstrap.Runtime;

    [Serializable]
    public class NetworkClientFeature : EcsFeature
    {
        protected sealed override UniTask OnInitializeAsync(IProtoSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
    }

}