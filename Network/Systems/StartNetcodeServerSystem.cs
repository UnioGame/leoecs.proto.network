namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Data;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniCore.Runtime.ProfilerTools;
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
    public class StartNetcodeServerSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkServerAspect _serverAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _startServerFilter= It
            .Chain<StartServerRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
        private bool _isLoading;

        public void Run()
        {
            var startRequestOk = _startServerFilter.First();
            if (!startRequestOk.Ok) return;
            
            var netcodeEntityResult = _netFilter.First();
            if (!netcodeEntityResult.Ok) return;
            
            var netcodeEntity = netcodeEntityResult.Entity;
            ref var networkSource = ref _networkAspect.NetworkSource.Get(netcodeEntity);
            ref var connectionInfoComponent = ref _networkAspect.ConnectionInfo.Get(netcodeEntity);
            ref var transport = ref connectionInfoComponent.Value;
            
            //server already started
            var manager = networkSource.Value;
            if (!manager.IsServerStarted)
            {
                var entity = startRequestOk.Entity;
                ref var request = ref _networkAspect.StartServer.Get(entity);

                var address = request.Address;
                var port = request.Port;

                transport.Address = address;
                transport.Port = port;

                //start server
                var connected = manager.StartServer(port);

                if (!connected)
                {
                    var error = string.Format(EcsNetworkMessages.FailedToStartServer, address, port);
                    GameLog.LogError(error);
                    return;
                }

                _serverAspect.Active.Add(netcodeEntity);

                var mode = request.AllowHostMode ? "host mode" : "server mode";
                var message = string.Format(EcsNetworkMessages.SuccessStartedServer, mode, address, port);
                GameLog.LogRuntime(message);

                //fire connected self event notification
                ref var connectedEvent = ref _networkAspect.ServerConnected.Add(netcodeEntity);
            }
            else
            {
                GameLog.LogError(EcsNetworkMessages.ServerAlreadyStarted);
            }

            //remove all requests
            foreach (var startEntity in _startServerFilter)
                _networkAspect.StartServer.Del(startEntity);
        }

        private void ClientConnected_Callback(ulong id)
        {
            GameLog.Log($"Client connected with | id: {id}");
        }

    }
}