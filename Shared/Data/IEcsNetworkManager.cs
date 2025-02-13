namespace Game.Ecs.Network.UnityNetcode
{
    using System;
    using Data;

    public interface IEcsNetworkManager : INetworkMessageChannel, IDisposable
    {
        public bool IsInitialized { get; }
        public bool IsServerStarted { get; }
        public bool IsHostStarted { get; }
        public bool IsClientStarted { get; }
        
        public int ActiveConnectionId { get; }
        public int ActiveClientId { get; }
        public float ServerTime { get; }
        public uint ServerTick { get; }
        
        public int TickRate { get; }
        
        public bool StartServer(uint port);
        public bool StopServer(bool notifyClients);
    }


    public interface INetworkMessageChannel
    {
        void SendToClientsMessage(byte[] data,int size, NetworkMessageParams rpcParams);
        void SendMessage(byte[] data,int size, NetworkMessageParams rpcParams);
    }
}