namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using Aspects;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetworkCommands.Components;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

    /// <summary>
    /// send message with base rpc channel
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public sealed class UpdateActiveHistoryPointSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private NetcodeMessageAspect _rpcAspect;

        private ProtoWorld _world;

        private ProtoIt _netcodeFilter= It
            .Chain<NetworkSourceComponent>()
            .Inc<NetworkTimeComponent>()
            .End();
        
        private ProtoIt _historyFilter= It
            .Chain<NetworkHistoryComponent>()
            .End();
        
        private ProtoEntity _historyEntity;
        private NetworkData _networkSettings;

        public void Run()
        {
            var netcodeEntityOk = _netcodeFilter.First();
            if (!netcodeEntityOk.Ok) return;

            var historyEntityOk = _historyFilter.First();
            if (!historyEntityOk.Ok) return;

            _historyEntity = historyEntityOk.Entity;
            ref var historyComponent = ref _rpcAspect.History.Get(_historyEntity);
            ref var timeComponent = ref _networkAspect.NetworkTime.Get(netcodeEntityOk.Entity);

            var time = timeComponent.Time;
            var tick = (int)timeComponent.Tick;
            var historyTick = historyComponent.Tick;

            if (tick == historyTick) return;

            var historyLength = historyComponent.History.Length;
            var index = historyComponent.Index;
            var lastIndex = index;
            index = tick % historyLength;

            ref var historyData = ref historyComponent.History[index];

            historyData.EntityMap.Clear();

            historyData.Size = 0;
            historyData.Count = 0;
            historyData.Offset = 0;
            historyData.Tick = tick;
            historyData.Time = time;
            historyData.PreviousTick = historyComponent.Tick;

            historyComponent.Tick = tick;
            historyComponent.Index = index;
            historyComponent.LastIndex = lastIndex;
            historyComponent.Time = time;
        }
    }
}