namespace Game.Ecs.Network.Shared.Aspects
{
    using System;
    using Leopotam.EcsProto;
    using Modules.leoecs.proto.network.Shared.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;
    using UniGame.LeoEcs.Bootstrap.Runtime.Attributes;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    [ECSDI]
    public class NetworkServerAspect : EcsAspect
    {
        public ProtoPool<NetworkServerActiveComponent> Active;
    }
}