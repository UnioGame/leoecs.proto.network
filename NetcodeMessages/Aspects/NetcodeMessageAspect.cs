namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Aspects
{
    using System;
    using Components;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UnityNetcode.Components;

    /// <summary>
    /// netcode rpc aspect
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetcodeMessageAspect : EcsAspect
    {
        public ProtoPool<NetworkMessageChannelSource> Source;
        public ProtoPool<NetcodeMessageChannelComponent> Channel;
        public ProtoPool<NetworkSerializationResult> SerializationResult;
        public ProtoPool<NetworkSyncComponent> ServerEntity;
        public ProtoPool<ReceivedMessageComponent> ReceivedMessage;
        public ProtoPool<NetcodeMessageSenderId> SenderId;
        
        /// <summary>
        /// history of sync values during several ticks
        /// </summary>
        public ProtoPool<NetworkHistoryComponent> History;
        
        /// <summary>
        /// network value id
        /// </summary>
        public ProtoPool<NetworkSyncValuesComponent> SyncValues;
        
        //data to receive
        public ProtoPool<NetworkMessageDataComponent> MessageData;
        
        //=== requests ===
        
        //request to serialize current ecs data to history
        public ProtoPool<NetworkSerializeRequest> Serialize;
    }
}