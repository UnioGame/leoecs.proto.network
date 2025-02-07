namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Aspects;
    using Componenets.Requests;
    using Cysharp.Threading.Tasks;
    using Data;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.AddressableTools.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Object = UnityEngine.Object;

    /// <summary>
    /// initialize netcode data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class InitializeNetcodeSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _filter= It
            .Chain<InitializeNetcodeSelfRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
        private UnityNetcodeSettings _netcodeSettings;
        private bool _isLoading;

        public void Run()
        {
            var isExists = _netFilter.First();
            
            foreach (var entity in _filter)
            {
                if (isExists.Ok)
                {
                    _netcodeAspect.InitializeSelf.Del(entity);
                    continue;
                }

                //network object in loading state
                if (_isLoading) continue;
                
                _isLoading = true;
                LoadNetcodeAgent().Forget();
            }
        }

        private async UniTask LoadNetcodeAgent()
        {
            var agentSource = _netcodeSettings.networkPrefab;
            var agent = await agentSource.LoadAssetInstanceTaskAsync(_world.GetWorldLifeTime(), true);
            Object.DontDestroyOnLoad(agent);
        }
    }
}