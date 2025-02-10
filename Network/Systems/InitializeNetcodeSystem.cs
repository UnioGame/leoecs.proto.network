namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
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
    public class InitializeNetcodeSystem : IEcsRunSystem, IProtoInitSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkAssetsSettings _netcodeSettings;
        
        private ProtoWorld _world;
        private IProtoSystems _systems;
        
        private ProtoIt _filter= It
            .Chain<InitializeNetcodeRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
       
        private bool _isLoading;

        
        public void Init(IProtoSystems systems)
        {
            var lifeTime = _world.GetWorldLifeTime();
            foreach (var setting in _netcodeSettings.networkSettings)
                setting.InitializeAsync(_systems, lifeTime).Forget();
        }
        
        public void Run()
        {
            var isExists = _netFilter.First();
        }

    }
}