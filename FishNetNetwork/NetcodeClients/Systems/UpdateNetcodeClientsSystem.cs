namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using System.Linq;
    using Aspects;
    using Components;
    using FishNet.Object;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

    /// <summary>
    /// update netcode clients list
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateNetcodeClientsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private FishNetAspect _netcodeAspect;
        private NetcodeClientAspect _clientAspect;
        private NetworkClientAspect _networkClientAspect;
        private NetcodePlayerAspect _netcodePlayerAspect;
        
        private ProtoWorld _world;
        
        
        private NativeHashMap<ulong,ProtoPackedEntity> _clients;
        private NativeList<ulong> _removedIds;
        
        private ProtoItExc _newClients= It
            .Chain<NetworkClientComponent>()
            .Inc<NetcodeClientObjectComponent>()
            .Exc<NetworkLinkComponent>()
            .End();
        
        private ProtoIt _managerFilter= It
            .Chain<NetcodeManagerComponent>()
            .End();
        
        public void Init(IProtoSystems systems)
        {
            _world = systems.GetWorld();
            
            var lifeTime = _world.GetWorldLifeTime();
            
            _clients = new NativeHashMap<ulong, ProtoPackedEntity>(100, Allocator.Persistent)
                .AddTo(lifeTime);
            
            _removedIds = new NativeList<ulong>(8,Allocator.Persistent).AddTo(lifeTime);
        }

        public void Run()
        {
            var managerEntityResult = _managerFilter.First();
            if(!managerEntityResult.Ok) return;

            var managerEntity = managerEntityResult.Entity;
            ref var managerComponent = ref _netcodeAspect.Manager.Get(managerEntity);
            ref var connectionTypeComponent = ref _netcodeAspect.ConnectionType.Get(managerEntity);
            var manager = managerComponent.Value;
            
            //add new clients and fire event
            foreach (var newClientEntity in _newClients)
            {
                ref var gameObjectComponent = ref _clientAspect.GameObject.Get(newClientEntity);
                ref var objectComponent = ref _clientAspect.ClientObject.GetOrAddComponent(newClientEntity);
                ref var idComponent = ref _networkClientAspect.ClientId.GetOrAddComponent(newClientEntity);
                ref var connectionComponent = ref _networkClientAspect.Connection.GetOrAddComponent(newClientEntity);
                
                ref var linkComponent = ref _networkClientAspect.NetworkLink.Add(newClientEntity);
                ref var connectedSelfEvent = ref _networkClientAspect.Connected.Add(newClientEntity);

                var clientBehaviour = gameObjectComponent.Value.GetComponent<NetworkObject>();
                objectComponent.Value = clientBehaviour;
                linkComponent.Value = _world.PackEntity(managerEntity);
                var networkClient = objectComponent.Value;

                connectionComponent.IsActive = connectionTypeComponent.IsActive;
                connectionComponent.IsClient = connectionTypeComponent.IsClient;
                connectionComponent.IsServer = connectionTypeComponent.IsServer;
                
                if(networkClient == null) continue;

                var id = networkClient.OwnerId;
                var ownerId = (ulong)id;
                idComponent.Id = ownerId;
                
                _clients[ownerId] = _world.PackEntity(newClientEntity);
                
                if (clientBehaviour.IsHostInitialized)
                {
                    _networkClientAspect.Local.Add(newClientEntity);
                }
            }

            _removedIds.Clear();
            
            //is client disconnected
            foreach (var client in _clients)
            {
                var clientValue = client.Value;
                var clientId = client.Key;

                if (clientValue.Unpack(_world, out var clientEntity)) continue;

                _removedIds.Add(clientId);
                    
                //fire disconnect event
                var eventEntity = _world.NewEntity();
                ref var clientDisconnectEvent = ref _networkClientAspect.Disconnected.Add(eventEntity);
                clientDisconnectEvent.ClientId = clientId;
            }

            foreach (var id in _removedIds)
                _clients.Remove(id);
        }
    }
}