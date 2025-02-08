namespace Game.Ecs.Network.UnityNetcode
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Data;
    using Profiler;
    using Shared.Components.Events;
    using Shared.Components.Requests;
    using Shared.Data;
    using Systems;
    using UniGame.AddressableTools.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [CreateAssetMenu(menuName = "ECS Proto/Features/Network/Netcode Feature",fileName = "Network Feature")]
    public class NetworkProtoFeature : BaseLeoEcsFeature
    {
        public AssetReferenceT<EcsNetworkSettingsAsset> networkSettings;
        
        public NetworkEcsProfilerFeature profilerFeature = new();
        
        [SerializeReference]
        public IEcsNetworkModuleFeature[] networkModules = Array.Empty<IEcsNetworkModuleFeature>();
        
        public sealed override async UniTask InitializeAsync(IProtoSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();
            var lifeTime = world.GetWorldLifeTime();

            var settingsAsset = await networkSettings
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            
            var settings = settingsAsset.networkSettings;
            var networkData = settings.networkData;
            var typesCount = networkData.networkTypes.Length;
            
            //enable network profiler if activated in settings
            if(settings.enableProfiler)
                await profilerFeature.InitializeAsync(ecsSystems);
            
            var ecsNetworkData = new EcsNetworkData()
            {
                Types = new NetworkSyncType[typesCount],
                IdTypeMap = new Type[typesCount],
                ClientIsTypeMap = new Type[typesCount],
                ClientTypes = new NetworkSyncType[typesCount],
            };
            
            foreach (var syncType in networkData.networkTypes)
            {
                var type = syncType.type;
                var id = syncType.id;
                ecsNetworkData.Types[id] = syncType;
                ecsNetworkData.SyncTypes[id] = BitConverter.GetBytes(id);
                ecsNetworkData.IdTypeMap[id] = type;
                // ecsNetworkData.TypesMap.Add(type, id);
                ecsNetworkData.TypesMap[type] = id;
            }

            foreach (var clientType in networkData.clientTypes)
            {
                ecsNetworkData.ClientTypes[clientType.id] = clientType;
                ecsNetworkData.ClientIsTypeMap[clientType.id] = clientType.type;
                ecsNetworkData.ClientTypeMap[clientType.type] = clientType.id;
                ecsNetworkData.ClientIdTypeMap[clientType.id] = clientType.id;
            }
            
            //set global settings of network configuration
            world.SetGlobal(ecsNetworkData);
            world.SetGlobal(settingsAsset.assetsSettings);
            world.SetGlobal(settings);
            world.SetGlobal(networkData);
            
            //if get request to start network and netcode not initialized - start netcode
            ecsSystems.Add(new StartNetcodeOnNonInitializedSystem());
            //link request entity ot network agent
            ecsSystems.Add(new InitializeNetcodeSystem());

            //remove server connected event
            ecsSystems.DelHere<NetworkServerConnectedSelfEvent>();
            
            //start netcode server and fire server connected if success
            ecsSystems.Add(new StartNetcodeServerSystem());
            //stop netcode server
            ecsSystems.Add(new StopNetcodeSystem());
            ecsSystems.Add(new UpdateNetcodeStatusSystem());
            ecsSystems.Add(new UpdateNetcodeTimeSystem());
            
            //send rpc commands
            ecsSystems.Add(new SendCommonRPCSystem());
            
            foreach (var moduleFeature in networkModules)
            {
                await moduleFeature.InitializeAsync(ecsSystems);
            }
            
            //remove stop request
            ecsSystems.DelHere<StopNetworkSelfRequest>();
        }
    }

}