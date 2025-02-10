namespace Game.Ecs.Network.UnityNetcode.Data
{
    using FishNet.Managing;

    public class FishNetNetworkManager : IEcsNetworkManager
    {
        private readonly NetworkManager _networkManager;

        public FishNetNetworkManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
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
        
        public bool StartServer(uint port)
        {
            return _networkManager.ServerManager.StartConnection((ushort)port);
        }

        public bool StopServer(bool notifyClients)
        {
            return _networkManager.ServerManager.StopConnection(notifyClients);
        }
    }
}