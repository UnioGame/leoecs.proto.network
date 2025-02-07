namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Aspects;
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
    /// send rpc with base rpc source
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class SendCommonRPCSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        
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
                
                foreach (var netcodeEntity in _netFilter)
                {
                    ref var managerComponent = ref _netcodeAspect.Manager.Get(netcodeEntity);
                    ref var transportComponent = ref _netcodeAspect.Transport.Get(netcodeEntity);

                    var manager = managerComponent.Value;
                    var transport = transportComponent.Value;
                
                    if(manager.IsServerStarted || manager.IsHostStarted) continue;
                    
                    transport.Address = address;
                    transport.Port = (ushort)port;

                    var serverManager = manager.ServerManager;
                    //start server
                    var result = serverManager.StartConnection(port);
                    
                    if(!result)
                    {
                        GameLog.LogError($"Failed to start host for address: {address} | port: {port}");
                        continue;
                    }
                    
                    var packedNetEntity = _world.PackEntity(netcodeEntity);
                    ref var linkComponent = ref _networkAspect.NetworkLink.GetOrAddComponent(entity);
                    linkComponent.Value = packedNetEntity;
                    
                    break;
                }

                _networkAspect.StartNetwork.Del(entity);
            }
        }

    }
}