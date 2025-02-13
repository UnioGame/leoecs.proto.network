namespace Game.Ecs.Network.UnityNetcode.NetcodeMessages.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using FishNet.Object;
    using FishNet.Transporting;
    using NetworkCommands.Data;
    using UnityNetcode.Data;

    public static class NetworkObjectExtensions
    {
        public static Channel GetChannel(this NetworkChannel networkChannel)
        {
            switch (networkChannel)
            {
                case NetworkChannel.Reliable:
                    return Channel.Reliable;
                case NetworkChannel.Unreliable:
                    return Channel.Unreliable;
                default:
                    return Channel.Reliable;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NetworkMessageParams GetRpcTarget(this NetworkBehaviour target,
            NetworkMessageTarget messageTarget, int id = default)
        {
            var rpcTarget = new NetworkMessageParams
            {
                Target = messageTarget,
                SenderId = id,
            };

            // var targetResult = messageTarget switch
            // {
            //     NetworkMessageTarget.Server => rpcTarget.Server,
            //     NetworkMessageTarget.NotServer => rpcTarget.NotServer,
            //     NetworkMessageTarget.All => rpcTarget.Everyone,
            //     NetworkMessageTarget.Me => rpcTarget.Me,
            //     NetworkMessageTarget.NotMe => rpcTarget.NotMe,
            //     NetworkMessageTarget.Specified => rpcTarget.Single(id, RpcTargetUse.Temp),
            //     _ => rpcTarget.Server,
            // };
            
            return rpcTarget;
        }
    }
}