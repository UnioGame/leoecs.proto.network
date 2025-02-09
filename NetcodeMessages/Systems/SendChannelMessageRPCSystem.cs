namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Components;
    using Extensions;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
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
    public class SendChannelMessageRPCSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeMessageAspect _rpcAspect;
        private NetworkCommandsAspect _messageAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _filter= It
            .Chain<NetcodeMessageChannelComponent>()
            .End();
        
        private ProtoIt _managerFilter = It
            .Chain<NetcodeStatusComponent>()
            .End();
        
        private ProtoIt _requestFilter= It
            .Chain<NetworkMessageRequest>()
            .End();

        public void Run()
        {
            foreach (var requestEntity in _requestFilter)
            {
                ref var request = ref _messageAspect.SendMessage.Get(requestEntity);
                var channelEntity = _managerFilter.First();
                
                if(!channelEntity.Ok ) continue;
                
                ref var status = ref _networkAspect.Status.Get(channelEntity.Entity);
                if(!status.IsConnected) continue;
                
                ref var channel = ref _rpcAspect.Channel.Get(channelEntity.Entity);
                
                var channelObject = channel.Value;
                var connection = channelObject.ClientManager.Connection;
                var target = channelObject.GetRpcTarget(request.Target);
                
                channelObject.SendMessageRPC(connection,request.Data,target);
                
                _messageAspect.SendMessage.Del(requestEntity);
            }
        }
    }
}