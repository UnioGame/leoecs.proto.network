namespace Game.Ecs.Network.NetworkCommands.Aspects
{
    using System;
    using Components;
    using Components.Events;
    using Components.Requests;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// rpc aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetworkMessageAspect : EcsAspect
    {
        public ProtoPool<NetworkMessageChannelSource> Source;
        public ProtoPool<NetworkIdComponent> NetworkId;
        public ProtoPool<NetworkSyncValuesComponent> SyncValues;
        public ProtoPool<NetworkTargetComponent> Target;
        public ProtoPool<NetworkHistoryComponent> History;
        public ProtoPool<NetworkSerializationResult> SerializationResult;
        public ProtoPool<NetworkSyncComponent> ServerEntity;
        public ProtoPool<NetworkReceiveResultComponent> ReceiveResult;
        public ProtoPool<NetworkEventComponent> NetworkEvent;
        
        // === requests ===
        
        //request to remove entity from network
        public ProtoPool<NetworkTransferRequest> Transfer;
        public ProtoPool<NetworkMessageRequest> SendMessage;
        public ProtoPool<SerializeNetworkEntityRequest> SerializeEntity;
        public ProtoPool<NetworkForceResendRequest> ForceResend;
        
        //events
        public ProtoPool<EcsNetworkDataSendEvent> DataSendEvent;
    }
}