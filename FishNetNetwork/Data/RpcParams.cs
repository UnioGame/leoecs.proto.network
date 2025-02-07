namespace Game.Ecs.Network.UnityNetcode.Data
{
    using NetworkCommands.Data;

    public struct RpcParams
    {
        public NetworkMessageTarget Target; 
        public ulong SenderId;
    }
}