namespace Game.FishNetModule.NetworkResources
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using FishNet.Managing.Scened;
    using UnityEngine.SceneManagement;

    public class FishNetAddressableSceneProcessor : SceneProcessorBase 
    {
        
        public bool IsDone { get; private set; }
        
        public override void BeginLoadAsync(string sceneName, LoadSceneParameters parameters)
        {
            IsDone = false;
        }

        public override void BeginUnloadAsync(Scene scene)
        {
            
        }

        public override bool IsPercentComplete()
        {
            throw new System.NotImplementedException();
        }

        public override float GetPercentComplete()
        {
            throw new System.NotImplementedException();
        }

        public override List<Scene> GetLoadedScenes()
        {
            throw new System.NotImplementedException();
        }

        public override void ActivateLoadedScenes()
        {
            
        }

        public override IEnumerator AsyncsIsDone()
        {
            while (IsDone == false)
                yield return null;
        }
    }
}