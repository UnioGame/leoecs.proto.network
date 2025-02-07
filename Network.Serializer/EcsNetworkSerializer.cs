namespace Game.Ecs.Network.Network.Serializer
{
    using System;
    using System.Buffers;
    using System.Runtime.CompilerServices;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine;

#if ENABLE_MEMORY_PACK    
    using MemoryPack;
#endif

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public static class EcsNetworkSerializer
    {
        public const string EnableMemoryPackMessage= "MemoryPack is not enabled with ENABLE_MEMORY_PACK define not Blittable serialization detected!";
        
        [ThreadStatic]
        private static ArrayBufferWriter<byte> _arrayBufferWriter;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref DataStreamWriter writer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (!isBlittable)
                {
#if ENABLE_MEMORY_PACK  
                    _arrayBufferWriter ??= new ArrayBufferWriter<byte>();
                    _arrayBufferWriter.Clear();
                    MemoryPackSerializer.Serialize(_arrayBufferWriter, value, MemoryPackSerializerOptions.Utf16);
                    var span = _arrayBufferWriter.WrittenSpan;
                    return writer.WriteData(ref span);
#endif
                    Debug.LogWarning(EnableMemoryPackMessage);
                    return default;
                }
                
                var length = UnsafeUtility.SizeOf<TValue>();
                writer.WriteData(ref value);
    
                return length;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ref NativeArray<byte> writer,ref TValue value,int offset)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (isBlittable) return writer.WriteData(ref value,offset);
                
#if ENABLE_MEMORY_PACK   
                _arrayBufferWriter ??= new ArrayBufferWriter<byte>(512);
                _arrayBufferWriter.Clear();
                
                MemoryPackSerializer.Serialize(_arrayBufferWriter, value, MemoryPackSerializerOptions.Utf16);
                
                var span = _arrayBufferWriter.WrittenSpan;
                return writer.WriteData(ref span, offset);
#endif
                Debug.LogWarning(EnableMemoryPackMessage);
                return default;
            }
        }
                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Serialize<TValue>(this ArrayBufferWriter<byte> writer,ref TValue value)
            where TValue : struct
        {
#if ENABLE_MEMORY_PACK   
            unsafe
            {
                var start = writer.WrittenCount;
                MemoryPackSerializer.Serialize(writer, value, MemoryPackSerializerOptions.Utf16);
                return writer.WrittenCount - start;
            }
#endif
            Debug.LogWarning(EnableMemoryPackMessage);
            return default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Deserialize(this ref NativeArray<byte> buffer,int offset,Type type)
        {
#if ENABLE_MEMORY_PACK  
            var span = buffer.AsReadOnlySpan();
            //if (UnsafeUtility.IsBlittable(type)) return span.ReadData(type);
            var slice = span.Slice(offset, span.Length - offset);
            return MemoryPackSerializer.Deserialize(type,slice, MemoryPackSerializerOptions.Utf16);
#endif
            Debug.LogWarning(EnableMemoryPackMessage);
            return default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Deserialize<TValue>(this ref NativeArray<byte> buffer,int offset, ref TValue value)
            where TValue : struct
        {
            var span = buffer.AsReadOnlySpan();
            var slice = span.Slice(offset, span.Length - offset);
            return Deserialize(ref slice, ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Deserialize<TValue>(this ref ReadOnlySpan<byte> buffer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (isBlittable) return buffer.ReadData(ref value);
                
#if ENABLE_MEMORY_PACK  
                var readCount = MemoryPackSerializer.Deserialize(buffer, ref value);
                return readCount;
#endif
                Debug.LogWarning(EnableMemoryPackMessage);
                return default;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [BurstCompile]
        public static int Deserialize<TValue>(this ref NativeSlice<byte> buffer,ref TValue value)
            where TValue : struct
        {
            unsafe
            {
                var isBlittable = UnsafeUtility.IsBlittable<TValue>();
                if (isBlittable) return buffer.ReadData(ref value);
                
#if ENABLE_MEMORY_PACK  
                var readCount = MemoryPackSerializer
                    .Deserialize(buffer.ToArray(), ref value);
                return readCount;
#endif
                Debug.LogWarning(EnableMemoryPackMessage);
                return default;
            }
        }
        
    }
}