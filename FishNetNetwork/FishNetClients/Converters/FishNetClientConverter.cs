namespace Game.Ecs.Network.UnityNetcode.NetcodeClients.Converters
{
    using System;
    using Components;
    using FishNet.Object;
    using Leopotam.EcsProto;
    using Modules.leoecs.proto.network.Shared.Components;
    using Shared.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;
    using UnityNetcode.Components;

    /// <summary>
    /// convert GameObject to NetworkClientComponent
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Serializable]
    public class FishNetClientConverter : GameObjectConverter
    {
        protected override void OnApply(GameObject target, ProtoWorld world, ProtoEntity entity)
        {
            var networkObject = target.GetComponent<NetworkObject>();
            ref var networkObjectComponent = ref world.AddComponent<NetcodeClientObjectComponent>(entity);
            ref var networkClientComponent = ref world.AddComponent<NetworkClientComponent>(entity);
            ref var networkConnectionTypeComponent = ref world.AddComponent<NetworkConnectionTypeComponent>(entity);
            ref var networkClientIdComponent = ref world.AddComponent<NetworkClientIdComponent>(entity);
            ref var connectionInfoComponent = ref world.AddComponent<NetworkConnectionInfoComponent>(entity);
            ref var ownerIdComponent = ref world.AddComponent<NetworkOwnerIdComponent>(entity);
            
            networkObjectComponent.Value = networkObject;
        }
    }
}