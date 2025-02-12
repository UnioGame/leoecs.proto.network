namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Aspects
{
    using System;
    using System.Runtime.CompilerServices;
    using Components;
    using FishNet.Connection;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Modules.leoecs.proto.network.Shared.Components;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Components;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityNetcode.Components;

    /// <summary>
    /// network client aspect data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class FishNetClientAspect : EcsAspect
    {
        public ProtoWorld World;
        
        public NetworkClientAspect ClientAspect;
        
        //id of client
        public ProtoPool<NetworkClientIdComponent> ClientId;
        public ProtoPool<FishNetConnectionComponent> Connection;
        public ProtoPool<FishNetClientObjectComponent> ClientObject;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProtoEntity CreateClient(ProtoEntity entity,NetworkConnection connection)
        {
            ref var networkClientComponent = ref World.GetOrAddComponent<NetworkClientComponent>(entity);
            ref var networkLinkComponent = ref World.GetOrAddComponent<NetworkSourceLinkComponent>(entity);
            ref var networkConnectionTypeComponent = ref World.GetOrAddComponent<NetworkConnectionTypeComponent>(entity);
            ref var networkClientIdComponent = ref World.GetOrAddComponent<NetworkClientIdComponent>(entity);
            ref var connectionInfoComponent = ref World.GetOrAddComponent<NetworkConnectionInfoComponent>(entity);
            ref var ownerIdComponent = ref World.GetOrAddComponent<NetworkOwnerIdComponent>(entity);
            ref var connectionComponent = ref World.GetOrAddComponent<FishNetConnectionComponent>(entity);
            
            connectionComponent.Value = connection;
            
            return entity;
        }
    }
}