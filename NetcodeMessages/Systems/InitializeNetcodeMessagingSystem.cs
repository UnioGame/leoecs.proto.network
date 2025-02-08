namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Components;
    using FishNet.Object;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Components.Events;
    using UniGame.Core.Runtime;
    using UniGame.Core.Runtime.Extension;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

    /// <summary>
    /// initiaize netcode messaging system
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class InitializeNetcodeMessagingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private NetworkObject _rpcPrefab;
        private FishNetAspect _netcodeAspect;
        private ILifeTime _lifeTime;
        private ProtoWorld _world;
        
        private ProtoIt _filter = It.Chain<NetcodeManagerComponent>()
            .Inc<NetworkServerConnectedSelfEvent>()
            .End();
        
        private ProtoIt _rpcFilter = It
            .Chain<NetcodeMessageChannelComponent>()
            .End();


        public InitializeNetcodeMessagingSystem(NetworkObject rpcPrefab)
        {
            _rpcPrefab = rpcPrefab;
        }
        
        public void Init(IProtoSystems systems)
        {
            _world = systems.GetWorld();
            _lifeTime = _world.GetLifeTime();
        }

        public void Run()
        {
            if (_rpcFilter.Len() > 0) return;

            var first = _filter.First();
            if(!first.Ok) return;

            var managerEntity = first.Entity;
            ref var managerComponent = ref _netcodeAspect.Manager.Get(managerEntity);
            var manager = managerComponent.Value;
            
            if(!manager.Initialized || !manager.IsServerStarted)return;

            var rpcInstanceObject = _rpcPrefab.Spawn()
                .DespawnWith(_lifeTime);
            
            var rpcInstance = rpcInstanceObject.GetComponent<NetworkObject>();
            rpcInstance.Spawn();
        }
    }
}