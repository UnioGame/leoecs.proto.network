namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

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
    public class StopNetcodeSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        
        private ProtoWorld _world;
        private bool _isLoading;
        
        private ProtoIt _networkFilter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
        private ProtoIt _filter= It
            .Chain<StopNetworkSelfRequest>()
            .End();

        public void Run()
        {
            foreach (var entity in _filter)
            {
                foreach (var netEntity in _networkFilter)
                {
                    ref var managerComponent = ref _networkAspect.NetworkSource.Get(netEntity);
                    var manager = managerComponent.Value;
                    //stop host
                    if (manager.IsHostStarted || manager.IsServerStarted)
                        manager.StopServer(true);
                }
            }
        }

    }
}