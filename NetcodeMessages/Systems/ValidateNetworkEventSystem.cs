namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using Shared.Aspects;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityNetcode.Aspects;

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
    public class ValidateNetworkEventSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        private NetworkMessageAspect _networkMessageAspect;
        
        private ProtoWorld _world;
        
        private ProtoItExc _networkValueFilter= It
            .Chain<NetworkIdComponent>()
            .Inc<NetworkEventComponent>()
            .Exc<NetworkSyncComponent>()
            .Exc<SerializeNetworkEntityRequest>()
            .End();

        public void Run()
        {
            foreach (var valueEntity in _networkValueFilter)
            {
                _networkMessageAspect.SerializeEntity.Add(valueEntity);
            }
        }
    }
}