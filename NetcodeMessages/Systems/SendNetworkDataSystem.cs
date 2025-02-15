namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Components;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
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
    public class SendNetworkDataSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetworkCommandsAspect _networkMessageAspect;
        private NetcodeMessageAspect _messageAspect;
        
        private ProtoWorld _world;
        private EcsNetworkSettings _networkSettings;
        private object[] _components;

        private ProtoIt _netcodeFilter= It
            .Chain<NetworkSourceComponent>()
            .Inc<NetworkTimeComponent>()
            .End();

        private ProtoIt _transferFilter = It
            .Chain<NetworkTransferRequest>()
            .Inc<NetworkSerializationResult>()
            .End();
        
        private ProtoIt _filter= It
            .Chain<NetcodeMessageChannelComponent>()
            .End();
        
        private ProtoIt _historyFilter= It
            .Chain<NetworkHistoryComponent>()
            .End();

        public void Run()
        {
            var transferEntityOk = _transferFilter.First();
            if (!transferEntityOk.Ok) return;
            
            var transferEntity = transferEntityOk.Entity;
            ref var transferComponent = ref _networkMessageAspect.Transfer.Get(transferEntity);
            ref var targetComponent = ref _networkMessageAspect.Target.Get(transferEntity);
            ref var seializationResult = ref _messageAspect.SerializationResult.Get(transferEntity);
            
            var netcodeEntityOk = _netcodeFilter.First();
            if (!netcodeEntityOk.Ok) return;
            
            var rpcEntityOk = _filter.First();
            if (!rpcEntityOk.Ok) return;
            
            var historyEntityOk = _historyFilter.First();
            if (!historyEntityOk.Ok) return;
            
            ref var channel = ref _messageAspect.Channel.Get(rpcEntityOk.Entity);
            var channelObject = channel.Value;

            ref var resultArray = ref seializationResult.Value;
            var targetArray = ArrayPool<byte>.Shared.Rent(seializationResult.Size);
            var size = seializationResult.Size;
            
            resultArray.AsSpan()
                .Slice(0,size)
                .CopyTo(targetArray);
            
            //TODO FIX MESSAGE SENDING
            //var target = channelObject.GetRpcTarget(targetComponent.Value, targetComponent.Id);
            //var connection = channelObject.ClientManager.Connection;
            //channelObject.SendToClientRPC(connection,targetArray,size,target);
            
            ArrayPool<byte>.Shared.Return(targetArray);
            
            var sendEvent = _world.NewEntity();
            ref var sendComponent = ref _networkMessageAspect.DataSendEvent.Add(sendEvent);
            sendComponent.Size = size;
            sendComponent.TargetType = targetComponent.Value;
        }
    }
}