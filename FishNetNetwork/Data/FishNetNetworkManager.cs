namespace Game.Ecs.Network.UnityNetcode.Data
{
    using FishNet.Connection;
    using FishNet.Managing;
    using FishNet.Transporting;
    using Leopotam.EcsProto;
    using NetworkCommands.Components;
    using UniGame.LeoEcs.Shared.Extensions;

    public class FishNetNetworkManager : IEcsNetworkManager
    {
        private readonly NetworkManager _networkManager;
        private readonly ProtoWorld _world;

        public FishNetNetworkManager(NetworkManager networkManager,ProtoWorld world)
        {
            _networkManager = networkManager;
            _world = world;
            
            _networkManager.ServerManager.RegisterBroadcast<BroadcastMessage>(OnServerReceiveMessage);
            _networkManager.ClientManager.RegisterBroadcast<BroadcastMessage>(OnClientReceiveMessage);
        }

        public bool IsInitialized => _networkManager.Initialized;
        public bool IsServerStarted => _networkManager.IsServerStarted;
        public bool IsHostStarted => _networkManager.IsHostStarted;
        public bool IsClientStarted => _networkManager.IsClientStarted;

        public int ActiveConnectionId => _networkManager.ClientManager.Connection.ClientId;
        public int ActiveClientId => _networkManager.ClientManager.Connection.ClientId;
        public float ServerTime => _networkManager.TimeManager.ServerUptime;
        
        public uint ServerTick => _networkManager.TimeManager.Tick;
        
        public int TickRate => _networkManager.TimeManager.TickRate;
        
        
        public void Dispose()
        {
            _networkManager.ServerManager.UnregisterBroadcast<BroadcastMessage>(OnServerReceiveMessage);
            _networkManager.ClientManager.UnregisterBroadcast<BroadcastMessage>(OnClientReceiveMessage);
        }
        
        public bool StartServer(uint port)
        {
            return _networkManager.ServerManager.StartConnection((ushort)port);
        }

        public bool StopServer(bool notifyClients)
        {
            return _networkManager.ServerManager.StopConnection(notifyClients);
        }

        public void SendToClientsMessage(byte[] data, int size, NetworkMessageParams messageParams)
        {
            //var channel = messageParams.Channel.GetChannel();
            //_networkManager.ServerManager.Broadcast(data, channel); 
        }

        public void SendMessage(byte[] data, int size, NetworkMessageParams rpcParams)
        {
            // Отправляем через Broadcast (Channel.Reliable для гарантированной доставки)
            //_networkManager.ServerManager.Broadcast(data, Channel.Reliable); 
        }


        private void OnServerReceiveMessage(NetworkConnection connection,BroadcastMessage data,Channel channel)
        {
            var entity = _world.NewEntity();
            
            ref var rpcDataComponent = ref _world.AddComponent<NetworkMessageDataComponent>(entity);
            rpcDataComponent.Value = data.Data;
            rpcDataComponent.Size = data.Size;
            rpcDataComponent.Sender = data.SenderId;
        }
        
        private void OnClientReceiveMessage(BroadcastMessage data,Channel channel)
        {
            var entity = _world.NewEntity();
            
            ref var rpcDataComponent = ref _world.AddComponent<NetworkMessageDataComponent>(entity);
            rpcDataComponent.Value = data.Data;
            rpcDataComponent.Size = data.Size;
            rpcDataComponent.Sender = data.SenderId;
        }

    }
}