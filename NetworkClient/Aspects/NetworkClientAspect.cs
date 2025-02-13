namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Modules.leoecs.proto.network.Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UnityNetcode.Components;

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
        public ProtoPool<NetworkConnectionTypeComponent> ConnectionType;
        public ProtoPool<NetworkConnectionInfoComponent> ConnectionInfo;
        public ProtoPool<NetworkOwnerIdComponent> OwnerId;
        public ProtoPool<NetworkAuthenticatedComponent> Authenticated;
        
        
        //=== optional ===
        //mark client as local
        public ProtoPool<NetworkLocalClientComponent> LocalClient;
        //mark client as master host
        public ProtoPool<NetworkMasterClientComponent> Master;
        
        //=== requests ===
        
        /// <summary>
        /// Connect to server as a client
        /// </summary>
        public ProtoPool<StartClientNetworkRequest> StartClient;
        
        //=== events ====
        
        /// <summary>
        /// send when client connected to server
        /// </summary>
        public ProtoPool<NetworkClientConnectedSelfEvent> Connected;

        /// <summary>
        /// send when client disconnected from server
        /// </summary>
        public ProtoPool<NetworkClientDisconnectedEvent> Disconnected;
        public ProtoPool<NetworkClientErrorSelfEvent> ClientError;
    }
}