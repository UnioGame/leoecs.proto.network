namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Text;
    using Aspects;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniCore.Runtime.ProfilerTools;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

    /// <summary>
    /// send message with base rpc channel
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class NetcodeDebugStatisticsSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        private NetworkSyncAspect _networkSyncAspect;
        private NetcodeMessageAspect _messageAspect;
        private NetworkMessageAspect _networkMessageAspect;
        
        private ProtoWorld _world;
        private EcsNetworkSettings _networkData;
        private StringBuilder _stringBuilder = new StringBuilder(512);
        
        private ProtoIt _receiveFilter= It
            .Chain<NetworkReceiveResultComponent>()
            .End();
        
        private ProtoIt _networkFilter= It
            .Chain<NetcodeManagerComponent>()
            .Inc<NetworkConnectionTypeComponent>()
            .End();
        
        private ProtoIt _messageFilter= It
            .Chain<NetworkMessageDataComponent>()
            .End();

        public void Run()
        {
            if (!_networkData.enableDebug) return;
            
            var networkEntityOk = _networkFilter.First();
            if (!networkEntityOk.Ok) return;
            
            var networkEntity = networkEntityOk.Entity;
            ref var connection = ref _networkAspect.ConnectionType.Get(networkEntity);
            ref var managerComponent = ref _netcodeAspect.Manager.Get(networkEntity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(networkEntity);
            
            if(!connection.IsActive)return;
            
            
            if(_receiveFilter.Len() == 0 && _messageFilter.Len() == 0) return;
            
            _stringBuilder.Clear();
            
            var tick = timeComponent.Tick;
            var time = timeComponent.Time;

            _stringBuilder.AppendLine($"RECEIVE DATA: TICK: {tick} NET TIME: {time}");
    
            
            foreach (var entity in _messageFilter)
            {
                ref var messageDataComponent = ref _messageAspect.MessageData.Get(entity);
                
                var bytes = messageDataComponent.Size;
                var kb = messageDataComponent.Size / 1024f;
                var mb = kb / 1024f;
                
                _stringBuilder.AppendLine($"RECEIVE: {bytes} bytes | {kb} KB | {mb} MB");
            }
            
            foreach (var entity in _receiveFilter)
            {
                ref var receiveResult = ref _networkMessageAspect.ReceiveResult.Get(entity);
                
                var bytes = receiveResult.Size;
                var kb = receiveResult.Size / 1024f;
                var mb = kb / 1024f;
                
                _stringBuilder.AppendLine($"RECEIVE UNPACKED: {bytes} bytes | {kb} KB | {mb} MB | SYNC_COUNT: {receiveResult.Count}");
            }
            
            GameLog.LogRuntime(_stringBuilder.ToString(),Color.yellow);
        }
    }
}