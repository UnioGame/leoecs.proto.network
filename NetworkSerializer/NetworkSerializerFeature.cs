namespace Ecs.Network.GameSettings
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Feature/Network Serializer Feature", 
        fileName = "Network Serializer Feature")]
    public class NetworkSerializerFeature : BaseLeoEcsFeature
    {
        public sealed override UniTask InitializeAsync(IProtoSystems ecsSystems)
        {
            return UniTask.CompletedTask;
        }
    }

}