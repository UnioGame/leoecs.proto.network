namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;

    [Serializable]
    public struct EcsNetworkConnectionData
    {
        public string Address;
        public ushort Port;
    }
}