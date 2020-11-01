using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class LevelsDatabase : DatabaseBase
{
    #region Variables
    private LevelMgr[] m_EasyLevels;
    public LevelMgr[] EasyLevels => m_EasyLevels;
    private LevelMgr[] m_NormalLevels;
    public LevelMgr[] NormalLevels => m_NormalLevels;
    private LevelMgr[] m_HardLevels;
    public LevelMgr[] HardLevels => m_HardLevels;
    #endregion

    public override IEnumerator LoadDatabase_F()
    {
        //Debug.Log("<color=red><b>-----LEVELS LOADING BEGINS-----</b></color>");

        List<LevelMgr> levels = new List<LevelMgr>();
        yield return LevelsInstantiateAndSetup_F("Easy", levels);
        m_EasyLevels = levels.ToArray();

        yield return LevelsInstantiateAndSetup_F("Normal", levels);
        m_NormalLevels = levels.ToArray();

        yield return LevelsInstantiateAndSetup_F("Hard", levels);
        m_HardLevels = levels.ToArray();
        
        m_IsLoaded = true;
    }

    private IEnumerator LevelsInstantiateAndSetup_F(string type, List<LevelMgr> list)
    {
        //Debug.Log($"Instantiating {type} levels");
        list.Clear();

        type += " Level";
        IList<IResourceLocation> resourceLocations = new List<IResourceLocation>();
        AsyncOperationHandle<IList<IResourceLocation>> asyncOpHandle0 = Addressables.LoadResourceLocationsAsync(type);
        asyncOpHandle0.Completed += x => resourceLocations = x.Result;
        yield return asyncOpHandle0;
        
        //Debug.Log($"ResourceLocations.Length = {resourceLocations.Count}");

        foreach (IResourceLocation rl in resourceLocations)
        {
            AsyncOperationHandle<GameObject> asyncOpHandle_1 = Addressables.InstantiateAsync(rl);
            asyncOpHandle_1.Completed += x =>
            {
                if (x.Result.TryGetComponent(out LevelMgr level))
                {
                    x.Result.SetActive(false);
                    list.Add(level);
                    level.transform.name += " from Addressables";
                    level.Init_F();
                    DontDestroyOnLoad(x.Result);
                }
                //Debug.Log("<color=yellow>Level Instantiated- " + level.transform.name + "</color>");
            };
            yield return asyncOpHandle_1;
        }
    }

    public class CustomAsyncOp<T> : CustomYieldInstruction
    {
        protected bool m_IsComplete;
        public bool IsComplete => m_IsComplete;
        protected T m_Result;
        public T Result => m_Result;

        public override bool keepWaiting => m_IsComplete;
    }

    //public class ResourceLocationsLoad : CustomAsyncOp<IList<IResourceLocation>>
    //{
    //    Re
    //}
}
