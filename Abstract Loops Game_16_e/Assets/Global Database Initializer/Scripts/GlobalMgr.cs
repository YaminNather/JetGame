using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMgr : MonoBehaviour
{
    #region Variables
    static public GlobalMgr INSTANCE;
    static private bool s_DidResolutionScale;

    [HideInInspector] public GlobalData m_GlobalData;
    [HideInInspector] public LevelsDatabase m_LevelsDatabase;
    [HideInInspector] public JetsDatabase m_JetsDatabase;
    [HideInInspector] public LoopsDatabase m_LoopsDatabase;
    [HideInInspector] public ScenesLoader m_SceneLoader;
    [HideInInspector] public BackgroundMusicMgr m_BackgroundMusicMgr;
    [HideInInspector] public AdsMgr m_AdsMgr;
    [HideInInspector] public ColorMgr m_ColorMgr;

    public bool AllLoaded { get; private set; }
    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 60;
        
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;

        AddAllComponents_F();
        StartCoroutine(DatabasesLoad_F());
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (s_DidResolutionScale == false)
        {
            Screen.SetResolution((int)(Screen.currentResolution.width / 1.5f), (int)(Screen.currentResolution.height / 1.5f), true);
            s_DidResolutionScale = true;
        }
    }

    /// <summary>
    /// Creates and adds all the database objects and global data objects as components.
    /// </summary>
    private void AddAllComponents_F()
    {
        m_GlobalData = gameObject.AddComponent<GlobalData>();        
        m_LevelsDatabase = gameObject.AddComponent<LevelsDatabase>();
        m_JetsDatabase = gameObject.AddComponent<JetsDatabase>();
        m_LoopsDatabase = gameObject.AddComponent<LoopsDatabase>();
        m_SceneLoader = gameObject.AddComponent<ScenesLoader>();
        m_BackgroundMusicMgr = gameObject.AddComponent<BackgroundMusicMgr>();
        m_AdsMgr = gameObject.AddComponent<AdsMgr>();
        m_ColorMgr = gameObject.AddComponent<ColorMgr>();
    }       

    /// <summary>
    /// Load things into the database.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DatabasesLoad_F()
    {
        yield return StartCoroutine(m_LevelsDatabase.LoadDatabase_F());
        yield return StartCoroutine(m_JetsDatabase.LoadDatabase_F());
        yield return StartCoroutine(m_LoopsDatabase.LoadDatabase_F());

        AllLoaded = true;

    }
}

public abstract class DatabaseBase : MonoBehaviour
{
    protected bool m_IsLoaded;
    public bool IsLoaded => m_IsLoaded;

    public abstract IEnumerator LoadDatabase_F();
}
