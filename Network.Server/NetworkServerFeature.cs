namespace Game.Modules.leoecs.proto.network.Network.Server
{
    using System;
    using Cysharp.Threading.Tasks;
    using Ecs.Network.Shared.Components.Events;
    using Ecs.Network.UnityNetcode.Systems;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using UniGame.LeoEcs.Bootstrap.Runtime;
    
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    [Serializable]
    public class NetworkServerFeature : EcsFeature
    {
        protected override UniTask OnInitializeAsync(IProtoSystems ecsSystems)
        {
            //remove server connected event
            ecsSystems.DelHere<NetworkServerConnectedSelfEvent>();
            
            //start netcode server and fire server connected if success
            ecsSystems.AddSystem(new StartNetcodeServerSystem());
            //stop netcode server
            ecsSystems.AddSystem(new StopNetcodeServerSystem());
            
            return UniTask.CompletedTask;
        }
    }
}