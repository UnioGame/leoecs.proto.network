namespace Game.Ecs.Network.UnityNetcode
{
    using System;
    using Componenets.Requests;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Modules.leoecs.proto.network.Network.Server;
    using NetcodeMessages;
    using NetworkCommands.Data;
    using Profiler;
    using Shared.Components.Events;
    using Shared.Components.Requests;
    using Shared.Data;
    using Systems;
    using UniGame.AddressableTools.Runtime;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniModules;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
#if UNITY_EDITOR
    using UnityEditor;
    using UniModules.Editor;
    using UniModules.UniGame.AddressableExtensions.Editor;
#endif

    [CreateAssetMenu(menuName = "ECS Proto/Features/Network/Netcode Feature",fileName = "Network Feature")]
    public class NetworkProtoFeature : BaseLeoEcsFeature
    {
        public AssetReferenceT<EcsNetworkSettingsAsset> networkSettings;
        
        public NetworkEcsProfilerFeature profilerFeature = new();
        public NetcodeMessagingFeature messagingFeature = new();
        
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

            //link request entity ot network agent
            ecsSystems.Add(new InitializeNetcodeSystem());
            ecsSystems.DelHere<InitializeNetcodeRequest>();
            
#if ECS_NETWORK_SERVER
            var serverFeature = new NetworkServerFeature();
            await serverFeature.InitializeAsync(ecsSystems);
#endif
            
            ecsSystems.Add(new UpdateNetcodeStatusSystem());
            ecsSystems.Add(new UpdateNetcodeTimeSystem());
            
            //send rpc commands
            ecsSystems.Add(new SendCommonRPCSystem());
            
            foreach (var moduleFeature in networkModules)
            {
                await moduleFeature.InitializeAsync(ecsSystems);
            }

            await messagingFeature.InitializeAsync(ecsSystems);
            
            //remove stop request
            ecsSystems.DelHere<StopServerRequest>();
        }

#if UNITY_EDITOR

#if ODIN_INSPECTOR
        [OnInspectorInit]
#endif
        private void ValidateFeature()
        {
            var settingsExists = networkSettings != null && networkSettings.editorAsset != null;

            if (!settingsExists)
            {
                var settingsAsset = CreateInstance<EcsNetworkSettingsAsset>();
                var path = AssetDatabase.GetAssetPath(this);
                if (!string.IsNullOrEmpty(path))
                {
                    var settingsPath = path.GetDirectoryPath().CombinePath("NetworkSettings.asset");
                    AssetDatabase.CreateAsset(settingsAsset,settingsPath);
                    AssetDatabase.Refresh();
                    settingsAsset = AssetDatabase.LoadAssetAtPath<EcsNetworkSettingsAsset>(settingsPath);
                    settingsAsset.AddToDefaultAddressableGroup();
                    settingsAsset.MarkDirty();
                    var guid = settingsAsset.GetGUID();
                    networkSettings = new AssetReferenceT<EcsNetworkSettingsAsset>(guid);
                    this.MarkDirty();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            
        }

#endif
        
    }

}