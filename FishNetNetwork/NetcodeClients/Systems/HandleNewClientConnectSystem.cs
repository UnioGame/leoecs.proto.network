namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using Shared.Aspects;
    using Shared.Components.Events;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

    /// <summary>
    /// handle new client connect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class HandleNewClientConnectSystem : IEcsRunSystem
    {
        private ProtoWorld _world;

        private NetworkClientAspect _clientAspect;
        private NetworkCommandsAspect _networkMessage;
        
        private ProtoIt _filter = It
            .Chain<NetworkClientConnectedSelfEvent>()
            .End();

        public void Run()
        {
            foreach (var entity in _filter)
            {
                ref var resendComponent = ref _networkMessage
                    .ForceResend
                    .GetOrAddComponent(entity);

                ref var clientComponent = ref _clientAspect.ClientId.Get(entity);
                resendComponent.ClientId = (int)clientComponent.Id;
            }
        }
    }
}