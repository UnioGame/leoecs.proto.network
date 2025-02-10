namespace Game.Modules.leoecs.proto.network.Network.Tests
{
    using System.Text;
    using Ecs.Network.Shared.Components.Requests;
    using FishNet.Managing;
    using FishNet.Object;
    using Leopotam.EcsProto;
    using Sirenix.OdinInspector;
    using TMPro;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine.UI;

    public class NetworkDemoView : NetworkBehaviour
    {
        public string addressValue = "localhost";
        public ushort portValue = 10425;
        
        public Button server;
        public Button client;
        public Button room;
        public Toggle enableHost;
        public TMP_InputField address;
        public TMP_InputField port;
        public TextMeshProUGUI info;

        public string networkMessage = "Hello World";
        
        private NetworkManager manager;
        private StringBuilder _infoBuilder = new StringBuilder(512);
        private bool _isInitialized;
        private ProtoWorld _world;
        
        private void Start()
        {
            port.text = portValue.ToString();
            address.text = addressValue;
            
            server.onClick.AddListener(StartServer);
            client.onClick.AddListener(StartClient);
            address.onValueChanged.AddListener(value => addressValue = value);
            port.onValueChanged.AddListener(value =>
            {
                if(!int.TryParse(port.text,out var portData)) return;
                portValue = (ushort) portData;
            });
        }
        
        private void OnDestroy()
        {
            server.onClick.RemoveAllListeners();
            client.onClick.RemoveAllListeners();
            address.onValueChanged.RemoveAllListeners();
            port.onValueChanged.RemoveAllListeners();
        }
        
        public void StartServer()
        {
            if(_world == null) return;
            var requestEntity = _world.NewEntity();
            ref var startServer = ref _world.AddComponent<StartServerRequest>(requestEntity);
            startServer.Address = addressValue;
            startServer.Port = portValue;
            startServer.AllowHostMode = enableHost.isOn;
        }
        
        public void StartClient()
        {
            manager.ClientManager.StartConnection(address.text,portValue);
        }
        
        public void CreateRoom()
        {
            
        }

        [Button]
        public void SendMessageData()
        {
            
        }

        private void Initialize()
        {
            if(_isInitialized) return;
            _isInitialized = true;
            
        }
        
        private void Update()
        {
            if (_world == null)
            {
                _world = LeoEcsGlobalData.World;
                if (_world == null) return;
            }
            
            if (manager == null)
            {
                manager = FindAnyObjectByType<NetworkManager>();
                if(manager == null) return;
            }
            
            Initialize();
            
            server.interactable = !manager.IsServerStarted;
            client.interactable = !manager.IsClientStarted;
            room.interactable = manager.IsServerStarted;
            
            _infoBuilder.Clear();
            
            _infoBuilder.AppendLine($"Server: {manager.IsServerStarted}");
            _infoBuilder.AppendLine($"Client: {manager.IsClientStarted}");
            _infoBuilder.AppendLine($"Host: {manager.IsHostStarted}");
            
            _infoBuilder.AppendLine($"Clients Count: {manager.ClientManager.Clients.Count}");
            
            var clients = manager.ClientManager.Clients;
            foreach (var clientItem in clients)
            {
                var connection = clientItem.Value;
                var id = connection.ClientId;
                _infoBuilder.AppendLine($"Client: {id} IsHost {connection.IsHost} | Tick {connection.PacketTick}");
            }
            
            info.text = _infoBuilder.ToString();
        }
        
    }
}
