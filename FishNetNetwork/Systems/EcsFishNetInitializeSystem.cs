namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Componenets.Requests;
    using Components;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
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
    public class EcsFishNetInitializeSystem : IProtoRunSystem
    {
        private FishNetSettings _settings;
        private NetworkAspect _networkFeature;
        private ProtoWorld _world;
        
        private ProtoIt _filter = It
            .Chain<InitializeNetcodeRequest>()
            .End();
        
        private ProtoIt _networkFilter = It
            .Chain<FishNetManagerComponent>()
            .End();

        private async UniTask InitializeFishNet()
        {
            var lifeTime = _world.GetWorldLifeTime();
            var networkPrefab = await _settings.networkPrefab
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            networkPrefab.SetActive(true);
        }

        public void Run()
        {
            foreach (var entity in _filter)
            {
                var networkEntityOk = _networkFilter.First();
                if (networkEntityOk.Ok) return;
                
                InitializeFishNet().Forget();

                return;
            }
        }
    }

}