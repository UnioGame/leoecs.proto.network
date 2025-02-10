namespace Game.Ecs.Network.UnityNetcode.Data
{
    public class EcsNetworkMessages
    {
        public const string ServerAlreadyStarted = "server already started";
        public const string FailedToStartServer = "Failed to start host for address: {0} | port: {1}";

        public const string SuccessStartedServer = "Successfully started {0} for address: {1} | port: {2}";
    }
}