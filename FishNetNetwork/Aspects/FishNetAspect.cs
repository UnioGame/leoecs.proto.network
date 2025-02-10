namespace Game.Ecs.Network.UnityNetcode.Aspects
{
    using System;
    using Componenets.Requests;
    using Components;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcsLite.LeoEcs.Shared.Components;

    /// <summary>
    /// netcode aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class FishNetAspect : EcsAspect
    {
        public ProtoPool<NetcodeManagerComponent> Manager;
        public ProtoPool<FishNetClientManagerComponent> ClientManager;
        public ProtoPool<NetcodeSharedRPCComponent> RPCAsset;
        public ProtoPool<EcsNetworkConnectionInfoComponent> Transport;
        public ProtoPool<FishNetServerManagerComponent> ServerManager;
        public ProtoPool<LifeTimeComponent> LifeTime;
        public ProtoPool<NetworkTimeComponent> NetworkTime;
        
        public ProtoPool<NetcodeAgentComponent> Agent;
        public ProtoPool<NetcodeStatusComponent> Status;

        public ProtoPool<NetcodeMessageSenderId> SenderId;
        
        public ProtoPool<NetworkConnectionTypeComponent> ConnectionType;
        //requests
        //initialize netcode and create new entity if not exists
        public ProtoPool<InitializeNetcodeSelfRequest> InitializeSelf;
    }
}