namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;
    using FishNet.Broadcast;

    [Serializable]
    public struct BroadcastMessage : IBroadcast
    {
        public byte[] Data;
        public int Size;
        public int SenderId;
    }
}