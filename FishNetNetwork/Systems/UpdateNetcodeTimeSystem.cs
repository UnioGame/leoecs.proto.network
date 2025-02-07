namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Aspects;
    using Components;
    using Data;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;

    /// <summary>
    /// initialize netcode data
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class UpdateNetcodeTimeSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        
        private ProtoWorld _world;
        private EcsFilter _netFilter;
        
        private UnityNetcodeSettings _netcodeSettings;
        private EcsNetworkSettings _networkSettings;
        private bool _isLoading;
        
        private ProtoIt _filter= It
            .Chain<NetcodeManagerComponent>()
            .End();

        public void Run()
        {
            foreach (var entity in _filter)
            {
               ref var managerComponent = ref _netcodeAspect.Manager.Get(entity);
               ref var timeComponent = ref _networkAspect.NetworkTime.Get(entity);
               
               var manager = managerComponent.Value;
               
               timeComponent.Time = manager.TimeManager.ServerUptime;
               timeComponent.Tick = manager.TimeManager.Tick;
            }
        }

    }
}