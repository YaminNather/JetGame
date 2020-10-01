using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LevelsDatabase : DatabaseBase
{
    #region Variables
    private List<LevelMgr> m_Levels;
    public List<LevelMgr> Levels { get => m_Levels; }    
    #endregion

    public LevelMgr LevelGet_F(int index)
    {
        if (m_IsLoaded == false || m_Levels == null || index >= Levels.Count) return null;        
        
        return Levels[index];
    }

    public override IEnumerator LoadDatabase_F()
    {
        if (Levels == null || Levels.Count == 0)
        {
            //Debug.Log("<color=red><b>-----LEVELS LOADING BEGINS-----</b></color>");

            m_Levels = new List<LevelMgr>();

            //Finding the resource locations of all loop addressables.
            IList<IResourceLocation> resourceLocations = null;
            AsyncOperationHandle<IList<IResourceLocation>> asyncOpHandle_0 = Addressables.LoadResourceLocationsAsync("Level");
            asyncOpHandle_0.Completed += x => resourceLocations = x.Result;
            yield return asyncOpHandle_0;

            //Loading all the loop addressables.
            yield return Addressables.LoadAssetsAsync<GameObject>(resourceLocations, gObj =>
            {
                //if (gObj.TryGetComponent(out LevelMgr level)) Debug.Log("<color=green>Level Loaded- " + level.transform.name + "</color>");
            });

            //Instantiating all the loop addressables.
            foreach (IResourceLocation rl in resourceLocations)
            {
                AsyncOperationHandle<GameObject> asyncOpHandle_1 = Addressables.InstantiateAsync(rl);
                asyncOpHandle_1.Completed += x =>
                {
                    if (x.Result.TryGetComponent(out LevelMgr level))
                    {
                        x.Result.SetActive(false);
                        m_Levels.Add(level);
                        level.transform.name = level.transform.name + " from Addressables";
                        level.Init_F();
                        DontDestroyOnLoad(x.Result);
                    }
                    //Debug.Log("<color=yellow>Level Instantiated- " + level.transform.name + "</color>");
                };
                yield return asyncOpHandle_1;
            }

            m_IsLoaded = true;
        }      

    }
}
