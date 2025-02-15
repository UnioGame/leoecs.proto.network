namespace Game.Modules.leoecs.proto.network.Network.Server.Aspects
{
    using System;
    using Components.Requests;
    using Ecs.Network.Shared.Components;
    using Ecs.Network.Shared.Components.Events;
    using Ecs.Network.Shared.Components.Requests;
    using Ecs.Network.UnityNetcode.Components;
    using Leopotam.EcsProto;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// ADD DESCRIPTION HERE
    /// </summary>
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
        
        //requests
        public ProtoPool<InitializeServerRequest> Initialize;
        // create new host
        public ProtoPool<StartServerRequest> StartServer;
        public ProtoPool<StopServerRequest> StopServer;
        
        /// <summary>
        /// server connected event
        /// </summary>
        public ProtoPool<NetworkServerConnectedSelfEvent> ServerConnected;
    }
}