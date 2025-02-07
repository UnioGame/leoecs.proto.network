namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Data
{
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
        public void SendMessageRPC(byte[] data,RpcParams rpcParams)
        {
            //TODO add defines check to send from client to client
            var result = MemoryPackSerializer.Deserialize<string>(data);
            Debug.Log($"SendFromServerRPC: {result}");
        }
        
        [TargetRpc(ExcludeServer = true)]
        public void SendToClientRPC(byte[] data,int size,RpcParams rpcParams)
        {
            var world = LeoEcsGlobalData.World;
            var entity = world.NewEntity();
            
            ref var rpcDataComponent = ref world.AddComponent<NetworkMessageDataComponent>(entity);
            rpcDataComponent.Value = data;
            rpcDataComponent.Size = size;
            rpcDataComponent.Sender = rpcParams.SenderId;
        }

    }
}