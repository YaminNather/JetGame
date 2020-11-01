using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LoopsDatabase : DatabaseBase
{
    #region Variables
    private Dictionary<int, LoopMgrBase> m_Loops;
    public Dictionary<int, LoopMgrBase> Loops => m_Loops;
    #endregion

    public LoopMgrBase LoopGet_F(int index)
    {
        if (m_IsLoaded == false || m_Loops == null || m_Loops.ContainsKey(index) == false) return null;

        return m_Loops[index];
    }    

    public override IEnumerator LoadDatabase_F()
    {
        if(m_Loops == null)
        {
            //Debug.Log("<color=red><b>-----LOOPS LOADING BEGINS-----</b></color>");

            m_Loops = new Dictionary<int, LoopMgrBase>();

            //Finding the resource locations of all loop addressables.
            IList<IResourceLocation> resourceLocations = null;
            AsyncOperationHandle<IList<IResourceLocation>> asyncOpHandle_0 = Addressables.LoadResourceLocationsAsync("Loop");
            asyncOpHandle_0.Completed += x => resourceLocations = x.Result;
            yield return asyncOpHandle_0;

            //Instantiating all the loop addressables.
            foreach(IResourceLocation rl in resourceLocations)
            {
                AsyncOperationHandle<GameObject> asyncOpHandle_1 = Addressables.InstantiateAsync(rl);
                asyncOpHandle_1.Completed += x =>
                {
                    if (x.Result.TryGetComponent(out LoopMgrBase loop))
                    {
                        x.Result.SetActive(false);
                        m_Loops.Add(loop.Id, loop);
                        loop.transform.name = "Loop" + loop.Id;
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
