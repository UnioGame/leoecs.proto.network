namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

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
    public class ValidateTickNetworkSerializationSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkCommandsAspect _messageAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _filter= It
            .Chain<NetcodeMessageChannelComponent>()
            .End();
        
        private ProtoIt _netcodeFilter = It
            .Chain<NetworkSourceComponent>()
            .Inc<NetworkConnectionTypeComponent>()
            .Inc<NetworkTimeComponent>()
            .End();
        
        private ProtoIt _historyFilter= It
            .Chain<NetworkHistoryComponent>()
            .End();
        
        private ProtoItExc _networkValueFilter= It
            .Chain<NetworkIdComponent>()
            .Exc<NetworkSyncComponent>()
            .End();

        private EcsNetworkSettings _networkSettings;
        
        public void Run()
        {
            var netcodeEntityOk = _netcodeFilter.First();
            if (!netcodeEntityOk.Ok) return;
            
            var rpcEntityOk = _filter.First();
            if (!rpcEntityOk.Ok) return;
            
            var historyEntityOk = _historyFilter.First();
            if(!historyEntityOk.Ok) return;

            var targetEntity = netcodeEntityOk.Entity;
            ref var connectionType = ref _networkAspect.ConnectionType.Get(targetEntity);
            ref var historyComponent = ref _messageAspect.History.Get(targetEntity);
            ref var timeComponent = ref _networkAspect.NetworkTime.Get(targetEntity);
            
            var time = timeComponent.Time;
            var tick = timeComponent.Tick;
            var historyTick = historyComponent.Tick;

            var useTickFilter = _networkSettings.useTickSendingOnClient || connectionType.IsServer;
            if(useTickFilter && tick == historyTick) return;
                      
            var serializeEntity = _world.NewEntity();
            //todo allow target to be a server from client side
            ref var transferRequest = ref _messageAspect.Transfer.Add(serializeEntity);
            ref var serializeComponent = ref _messageAspect.SerializeEntity.Add(serializeEntity);
            ref var targetComponent = ref _messageAspect.Target.Add(serializeEntity);

            NetworkMessageTarget target;
            switch (connectionType.IsServer)
            {
                case true when connectionType.IsClient:
                    break;
            }
            
            targetComponent.Value = connectionType.IsServer switch
            {
                true when connectionType.IsClient => NetworkMessageTarget.All,
                true => _networkSettings.defaultServerTarget,
                false => _networkSettings.defaultClientTarget
            };
                
            transferRequest.Tick = (int)tick;
            transferRequest.Time = time;
        }
    }
}