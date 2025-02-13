// namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
// {
//     using System;
//     using Components;
//     using FishNet.Object;
//     using Leopotam.EcsLite;
//     using Leopotam.EcsProto;
//     using Leopotam.EcsProto.QoL;
//     using Shared.Aspects;
//     using Shared.Components;
//     using Shared.Components.Events;
//     using UniGame.Core.Runtime;
//     using UniGame.Core.Runtime.Extension;
//     using UniGame.LeoEcs.Shared.Extensions;
//     using UniGame.Runtime.ObjectPool.Extensions;
//     using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
//
//     /// <summary>
//     /// initiaize netcode messaging system
//     /// </summary>
// #if ENABLE_IL2CPP
//     using Unity.IL2CPP.CompilerServices;
//
//     [Il2CppSetOption(Option.NullChecks, false)]
//     [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
//     [Il2CppSetOption(Option.DivideByZeroChecks, false)]
// #endif
//     [Serializable]
//     [ECSDI]
//     public class InitializeNetcodeMessagingSystem : IEcsInitSystem, IEcsRunSystem
//     {
//         private NetworkAspect _networkAspect;
//         private NetworkObject _rpcPrefab;
//         private ILifeTime _lifeTime;
//         private ProtoWorld _world;
//         
//         private ProtoIt _filter = It
//             .Chain<NetworkSourceComponent>()
//             .Inc<NetworkServerConnectedSelfEvent>()
//             .End();
//         
//         private ProtoIt _rpcFilter = It
//             .Chain<NetcodeMessageChannelComponent>()
//             .End();
//
//
//         public InitializeNetcodeMessagingSystem(NetworkObject rpcPrefab)
//         {
//             _rpcPrefab = rpcPrefab;
//         }
//         
//         public void Init(IProtoSystems systems)
//         {
//             _world = systems.GetWorld();
//             _lifeTime = _world.GetLifeTime();
//         }
//
//         public void Run()
//         {
//             if (_rpcFilter.Len() > 0) return;
//
//             var first = _filter.First();
//             if(!first.Ok) return;
//
//             var managerEntity = first.Entity;
//             ref var managerComponent = ref _networkAspect.NetworkSource.Get(managerEntity);
//             var manager = managerComponent.Value;
//             
//             if(!manager.IsInitialized || !manager.IsServerStarted)return;
//
//             var rpcInstanceObject = _rpcPrefab.Spawn()
//                 .DespawnWith(_lifeTime);
//             
//             var rpcInstance = rpcInstanceObject.GetComponent<NetworkObject>();
//             rpcInstance.Spawn();
//         }
//     }
// }