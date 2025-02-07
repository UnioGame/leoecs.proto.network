namespace Game.Ecs.Network.NetworkCommands.Data
{
#if ENABLE_MEMORY_PACK
    using MemoryPack;
#endif
    
#if ENABLE_MEMORY_PACK
    [MemoryPackable]
#endif
    public partial struct NetworkComponentHeader
    {
        public int TypeId;
        public int Size;
    }
}