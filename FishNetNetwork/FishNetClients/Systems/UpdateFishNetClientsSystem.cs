namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Components;
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
    using FishNetClientAspect = Aspects.FishNetClientAspect;

    /// <summary>
    /// update fishnet netcode clients list
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateFishNetClientsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private FishNetAspect _fishNetAspect;
        private FishNetClientAspect _fishNetClientAspect;
        private NetworkClientAspect _networkClientAspect;
        private NetcodePlayerAspect _netcodePlayerAspect;
        
        private ProtoWorld _world;
        
        private NativeHashMap<int,ProtoPackedEntity> _clients;
        private NativeList<int> _removedIds;
        
        private ProtoItExc _clientsFilter= It
            .Chain<NetworkClientComponent>()
            .Inc<NetcodeClientObjectComponent>()
            .Exc<NetworkSourceLinkComponent>()
            .End();
        
        private ProtoIt _managerFilter= It
            .Chain<FishNetManagerComponent>()
            .End();
        
        public void Init(IProtoSystems systems)
        {
            _world = systems.GetWorld();
            
            var lifeTime = _world.GetWorldLifeTime();
            
            _clients = new NativeHashMap<int, ProtoPackedEntity>(100, Allocator.Persistent)
                .AddTo(lifeTime);
            
            _removedIds = new NativeList<int>(8,Allocator.Persistent).AddTo(lifeTime);
        }

        public void Run()
        {
            var managerEntityResult = _managerFilter.First();
            if(!managerEntityResult.Ok) return;

            var managerEntity = managerEntityResult.Entity;
            ref var managerComponent = ref _fishNetAspect.Manager.Get(managerEntity);
            var manager = managerComponent.Value;
            var clientManager = manager.ClientManager;
            var fishNetClients = clientManager.Clients;
            
            var networkPacked = _world.PackEntity(managerEntity);

            foreach (var clientPair in fishNetClients)
            {
                //check is new client
                if (_clients.TryGetValue(clientPair.Key, out var packedClient) &&
                    packedClient.Unpack(_world, out var clientEntity))
                {
                    continue;
                }

                var newClientEntity = _fishNetClientAspect.CreateClient(_world.NewEntity(), clientPair.Value);
                _clients[clientPair.Key] = _world.PackEntity(newClientEntity);
                
                //fire new client connected event
                ref var clientConnectedEvent = ref _networkClientAspect.Connected.Add(newClientEntity);
                clientConnectedEvent.ClientId = clientPair.Key;
            }
            
            //add new clients and fire event
            foreach (var clientPair in _clients)
            {
                if(!clientPair.Value.Unpack(_world,out var clientEntity))
                    continue;
                
                ref var networkSourceLinkComponent = ref _networkClientAspect.NetworkLink.Get(clientEntity);
                ref var idComponent = ref _networkClientAspect.ClientId.Get(clientEntity);
                ref var connectionTypeComponent = ref _networkClientAspect.ConnectionType.Get(clientEntity);
                ref var ownerIdComponent = ref _networkClientAspect.OwnerId.Get(clientEntity);
                ref var connectionComponent = ref _fishNetClientAspect.Connection.Get(clientEntity);

                networkSourceLinkComponent.Value = networkPacked;
                
                var connection = connectionComponent.Value;
                var clientId = connection.ClientId;
                
                idComponent.Id = connection.ClientId;
                connectionTypeComponent.IsHost = connection.IsHost;
                connectionTypeComponent.IsActive = connection.IsActive;
                connectionTypeComponent.IsClient = true;
                connectionTypeComponent.IsServer = connection.IsHost;
                
                ownerIdComponent.Value = clientId;
                
                _clients[clientId] = _world.PackEntity(clientEntity);
                
                if (connection.IsLocalClient)
                {
                    _networkClientAspect.LocalClient.GetOrAddComponent(clientEntity);
                }
                else
                {
                    _networkClientAspect.LocalClient.TryRemove(clientEntity);
                }

                if (connection.IsHost)
                {
                    _networkClientAspect.Master.GetOrAddComponent(clientEntity);
                }
                else
                {
                    _networkClientAspect.Master.TryRemove(clientEntity);
                }
            }

            _removedIds.Clear();
            
            //is client disconnected
            foreach (var client in _clients)
            {
                var clientValue = client.Value;
                var clientId = client.Key;

                if (fishNetClients.TryGetValue(clientId, out var connection))
                {
                    if(connection.IsActive)
                        continue;
                }
                
                _removedIds.Add(clientId);
            }

            foreach (var id in _removedIds)
            {
                //fire disconnect event
                var eventEntity = _world.NewEntity();
                ref var clientDisconnectEvent = ref _networkClientAspect.Disconnected.Add(eventEntity);
                clientDisconnectEvent.ClientId = id;
                _clients.Remove(id);
            }
        }
    }
}