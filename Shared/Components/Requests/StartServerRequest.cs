namespace Game.Ecs.Network.Shared.Components.Requests
{
    using System;
    using UnityEngine.Serialization;

    /// <summary>
    /// connect to network as host
    /// </summary>
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public struct StartServerRequest
    {
        public string Address;
        public ushort Port;
        /// <summary>
        /// if true, server will be created as a host and allow make server client on his side
        /// overwise server will be only in server mode
        /// </summary>
        public bool AllowHostMode;
    }
}