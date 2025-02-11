namespace Game.Ecs.Network.UnityNetcode.NetcodeClients
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using Shared.Components.Events;
    using Systems;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;

    [Serializable]
    public class FishNetClientsFeature : EcsFeature
    {
        protected sealed override UniTask OnInitializeAsync(IProtoSystems ecsSystems)
        {
            ecsSystems.DelHere<NetworkClientConnectedSelfEvent>();
            ecsSystems.DelHere<NetworkClientDisconnectedEvent>();
            
            //start client by request and connect to server
            ecsSystems.Add(new StartNetcodeClientSystem());
            
            //update client info
            ecsSystems.Add(new UpdateNetcodeClientsSystem());
            
            //handle new client connect and ask to resend all data
            ecsSystems.Add(new HandleNewClientConnectSystem());
            
            return UniTask.CompletedTask;
        }
    }

}