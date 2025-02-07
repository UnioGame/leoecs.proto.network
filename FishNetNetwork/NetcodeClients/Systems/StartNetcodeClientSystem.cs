namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Aspects;
    using Componenets.Requests;
    using FishNet.Managing;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

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
    public class StartNetcodeClientSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        private NetworkClientAspect _clientAspect;
        private NetcodeClientAspect _netcodeClientAspect;
        
        private ProtoWorld _world;
        
        private ProtoItExc _filter= It
            .Chain<StartNetworkClientSelfRequest>()
            .Exc<InitializeNetcodeSelfRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .Inc<FishNetClientManagerComponent>()
            .Inc<FishNetTransportComponent>()
            .End();

        public void Run()
        {
            foreach (var entity in _filter)
            {
                ref var request = ref _clientAspect.Connect.Get(entity);
                
                var address = request.Address;
                var port = request.Port;

                var netcoreResult = _netFilter.First();
                if (!netcoreResult.Ok)
                {
                    ref var initializeComponent = ref _netcodeAspect
                        .InitializeSelf.GetOrAddComponent(entity);
                    continue;
                }

                var netcodeEntity = netcoreResult.Entity;
                if (request.StartNetwork)
                {
                    ref var managerComponent = ref _netcodeAspect.Manager.Get(netcodeEntity);
                    ref var transportComponent = ref _netcodeAspect.Transport.Get(netcodeEntity);

                    var manager = managerComponent.Value;
                    var connectionInfo = transportComponent.Value;

                    if (manager.IsClientStarted)
                        continue;

                    connectionInfo.Address = address;
                    connectionInfo.Port = (ushort)port;

                    //start server
                    var clientManager = manager.ClientManager;
                    var result = clientManager.StartConnection(address,port);

                    if (!result)
                    {
                        GameLog.LogError($"Failed to start client for address: {address} | port: {port}");
                        continue;
                    }

                    GameLog.Log($"Successfully started client for address: {address} | port: {port}");
                }

                var packedNetEntity = _world.PackEntity(netcodeEntity);
                ref var linkComponent = ref _networkAspect.NetworkLink.GetOrAddComponent(entity);
                linkComponent.Value = packedNetEntity;
                
                _clientAspect.Connect.Del(entity);
            }
        }

    }
}