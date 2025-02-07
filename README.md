ECS World Network transport for transparent client-server communication

# Installation

Memory Pack - Serialization

Zero encoding extreme performance binary serializer for C# and Unity.

More info and how to install: https://github.com/Cysharp/MemoryPack


# Data Serialization

Non Blittable Types

```
For Enable MemoryPack serialization support add *ENABLE_MEMORY_PACK* constraint to your project settings.
```

Without MemoryPack serialization support, ECS network allow serialize only Blittable types

All non Blittable types should be marked with [MemoryPackable]

```csharp
    [MemoryPackable]
    public partial struct NetworkComponentHeader
    {
        public int TypeId;
        public int Size;
    }
```
