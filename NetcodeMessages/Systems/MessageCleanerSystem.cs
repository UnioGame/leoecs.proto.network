namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class MessageCleanerSystem : IEcsRunSystem
    {
        private ProtoWorld _world;
        
        private ProtoIt _receivedMessagesFilter= It
            .Chain<ReceivedMessageComponent>()
            .End();

        public void Run()
        {
            foreach (var receivedMessageEntity in _receivedMessagesFilter)
            {
                _world.DelEntity(receivedMessageEntity);
            }
        }
    }
}