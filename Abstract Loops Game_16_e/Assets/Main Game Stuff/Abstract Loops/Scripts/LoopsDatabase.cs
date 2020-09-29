using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LoopsDatabase : DatabaseBase
{
    private List<LoopMgrBase> m_Loops;
    public List<LoopMgrBase> Loops { get => m_Loops; }

    public LoopMgrBase LoopGet_F(int index)
    {
        if (m_IsLoaded == false || m_Loops == null || index >= m_Loops.Count) return null;

        return m_Loops[index];
    }    

    public override IEnumerator LoadDatabase_F()
    {
        if(m_Loops == null)
        {
            //Debug.Log("<color=red><b>-----LOOPS LOADING BEGINS-----</b></color>");

            m_Loops = new List<LoopMgrBase>();

            //Finding the resource locations of all loop addressables.
            IList<IResourceLocation> resourceLocations = null;                
            AsyncOperationHandle<IList<IResourceLocation>> asyncOpHandle_0 = Addressables.LoadResourceLocationsAsync("Loop");
            asyncOpHandle_0.Completed += x => resourceLocations = x.Result;
            yield return asyncOpHandle_0;

            //Loading all the loop addressables.
            yield return Addressables.LoadAssetsAsync<GameObject>(resourceLocations, gObj =>
            {
                //if (gObj.TryGetComponent(out LoopMgrBase loop)) Debug.Log("<color=green>Loop Loaded- " + loop.transform.name + "</color>");

            });

            //Instantiating all the loop addressables.
            foreach(IResourceLocation rl in resourceLocations)
            {
                AsyncOperationHandle<GameObject> asyncOpHandle_1 = Addressables.InstantiateAsync(rl);
                asyncOpHandle_1.Completed += x =>
                {
                    if (x.Result.TryGetComponent(out LoopMgrBase loop))
                    {
                        x.Result.SetActive(false);
                        m_Loops.Add(loop);
                        loop.transform.name = loop.transform.name + " from Addressables";
                        DontDestroyOnLoad(x.Result);
                    }
                    //Debug.Log("<color=yellow>Loop Instantiated- " + loop.transform.name + "</color>");
                };
                yield return asyncOpHandle_1;
            }

            m_IsLoaded = true;            
        }
    }    
}
