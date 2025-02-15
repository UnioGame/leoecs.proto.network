namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Components;
    using Data;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using Shared.Components;
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
        private NetcodeMessageAspect _messageAspect;
        private NetworkCommandsAspect _commandsAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _managerFilter = It
            .Chain<NetcodeStatusComponent>()
            .Inc<NetworkSourceComponent>()
            .Inc<NetcodeMessageChannelComponent>()
            .End();
        
        private ProtoIt _requestFilter= It
            .Chain<NetworkMessageRequest>()
            .End();

        public void Run()
        {
            foreach (var requestEntity in _requestFilter)
            {
                ref var request = ref _commandsAspect.SendMessage.Get(requestEntity);
                var channelEntity = _managerFilter.First();
                
                if(!channelEntity.Ok ) continue;
                
                ref var status = ref _networkAspect.Status.Get(channelEntity.Entity);
                if(!status.IsConnected) continue;
                
                ref var channel = ref _messageAspect.Channel.Get(channelEntity.Entity);
                
                var channelObject = channel.Value;

                //TODO make real message parameters
                channelObject.SendMessage(request.Data, request.Data.Length, new NetworkMessageParams());
                
                _commandsAspect.SendMessage.Del(requestEntity);
            }
        }
    }
}