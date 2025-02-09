namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

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
    public class ValidateForceResendNetworkSystem : IEcsRunSystem
    {
        private NetworkCommandsAspect _messageAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _transferRequestFilter= It
            .Chain<SerializeNetworkEntityRequest>()
            .End();
        
        private ProtoIt _forceResendFilter= It
            .Chain<NetworkForceResendRequest>()
            .End();

        public void Run()
        {
            if(!_transferRequestFilter.First().Ok) return;
            
            var forceResend = false;
            
            foreach (var resendEntity in _forceResendFilter)
            {
                forceResend = true;
                _messageAspect.ForceResend.Del(resendEntity);
            }
            
            if(!forceResend) return;
            
            foreach (var entity in _transferRequestFilter)
            {
                ref var transferRequest = ref _messageAspect.SerializeEntity.Get(entity);
                transferRequest.Force = true;
            }
        }
    }
}