namespace Game.Ecs.Network.UnityNetcode.Data
{
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using UniGame.Core.Runtime;

    public interface INetworkInitSettings
    {
        UniTask InitializeAsync(IProtoSystems systems,ILifeTime lifeTime);
    }
}