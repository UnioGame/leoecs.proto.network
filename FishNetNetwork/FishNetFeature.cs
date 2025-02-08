namespace Game.Ecs.Network.UnityNetcode
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using NetcodeClients;
    using NetcodeMessages;
    using Shared.Components.Events;
    using Shared.Components.Requests;
    using Systems;
    using UniGame.LeoEcs.Shared.Extensions;

    [Serializable]
    public class FishNetFeature : EcsNetworkModuleFeature
    {
        public NetcodeClientsFeature clientsFeature = new();
        public NetcodeMessagingFeature messagingFeature = new();
        
        protected sealed override async UniTask OnInitializeAsync(IProtoSystems ecsSystems)
        {
            //additional feature for clients
            await clientsFeature.InitializeAsync(ecsSystems);
            //register rpc commands
            await messagingFeature.InitializeAsync(ecsSystems);
            
            
            //ecsSystems.DelHere<StartNetworkSelfRequest>();
        }
    }

}