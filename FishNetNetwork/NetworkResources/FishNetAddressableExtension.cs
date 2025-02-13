namespace Game.FishNetModule.NetworkResources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using FishNet;
    using FishNet.Managing;
    using FishNet.Managing.Object;
    using FishNet.Object;
    using GameKit.Dependencies.Utilities;
    using UniGame.AddressableTools.Runtime;
    using UniGame.Core.Runtime;
    using UniGame.Runtime.ObjectPool;
    using UniGame.Runtime.ObjectPool.Extensions;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;

    public static class FishNetAddressableExtension
    {
        private static Dictionary<int,FishNetAddressableHandle> _networkHandles = new(32);
        
        /// <summary>
        /// reset static data on domain reload
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            foreach (var networkPrefab in _networkHandles)
                networkPrefab.Value.Dispose();
            _networkHandles.Clear();
        }
        
        /// <summary>
        /// Reference to your NetworkManager.
        /// </summary>
        public static NetworkManager NetworkManager => InstanceFinder.NetworkManager;

        /// <summary>
        /// Loads an addressables package by string.
        /// You can load whichever way you prefer, this is merely an example.
        /// </summary>
        public static async UniTask<IEnumerable<NetworkObject>> LoadNetworkObjectsAsync(this string addressableResource)
        {
            var handle = GetFishNetAddressableHandle(addressableResource);
            
            /* Load addressables normally. If the object is a NetworkObject prefab
             * then add it to our cache! */
            var netObjects = await addressableResource
                .LoadAssetsTaskAsync<GameObject>(handle.lifeTime);

            foreach (var netObject in netObjects)
            {
                RegisterNetworkPrefab(addressableResource, netObject);
            }

            return handle.objects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FishNetAddressableHandle GetFishNetAddressableHandle(string addressableResource)
        {
            var id = addressableResource.GetAddressableFishNetId();
            
            //Check if we have already loaded this addressable package.
            if (_networkHandles.TryGetValue(id, out var handle)) return handle;
            
            handle = ClassPool.Spawn<FishNetAddressableHandle>();
            handle.addressableResource = addressableResource;
            handle.id = id;

            return handle;
        }
        
        /// <summary>
        /// Loads an addressables package by string.
        /// You can load whichever way you prefer, this is merely an example.
        /// </summary>
        public static async UniTask<NetworkObject> LoadNetworkObjectAsync(this string addressableResource)
        {
            var handle = GetFishNetAddressableHandle(addressableResource);
            
            /* Load addressables normally. If the object is a NetworkObject prefab
             * then add it to our cache! */
            var networkPrefab = await addressableResource
                .LoadAssetTaskAsync<GameObject>(handle.lifeTime);

            handle = RegisterNetworkPrefab(addressableResource, networkPrefab);

            return handle.objects.FirstOrDefault();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetAddressableFishNetId(this string addressableResource)
        {
            return addressableResource.GetStableHashU16();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FishNetAddressableHandle RegisterNetworkPrefab(string id, GameObject networkObject)
        {
            var nob = networkObject.GetComponent<NetworkObject>();
            if (nob == null) return null;
            networkObject.SetActive(true);
            
            return RegisterNetworkPrefab(id, nob);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FishNetAddressableHandle RegisterNetworkPrefab(string id, NetworkObject networkObject)
        {
            if (string.IsNullOrEmpty(id)) return FishNetAddressableHandle.Empty;
            return RegisterNetworkPrefab(id.GetAddressableFishNetId(), networkObject);
        }

        public static FishNetAddressableHandle RegisterNetworkPrefab(ushort id, NetworkObject networkObject)
        {
            if (!_networkHandles.TryGetValue(id, out var handle))
                return FishNetAddressableHandle.Empty;

            if(!handle.objects.Add(networkObject)) 
                return handle;
            
            /* Get a cache to store networkObject references in from our helper object pool.
             * This is not required, you can make a new list if you like. But if you
             * prefer to prevent allocations FishNet has the really helpful CollectionCaches
             * and ObjectCaches, as well Resettable versions of each. */
            var cache = CollectionCaches<NetworkObject>.RetrieveList();
            cache.Add(networkObject);
            
            /* GetPrefabObjects will return the prefab
             * collection to use for Id. Passing in true
             * will create the collection if needed. */
            var spawnablePrefabs = NetworkManager
                .GetPrefabObjects<SinglePrefabObjects>(id, true) as SinglePrefabObjects;
            
            /* Add the cached references to spawnablePrefabs. You could skip
             * caching entirely and just add them as they are read in our LoadAssetsAsync loop
             * but this saves more performance by adding them all at once. */
            spawnablePrefabs.AddObjects(cache);

            //Optionally(obviously, do it) store the collection cache for use later. We really don't like garbage!
            CollectionCaches<NetworkObject>.Store(cache);

            return handle;
        }

        

        /// <summary>
        /// Loads an addressables package by string.
        /// </summary>
        public static void UnloadFishNetAddressableObject(string addressableResource)
        {
            //Get the Id the same was as we did for loading.
            var id = addressableResource.GetAddressableFishNetId();

            /* Once again get the prefab collection for the Id and
             * clear it so that there are no references of the objects
             * in memory. */
            var spawnablePrefabs =
                (SinglePrefabObjects)NetworkManager.GetPrefabObjects<SinglePrefabObjects>(id, true);
            spawnablePrefabs.Clear();
            
            if (_networkHandles.TryGetValue(id, out var handle))
            {
                handle.Dispose();
                handle.Despawn();
            }

            _networkHandles.Remove(id);
        }
     
    }
    
       
    [Serializable]
    public class FishNetAddressableHandle : IDisposable
    {
        public static readonly FishNetAddressableHandle Empty = new FishNetAddressableHandle();
        
        public ushort id;
        public HashSet<NetworkObject> objects = new();
        public string addressableResource = string.Empty;
        public LifeTime lifeTime = new();
        
        public void Dispose()
        {
            id = 0;
            lifeTime.Restart();
            addressableResource = string.Empty;
            objects.Clear();
        }
    }
}