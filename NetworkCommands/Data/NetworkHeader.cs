namespace Game.Ecs.Network.NetworkCommands.Data
{
#if ENABLE_MEMORY_PACK
    using MemoryPack;
#endif
    
#if ENABLE_MEMORY_PACK
    [MemoryPackable]
#endif
    public partial struct NetworkHeader
    {
        public int Tick;
        public float Time;
        public int Count;
        public int Size;
        public int Components;
        public bool IsHashed;
    }
}