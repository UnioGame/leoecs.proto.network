namespace Game.Ecs.Network.NetworkCommands.Data
{
#if ENABLE_MEMORY_PACK
    using MemoryPack;
#endif
    
#if ENABLE_MEMORY_PACK
    [MemoryPackable]
#endif
    public partial struct NetworkEntityHeader
    {
        public int Id;
        public bool IsValueChanged;
        public int Count;
    }
}