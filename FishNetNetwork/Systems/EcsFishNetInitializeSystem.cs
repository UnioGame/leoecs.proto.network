namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using UniGame.AddressableTools.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine.AddressableAssets;

    /// <summary>
    /// create base data for a fishnet
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class EcsFishNetInitializeSystem : IProtoInitSystem
    {
        private FishNetSettings _settings;
        private ProtoWorld _world;
        
        public EcsFishNetInitializeSystem(FishNetSettings settings)
        {
            _settings = settings;
        }

        public void Init(IProtoSystems systems)
        {
            InitializeFishNet().Forget();
        }

        private async UniTask InitializeFishNet()
        {
            var lifeTime = _world.GetWorldLifeTime();
            var networkPrefab = await _settings.networkPrefab.LoadAssetInstanceTaskAsync(lifeTime, true);
            networkPrefab.SetActive(true);
        }
    }

}