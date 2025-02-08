namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class NetworkAssetsSettings
    {
        [Header("Network Prefabs")]
        public List<EcsNetworkAsset> networkPrefabs = new();
    }

    [Serializable]
    public class EcsNetworkAsset
    {
        public AssetReferenceGameObject asset;
        public bool immortal = true;
    }
}