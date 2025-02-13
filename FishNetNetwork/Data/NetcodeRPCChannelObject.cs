namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Data
{
    using FishNet.Connection;
    using FishNet.Object;
    using MemoryPack;
    using NetworkCommands.Components;
    using UniGame.LeoEcs.Converter.Runtime;
    using UniGame.LeoEcs.Shared.Extensions;
    using UnityEngine;
    using UnityNetcode.Data;

    public class NetcodeRPCChannelObject : NetworkBehaviour
    {
        
        [TargetRpc(ExcludeServer = false)]
        public void SendMessageRPC(NetworkConnection connection,byte[] data,NetworkMessageParams rpcParams)
        {
            //TODO add defines check to send from client to client
            var result = MemoryPackSerializer.Deserialize<string>(data);
            Debug.Log($"SendFromServerRPC: {result} from {connection.ClientId}");
        }
        
        [TargetRpc(ExcludeServer = true)]
        public void SendToClientRPC(NetworkConnection connection,byte[] data,int size,NetworkMessageParams rpcParams)
        {
            var world = LeoEcsGlobalData.World;
            var entity = world.NewEntity();
            
            ref var rpcDataComponent = ref world.AddComponent<NetworkMessageDataComponent>(entity);
            rpcDataComponent.Value = data;
            rpcDataComponent.Size = size;
            rpcDataComponent.Sender = connection.ClientId;
        }

    }
}