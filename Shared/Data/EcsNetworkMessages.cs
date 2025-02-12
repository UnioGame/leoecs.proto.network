namespace Game.Ecs.Network.UnityNetcode.Data
{
    public class EcsNetworkMessages
    {
        public const string ServerAlreadyStarted = "server already started";
        public const string FailedToStartServer = "failed to start host for address: {0} | port: {1}";

        public const string SuccessStartedServer = "successfully started {0} for address: {1} | port: {2}";
        
        //client
        public const string ClientAlreadyStarted = "network client already started";
    }
}