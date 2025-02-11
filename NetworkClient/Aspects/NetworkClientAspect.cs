namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// network client aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class NetworkClientAspect : EcsAspect
    {
        //base network client marker
        public ProtoPool<NetworkClientComponent> Client;
        //id of client
        public ProtoPool<NetworkClientIdComponent> ClientId;
        //link to network transport
        public ProtoPool<NetworkSourceLinkComponent> NetworkLink;
        //connection type data
        public ProtoPool<NetworkConnectionTypeComponent> Connection;
        
        //=== optional ===
        //mark client as local
        public ProtoPool<NetworkLocalClientComponent> Local;
        //mark client as master
        public ProtoPool<NetworkMasterClientComponent> Master;
        
        //=== requests ===
        
        /// <summary>
        /// Connect to server as a client
        /// </summary>
        public ProtoPool<StartNetworkClientRequest> Connect;
        
        //=== events ====
        
        /// <summary>
        /// send when client connected to server
        /// </summary>
        public ProtoPool<NetworkClientConnectedSelfEvent> Connected;

        /// <summary>
        /// send when client disconnected from server
        /// </summary>
        public ProtoPool<NetworkClientDisconnectedEvent> Disconnected;
    }
}