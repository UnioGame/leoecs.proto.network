namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class NetworkAssetsSettings
    {
        [SerializeReference]
        public List<INetworkInitSettings> networkSettings = new();
    }
}