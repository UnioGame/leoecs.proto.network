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
    using Sirenix.OdinInspector;
    using Systems;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class FishNetFeature : EcsNetworkModuleFeature
    {
        [InlineProperty]
        [HideLabel]
        public FishNetSettings settings = new();
        
        public FishNetClientsFeature clientsFeature = new();
        
        protected sealed override async UniTask OnInitializeAsync(IProtoSystems ecsSystems)
        {
            var world = ecsSystems.GetWorld();

            world.SetGlobal(settings);
            ecsSystems.AddService(settings);
            
            ecsSystems.AddSystem(new EcsFishNetInitializeSystem(settings));
            
            //additional feature for clients
            await clientsFeature.InitializeAsync(ecsSystems);
            //ecsSystems.DelHere<StartNetworkSelfRequest>();
        }
    }

    [Serializable]   
    public class FishNetSettings
    {
        public AssetReferenceGameObject networkPrefab;
    }
}