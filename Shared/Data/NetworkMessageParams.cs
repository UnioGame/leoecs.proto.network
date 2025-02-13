namespace Game.Ecs.Network.UnityNetcode.Data
{
    using System;
    using NetworkCommands.Data;

    [Serializable]
    public struct NetworkMessageParams
    {
        public NetworkMessageTarget Target; 
        public NetworkChannel Channel;
        public int SenderId;
        public int[] Targets;
    }
}