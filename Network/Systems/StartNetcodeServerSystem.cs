namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Componenets.Requests;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

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
        
        private ProtoWorld _world;
        
        private ProtoItExc _filter= It
            .Chain<StartNetworkSelfRequest>()
            .Exc<InitializeNetcodeSelfRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
        private bool _isLoading;

        public void Run()
        {
            foreach (var entity in _filter)
            {
                ref var request = ref _networkAspect.StartNetwork.Get(entity);
                
                var address = request.Address;
                var port = request.Port;

                var netcodeEntityResult = _netFilter.First();
                if (!netcodeEntityResult.Ok)
                    continue;

                var netcodeEntity = netcodeEntityResult.Entity;
                ref var networkSource = ref _networkAspect.NetworkSource.Get(netcodeEntity);
                ref var connectionInfoComponent = ref _networkAspect.ConnectionInfo.Get(netcodeEntity);

                var manager = networkSource.Value;
                var transport = connectionInfoComponent.Value;
                
                if(manager.IsServerStarted || manager.IsHostStarted) continue;

                transport.Address = address;
                transport.Port = port;
                //start server
                var connected = manager.StartServer(port);
                    
                if(!connected)
                {
                    GameLog.LogError($"Failed to start host for address: {address} | port: {port}");
                    continue;
                }

                var mode = request.AllowHostMode ? "host mode" : "server mode";
                GameLog.Log($"Successfully started {mode} for address: {address} | port: {port}");
                
                ref var connectedEvent = ref _networkAspect.ServerConnected.Add(netcodeEntity);
                
                var packedNetEntity = _world.PackEntity(netcodeEntity);
                ref var linkComponent = ref _networkAspect.NetworkLink.GetOrAddComponent(entity);
                linkComponent.Value = packedNetEntity;
                
                _networkAspect.StartNetwork.Del(entity);
            }
        }

        private void ClientConnected_Callback(ulong id)
        {
            GameLog.Log($"Client connected with | id: {id}");
        }

    }
}