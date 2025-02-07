namespace Game.Ecs.Network.Shared.Data
{
    using System;
    using System.Buffers;
    using Leopotam.EcsProto;
    using Unity.Collections;

    public interface IEcsTypeSerializer
    {
        bool Serialize(ProtoWorld world, ProtoEntity entity,IBufferWriter<byte> writer);

        int Serialize(ProtoWorld world, ProtoEntity entity, ref NativeArray<byte> stream,int offset);

        int Deserialize(ProtoWorld world, ProtoEntity entity, ref ReadOnlySpan<byte> buffer);

        int Deserialize(ProtoWorld world, ProtoEntity entity, ref NativeSlice<byte> buffer);

        int Deserialize(ProtoWorld world, ProtoEntity entity, ref NativeArray<byte> buffer, int offset);
    }
}