namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Components;
    using Shared.Aspects;
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
    public class RemoveNetworkReceivedDataSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeMessageAspect _messageAspect;
        
        private ProtoWorld _world;
        private ProtoIt _filter= It
            .Chain<NetworkMessageDataComponent>()
            .End();
        
        private ProtoIt _messageFilter= It
            .Chain<NetworkReceiveResultComponent>()
            .End();

        public void Run()
        {
            foreach (var entity in _filter)
                _world.DelEntity(entity);
            
            foreach (var entity in _messageFilter)
                _world.DelEntity(entity);
        }
    }
}