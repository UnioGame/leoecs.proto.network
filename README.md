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


# ECS Network Initialization


- Create Network Feature

```csharp
    [CreateAssetMenu(menuName = "ECS Proto/Features/Network/Netcode Feature",fileName = "Network Feature")]
    public class NetworkProtoFeature : BaseLeoEcsFeature
```


- Setup Network Settings

This asset will be create automatically it Odin Inspector installed
    
```csharp
[CreateAssetMenu(menuName = "ECS Proto/Features/Network/Network Settings", fileName = "Network Settings")]
public class EcsNetworkSettingsAsset : ScriptableObject
```

- Setup Fishnet Network Manager Prefab

1. Create Fishnet Network Manager Prefab
2. Add ProtoEcsMonoConverter mono behaviour to the prefab
3. Add FishNetConverter to ProtoEcsMonoConverter serializable converters


## Defines

```csharp
     #if ECS_NETWORK - activate ecs network feature
     #if ECS_NETWORK_SERVER - activate ecs network server feature
     #if ECS_NETWORK_CLIENT - activate ecs network client feature

     #if FISHNET - activate fishnet network transport
```

# Create Custom Network Providers


## Implement IEcsNetworkManager interface
