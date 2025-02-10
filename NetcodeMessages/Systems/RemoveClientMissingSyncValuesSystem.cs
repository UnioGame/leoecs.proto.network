namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using Shared.Aspects;
    using Shared.Components;
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
    public class RemoveClientMissingSyncValuesSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeMessageAspect _messageAspect;
        private NetworkCommandsAspect _networkMessageAspect;
        
        private ProtoWorld _world;

        private ProtoIt _filter= It
            .Chain<NetworkIdComponent>()
            .Inc<NetworkSyncComponent>()
            .End();
        
        private ProtoIt _networkFilter= It
            .Chain<NetworkConnectionTypeComponent>()
            .Inc<NetworkSyncValuesComponent>()
            .End();

        public void Run()
        {
            var networkEntityResult = _networkFilter.First();
            if (!networkEntityResult.Ok) return;
            
            var networkEntity = networkEntityResult.Entity;
            ref var syncValuesComponent = ref _messageAspect.SyncValues.Get(networkEntity);
            
            foreach (var entity in _filter)
            {
                ref var syncIdComponent = ref _networkMessageAspect.NetworkId.Get(entity);
                var syncId = syncIdComponent.Id;
                var found = syncValuesComponent.Values.TryGetValue(syncId, out var packedEntity);
                var exists = packedEntity.Unpack(_world, out var syncEntity);

                if (!found || !exists || entity != syncEntity)
                {
                    _world.DelEntity(entity);
                }
            }
        }
    }
}