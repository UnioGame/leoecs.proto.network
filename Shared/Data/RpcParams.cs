namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;
    using NetworkCommands.Data;

    [Serializable]
    public struct RpcParams
    {
        public NetworkMessageTarget Target; 
        public ulong SenderId;
    }
}