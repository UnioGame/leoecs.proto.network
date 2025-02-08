namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

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
    public class UpdateNetcodeStatusSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        
        private ProtoWorld _world;
        private ProtoIt _filter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
        private ProtoIt _networkLinkFilter= It
            .Chain<NetworkLinkComponent>()
            .End();

        public void Run()
        {
            foreach (var entity in _filter)
            {
                ref var managerComponent = ref _networkAspect.NetworkSource.Get(entity);
                var manager = managerComponent.Value;
                
                ref var agentComponent = ref _networkAspect.NetworkAgent.Get(entity);
                ref var connectionTypeComponent = ref _networkAspect.ConnectionType.Get(entity);
                
                var isClient = manager.IsClientStarted || manager.IsHostStarted;
                var isServer = manager.IsServerStarted;
        
                ref var statusComponent = ref _networkAspect.Status.Get(entity);
                statusComponent.IsConnected = true;
                statusComponent.Status = ConnectionStatus.Connected;
                connectionTypeComponent.IsClient = isClient;
                connectionTypeComponent.IsServer = isServer;
                connectionTypeComponent.IsHost = manager.IsHostStarted;
                connectionTypeComponent.IsActive = isClient || isServer;
                
                agentComponent.Id = manager.ActiveClientId;
            }

        }
    }
}