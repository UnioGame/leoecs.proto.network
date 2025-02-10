namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;
    using Cysharp.Threading.Tasks;
    using Leopotam.EcsProto;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Core.Runtime;
    using UnityEngine.AddressableAssets;
    using Object = UnityEngine.Object;

    [Serializable]
    public class EcsNetworkObjectInit : INetworkInitSettings
    {
        public AssetReferenceGameObject asset;
        public bool immortal = true;
        
        public async UniTask InitializeAsync(IProtoSystems systems, ILifeTime lifeTime)
        {
            var agentSource = asset;
            var agent = await agentSource
                .LoadAssetInstanceTaskAsync(lifeTime, true);
            if(immortal)
                Object.DontDestroyOnLoad(agent);
        }
    }
}