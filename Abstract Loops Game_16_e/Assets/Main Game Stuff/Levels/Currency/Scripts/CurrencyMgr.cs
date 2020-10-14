using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencyMgr : LevelComponent
{
    public override void Reset_F()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            if (MainGameReferences.INSTANCE != null)
            {
                MainGameReferences.INSTANCE.scoreMgr.CurrencyAdd_F(1);
                MainGameReferences.INSTANCE.currencyPSMgr.DoBurst_F(transform.position);
            }
            gameObject.SetActive(false);
        }
    }
}

#if UNITY_EDITOR
public class CurrencyCreateWizard : ScriptableWizard
{
    public int m_SpawnCount = 1;

    public Transform m_ContextTrans;
    public float m_InitialDist = 10f;
    public float m_RelativeDist = 1f;

    public float m_DistFromCenter = 2f;
    public float m_Angle = 0f;
    
    private GameObject m_CurrencyPrefab;
    [SerializeField] private LinkedList<GameObject> m_Ghosts;
    
    [MenuItem("Spawn Level Prefab Tool/Add Currency")]
    private static void CreateWizard(MenuCommand menuCommand)
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null) return;
            DisplayWizard<CurrencyCreateWizard>("Create Currency", "Spawn");
    }

    private void OnEnable()
    {
        //Initializing.
        m_Ghosts = new LinkedList<GameObject>();
        
        //Loading in the Currency Prefab.
        m_CurrencyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Main Game Stuff/Levels/Currency/Prefabs/Currency_Prefab.prefab");

        m_ContextTrans = Selection.activeTransform;

        //Creating the UI, and a ghost to start with.
        UICreate_F();
    }

    private void UICreate_F()
    {
        rootVisualElement.Add(new Label("INPUT VALUES")
        { 
            style = { unityFontStyleAndWeight = FontStyle.Bold, unityTextAlign = TextAnchor.MiddleCenter }
        });
        rootVisualElement.Add(new IMGUIContainer(() => DrawWizardGUI()));
        rootVisualElement.Add(new Button(OnWizardCreate) { text = "Spawn"});

        rootVisualElement.Add(new Button(() => Selection.activeTransform = m_Ghosts.First.Value.transform) { text = "Get first ghost"});

        rootVisualElement.schedule.Execute(OnWizardUpdate).Every(1);
    }

    private void GhostCreate_F()
    {
        //Debug.Log("<color=cyan>Ghosts being created</color>");

        //Destroy existing ghosts.
        GhostsDestroy_F();     
        
        //Spawning new ghosts.
        for(int i = 0; i < m_SpawnCount; i++)
        {
            GameObject gObj = PrefabUtility.InstantiatePrefab(m_CurrencyPrefab, PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.transform) as GameObject;
            gObj.transform.name = $"Ghost {i}";
            m_Ghosts.AddLast(gObj);
        }
    }

    private void OnGUI() { }

    private void OnWizardUpdate()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null) Close();

        if (m_Ghosts.Count != m_SpawnCount)
            GhostCreate_F();
        
        GhostsTransRefresh_F();
    }

    private void GhostsTransRefresh_F()
    {
        if (m_Ghosts.Count == 0) return;

        //Calculates first ghost's position.
        LinkedListNode<GameObject> node = m_Ghosts.First;        
        Vector3 pos = new Vector3(0f, 0f, (m_ContextTrans != null) ? m_ContextTrans.position.z : 0f) + Vector3.forward * m_InitialDist;
        pos += Quaternion.Euler(0f, 0f, m_Angle) * (Vector3.right * m_DistFromCenter);
        node.Value.transform.position = pos;        
        node = node.Next;

        //Calc the rest of the ghosts position.
        for(; node != null; node = node.Next) node.Value.transform.position = node.Previous.Value.transform.position + Vector3.forward * m_RelativeDist;            
    }

    private void OnWizardCreate()
    {
        if (m_Ghosts.Count == 0) return;
        
        ActualGObjsCreate_F();
        GhostsDestroy_F();
        Close();
    }

    private void ActualGObjsCreate_F()
    {
        GameObject gObj = null;
        foreach (GameObject ghost in m_Ghosts)
        {
            gObj = PrefabUtility.InstantiatePrefab(m_CurrencyPrefab, PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.transform) as GameObject;
            gObj.transform.position = ghost.transform.position;
            Undo.RegisterCreatedObjectUndo(gObj, "Created Currency with custom tool");            
        }
        Selection.activeTransform = gObj.transform;
    }

    private void OnDisable()
    {
        GhostsDestroy_F();
    }

    private void GhostsDestroy_F()
    {
        if(m_Ghosts.Count != 0)
        {
            foreach (GameObject ghost in m_Ghosts) DestroyImmediate(ghost);
            m_Ghosts.Clear();
        }
    }
}
#endif
