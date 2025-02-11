namespace Game.Ecs.Network.UnityNetcode.Converters
{
    using System;
    using Components;
    using Data;
    using FishNet.Managing;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UniGame.LeoEcsLite.LeoEcs.Shared.Components;
    using UniModules.UniGame.Core.Runtime.DataFlow.Extensions;
    using Unity.Collections;
    using UnityEngine;

    /// <summary>
    /// converter for netcode prefab to ecs world
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class FishNetConverter : GameObjectConverter
    {
        public NetworkManager networkManager;
        
        protected override void OnApply(GameObject target, ProtoWorld world, ProtoEntity entity)
        {
            ref var networkTime = ref world.GetOrAddComponent<NetworkTimeComponent>(entity);
            ref var agentComponent = ref world.GetOrAddComponent<NetcodeAgentComponent>(entity);
            ref var networkManagerComponent = ref world.GetOrAddComponent<NetcodeManagerComponent>(entity);
            ref var netcodeStatusComponent = ref world.GetOrAddComponent<NetcodeStatusComponent>(entity);
            ref var unityTransportComponent = ref world.GetOrAddComponent<EcsNetworkConnectionInfoComponent>(entity);
            ref var targetComponent = ref world.GetOrAddComponent<NetcodeSharedRPCComponent>(entity);
            ref var networkSourceComponent = ref world.GetOrAddComponent<NetworkSourceComponent>(entity);
            ref var connectionTypeComponent = ref world.GetOrAddComponent<NetworkConnectionTypeComponent>(entity);
            ref var syncValuesComponent = ref world.GetOrAddComponent<NetworkSyncValuesComponent>(entity);
            ref var lifeTimeComponent = ref world.GetOrAddComponent<LifeTimeComponent>(entity);
            
            var ecsNetworkManager = new FishNetNetworkManager(networkManager);
            networkSourceComponent.Value = ecsNetworkManager;

            agentComponent.Id = ecsNetworkManager.ActiveClientId;
            
            networkManagerComponent.Value = networkManager;
            targetComponent.Value = target;

            netcodeStatusComponent.Status = ConnectionStatus.Disconnected;
            netcodeStatusComponent.IsConnected = false;
            netcodeStatusComponent.IsInRoom = false;
            
            networkTime.Time = networkManager.TimeManager.ServerUptime;
            networkTime.Tick = networkManager.TimeManager.Tick;

            var lifeTime = target.GetAssetLifeTime();
            syncValuesComponent.Values = 
                new NativeHashMap<int, ProtoPackedEntity>(128, Allocator.Persistent)
                    .AddTo(lifeTime);
        }

        
    }
}