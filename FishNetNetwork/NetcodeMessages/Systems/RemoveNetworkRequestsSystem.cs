namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Components;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

    /// <summary>
    /// send message with base rpc channel
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class RemoveNetworkRequestsSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkMessageAspect _messageAspect;
        private NetcodeMessageAspect _rpcAspect;
        
        private ProtoWorld _world;
        
        private ProtoItExc _filter= It
            .Chain<NetworkIdComponent>()
            .Exc<NetworkSyncComponent>()
            .End();
        
        private ProtoIt _transferFilter= It
            .Chain<NetworkTransferRequest>()
            .End();
        
        private ProtoIt _networkFilter= It
            .Chain<NetworkConnectionTypeComponent>()
            .End();
        
        private ProtoItExc _eventFilter= It
            .Chain<SerializeNetworkEntityRequest>()
            .Inc<NetworkEventComponent>()
            .Exc<NetworkSyncComponent>()
            .End();
        
        public void Run()
        {
            foreach (var eventEntity in _eventFilter)
            {
                GameLog.Log($"Removing serialized message {eventEntity}");
                _world.DelEntity(eventEntity);
            }
            
            var networkEntityOk = _networkFilter.First();
            if (!networkEntityOk.Ok) return;
            
            ref var connectionComponent = ref _networkAspect.ConnectionType.Get(networkEntityOk.Entity);
            if (connectionComponent.IsServer) return;
            
            var transferEntity = _transferFilter.First();
            if(!transferEntity.Ok) return;
            
            foreach (var entity in _filter)
            {
                _messageAspect.NetworkId.Del(entity);
                _messageAspect.Target.TryRemove(entity);
            }
        }
    }
}