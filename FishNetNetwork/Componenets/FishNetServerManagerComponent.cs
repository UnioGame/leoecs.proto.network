namespace Game.Ecs.Network.UnityNetcode.Components
{
    using System;
    using FishNet.Managing.Server;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct FishNetServerManagerComponent
    {
        public ServerManager Value;
    }
}