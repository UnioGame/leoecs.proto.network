namespace Game.Ecs.Network.UnityNetcode.Systems
{
    using System;
    using Componenets.Requests;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Components.Requests;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// send rpc with base rpc source
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class SendCommonRPCSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        
        private ProtoWorld _world;
        
        private ProtoItExc _filter= It
            .Chain<StartNetworkSelfRequest>()
            .Exc<InitializeNetcodeSelfRequest>()
            .End();
        
        private ProtoIt _netFilter= It
            .Chain<NetworkSourceComponent>()
            .End();
        
        private bool _isLoading;

        public void Run()
        {
            foreach (var netcodeEntity in _netFilter)
            {
                ref var managerComponent = ref _networkAspect.NetworkSource.Get(netcodeEntity);
            }
        }

    }
}