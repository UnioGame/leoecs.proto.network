namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Systems
{
    using System;
    using System.Buffers;
    using Aspects;
    using Leopotam.EcsLite;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Network.Serializer;
    using NetworkCommands.Aspects;
    using NetworkCommands.Components;
    using NetworkCommands.Components.Requests;
    using NetworkCommands.Data;
    using Shared.Aspects;
    using Shared.Components;
    using Shared.Data;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;
    using UniGame.LeoEcs.Shared.Extensions;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityNetcode.Aspects;
    using UnityNetcode.Components;

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
    public class SerializeNetworkDataSystem : IEcsRunSystem
    {
        private NetworkAspect _networkAspect;
        private FishNetAspect _netcodeAspect;
        private NetcodeMessageAspect _netcodeMessageAspect;
        private NetworkMessageAspect _networkMessageAspect;

        private ProtoWorld _world;

        private EcsNetworkSettings _networkSettings;

        private ProtoIt _netcodeFilter= It
            .Chain<NetcodeManagerComponent>()
            .Inc<NetworkConnectionTypeComponent>()
            .Inc<NetworkTimeComponent>()
            .End();
        
        private ProtoIt _historyFilter= It
            .Chain<NetworkHistoryComponent>()
            .End();
        
        private ProtoIt _transferRequestFilter= It
            .Chain<NetworkTransferRequest>()
            .End();
        
        private NativeArray<byte> _arrayBuffer;
        private ArrayBufferWriter<byte> _arrayBufferWriter;
        private int _headerSize;
        private int _entityHeaderSize;

        public SerializeNetworkDataSystem()
        {
            _arrayBuffer = new NativeArray<byte>(512,Allocator.Persistent);
            _headerSize = UnsafeUtility.SizeOf<NetworkHeader>();
            _entityHeaderSize = UnsafeUtility.SizeOf<NetworkEntityHeader>();
            _arrayBufferWriter = new ArrayBufferWriter<byte>(512);
        }

        public void Run()
        {
            var serializeEntityOk = _transferRequestFilter.First();
            if (!serializeEntityOk.Ok) return;
            
            var netcodeEntityOk = _netcodeFilter.First();
            if (!netcodeEntityOk.Ok) return;

            var historyEntityOk = _historyFilter.First();
            if (!historyEntityOk.Ok) return;

            ref var historyComponent = ref _netcodeMessageAspect.History.Get(historyEntityOk.Entity);
            ref var timeComponent = ref _netcodeAspect.NetworkTime.Get(netcodeEntityOk.Entity);

            ref var history = ref historyComponent.History;
            var index = historyComponent.Index;
            var previousIndex = historyComponent.LastIndex;
            var previousData = historyComponent.History[previousIndex];
            
            ref var historyData = ref history[index];
            ref var byteData = ref historyData.SerializedData;
            var useHashFiltering = _networkSettings.useHashFiltering;
            var syncCount = historyData.EntityMap.Count;
            
            if(syncCount == 0 && previousData.EntityMap.Count == syncCount) return;

            _arrayBufferWriter.Clear();
            
            var resultMaxSize = byteData.Length + _headerSize;
            if (_arrayBuffer.Length < resultMaxSize)
            {
                _arrayBuffer.Dispose();
                _arrayBuffer = new NativeArray<byte>(resultMaxSize, Allocator.Persistent);
            }

            var offset = _headerSize;
            var byteArray = historyData.SerializedData;
            var readonlySpan = byteArray.AsReadOnlySpan();
            var componentsCount = 0;
            
            foreach (var historyPair in  historyData.EntityMap)
            {
                ref var value = ref historyPair.Value;
                componentsCount += value.Count;

                var writeSize = value.IsValueChanged ? value.Size : _entityHeaderSize;
                var slice = readonlySpan.Slice(value.Offset,writeSize);
                offset += _arrayBuffer.WriteData(ref slice, offset);
            }
            
            var header = new NetworkHeader()
            {
                Tick = historyComponent.Tick,
                Time = timeComponent.Time,
                Count = syncCount,
                Size = offset,
                Components = componentsCount,
                IsHashed = useHashFiltering,
            };

            _arrayBuffer.Serialize(ref header,0);
            var size = offset;
            
            ref var serializeResult = ref _netcodeMessageAspect
                .SerializationResult.Add(serializeEntityOk.Entity);
            serializeResult.Value =  _arrayBuffer;
            serializeResult.Size = size;
        }
       
    }
    
   
}