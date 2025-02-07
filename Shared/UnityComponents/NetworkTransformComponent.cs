namespace Game.Ecs.Network.Shared.UnityComponents
{
    using System;
    
    using NetworkCommands.Data;
    using Unity.Mathematics;
    
#if ENABLE_MEMORY_PACK
    using MemoryPack;
#endif

    /// <summary>
    /// position of object
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
#if ENABLE_MEMORY_PACK
    [MemoryPackable]
#endif
    [Serializable]
    public partial struct NetworkTransformComponent : IEcsNetworkValue
    {
        public float3 Position;  
        public quaternion Rotation;
    }
}