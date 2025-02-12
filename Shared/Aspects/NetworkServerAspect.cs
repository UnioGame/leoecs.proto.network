namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Components;
    using Leopotam.EcsProto;
    using Modules.leoecs.proto.network.Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UnityNetcode.Components;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class NetworkServerAspect : EcsAspect
    {
        public ProtoPool<NetworkServerActiveComponent> Active;
        public ProtoPool<NetworkSourceComponent> NetworkSource;
        public ProtoPool<NetworkAddressComponent> Address;
        public ProtoPool<NetcodeStatusComponent> Status;
        public ProtoPool<NetcodeAgentComponent> NetworkAgent;
        public ProtoPool<NetworkConnectionTypeComponent> ConnectionType;
        public ProtoPool<NetworkConnectionInfoComponent> ConnectionInfo;
        
        //server time
        public ProtoPool<NetworkTimeComponent> NetworkTime;
    }
}