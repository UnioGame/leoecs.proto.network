namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsProto;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UnityNetcode.Componenets.Requests;
    using UnityNetcode.Components;

    /// <summary>
    /// shared network aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetworkAspect : EcsAspect
    {
        public ProtoPool<NetworkLinkComponent> NetworkLink;
        public ProtoPool<NetworkSourceComponent> NetworkSource;
        public ProtoPool<NetworkAddressComponent> Address;
        public ProtoPool<NetcodeStatusComponent> Status;
        public ProtoPool<NetcodeAgentComponent> NetworkAgent;
        
        public ProtoPool<NetworkConnectionTypeComponent> ConnectionType;
        public ProtoPool<EcsNetworkConnectionInfoComponent> ConnectionInfo;
        public ProtoPool<NetcodeMessageSenderId> SenderId;
        //netcode runtime info
        //public EcsPool<NetworkActiveComponent> Active;
        
        //server time
        public ProtoPool<NetworkTimeComponent> NetworkTime;
        
        //requests
        public ProtoPool<InitializeNetcodeSelfRequest> InitializeNetcode;
        
        // create new host
        public ProtoPool<StartNetworkSelfRequest> StartNetwork;
        public ProtoPool<StopNetworkSelfRequest> StopNetwork;

        /// <summary>
        /// server connected event
        /// </summary>
        public ProtoPool<NetworkServerConnectedSelfEvent> ServerConnected;
    }
}