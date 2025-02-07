namespace Game.Ecs.Network.UnityNetcode.Components
{
    using System;
    using FishNet.Managing.Client;
    using Leopotam.EcsLite;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct FishNetClientManagerComponent : IEcsAutoReset<FishNetClientManagerComponent>
    {
        public ClientManager Value;
        
        public void AutoReset(ref FishNetClientManagerComponent c)
        {
            if (c.Value == null) return;
            c.Value.StopConnection();
        }
    }
}