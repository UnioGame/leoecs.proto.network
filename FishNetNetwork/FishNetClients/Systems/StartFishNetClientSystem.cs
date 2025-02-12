namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Componenets.Requests;
    using Data;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Events;
    using Shared.Components.Requests;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;
    using FishNetClientAspect = Aspects.FishNetClientAspect;

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
    public class StartFishNetClientSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkClientAspect _clientAspect;
        
        private FishNetAspect _fishnetAspect;
        private FishNetClientAspect _netcodeClientAspect;
        
        private ProtoWorld _world;
        
        private ProtoItExc _startFilter= It
            .Chain<StartClientNetworkRequest>()
            .Exc<InitializeNetcodeRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .Inc<FishNetClientManagerComponent>()
            .Inc<FishNetManagerComponent>()
            .Inc<NetworkConnectionInfoComponent>()
            .End();

        public void Run()
        {
            var startOk = _startFilter.First();
            if (startOk.Ok)
            {
                var netcoreResult = _netFilter.First();
                if (!netcoreResult.Ok) return;

                var networkEntity = netcoreResult.Entity;
                ref var networkManagerComponent = ref _fishnetAspect.Manager.Get(networkEntity);
                var networkManager = networkManagerComponent.Value;

                if (networkManager.IsClientStarted)
                {
                    GameLog.Log(EcsNetworkMessages.ClientAlreadyStarted,Color.red);
                    return;
                }
            
                var entity = startOk.Entity;
                ref var request = ref _clientAspect.StartClient.Get(entity);
                
                var address = request.Address;
                var port = request.Port;
            
                ref var transportComponent = ref _fishnetAspect.Transport.Get(entity);

                var connectionInfo = transportComponent.Value;

                connectionInfo.Address = address;
                connectionInfo.Port = (ushort)port;

                //start server
                var clientManager = networkManager.ClientManager;
                var result = clientManager.StartConnection(address,port);

                if (!result)
                {
                    var errorMessage = $"Failed to start client for address: {address} | port: {port}";
                    GameLog.LogError(errorMessage);
                    ref var errorSelfEvent = ref _clientAspect.ClientError.Add(entity);
                    errorSelfEvent.ErrorCode = NetworkClientErrors.ConnectionFailed;
                    errorSelfEvent.Message = errorMessage;
                }
                else
                {
                    GameLog.Log($"Successfully started client for address: {address} | port: {port}");

                    var packedNetEntity = _world.PackEntity(networkEntity);
                    ref var linkComponent = ref _clientAspect.NetworkLink.GetOrAddComponent(entity);
                    
                    linkComponent.Value = packedNetEntity;
                }
            }
            
            foreach (var reqEntity in _startFilter)
                _clientAspect.StartClient.Del(reqEntity);
        }

    }
}