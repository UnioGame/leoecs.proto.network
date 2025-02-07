namespace Game.Ecs.Network.UnityNetcode.Aspects
{
    using System;
    using Components;
    using Leopotam.EcsProto;
    using NetcodeClients.Components;
    using UniGame.LeoEcs.Bootstrap.Runtime.Abstract;

    /// <summary>
    /// ADD DESCRIPTION HERE
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class FishNetClientAspect : EcsAspect
    {
        public ProtoPool<NetcodeClientObjectComponent> NetcodeClientObject;
        public ProtoPool<FishNetClientManagerComponent> NetcodeClient;
    }
}