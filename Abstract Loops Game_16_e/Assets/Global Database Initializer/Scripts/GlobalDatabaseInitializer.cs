using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDatabaseInitializer : MonoBehaviour
{
    #region Variables
    static public GlobalDatabaseInitializer s_Instance;

    [HideInInspector] public GlobalData globalData;
    [HideInInspector] public LevelsDatabase levelsDatabase;
    [HideInInspector] public JetsDatabase jetsDatabase;
    [HideInInspector] public LoopsDatabase loopsDatabase;
    [HideInInspector] public ScenesLoader scenesDatabase;

    public bool AllLoaded { get; private set; }
    #endregion

    private void Awake()
    {
        if (s_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_Instance = this;

        AddAllComponents_F();
        StartCoroutine(DatabasesLoad_F());
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Creates and adds all the database objects and global data objects as components.
    /// </summary>
    private void AddAllComponents_F()
    {
        globalData = gameObject.AddComponent<GlobalData>();        
        levelsDatabase = gameObject.AddComponent<LevelsDatabase>();
        jetsDatabase = gameObject.AddComponent<JetsDatabase>();
        loopsDatabase = gameObject.AddComponent<LoopsDatabase>();
        scenesDatabase = gameObject.AddComponent<ScenesLoader>();
    }       

    /// <summary>
    /// Load things into the database.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DatabasesLoad_F()
    {
        yield return StartCoroutine(levelsDatabase.LoadDatabase_F());
        yield return StartCoroutine(jetsDatabase.LoadDatabase_F());
        yield return StartCoroutine(loopsDatabase.LoadDatabase_F());

        AllLoaded = true;
    }
}

public abstract class DatabaseBase : MonoBehaviour
{
    protected bool m_IsLoaded;
    public bool IsLoaded { get => m_IsLoaded; }    

    public abstract IEnumerator LoadDatabase_F();
}
