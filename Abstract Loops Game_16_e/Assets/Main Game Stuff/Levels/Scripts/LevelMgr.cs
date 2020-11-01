using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif

public partial class LevelMgr : MonoBehaviour
{
    #region Variables
    public Hitbox EndHitbox;
    private LevelComponent[] m_LevelComponents;

    private bool m_IsSpawned;
    public bool IsSpawned { get => m_IsSpawned; set => m_IsSpawned = value; }
    #endregion

    public void Init_F()
    {
        m_LevelComponents = GetComponentsInChildren<LevelComponent>(true);
    }

    public void Reset_F()
    {
        foreach (LevelComponent lc in m_LevelComponents)
            lc.Reset_F();
    }

    public void OnSpawn_F()
    {
        Reset_F();
    }

    public void OnDespawn_F()
    {

    }
}

#if UNITY_EDITOR
public partial class LevelMgr : MonoBehaviour
{
    [MenuItem("CONTEXT/LevelMgr/Reorder Children", true)]
    private static bool ReorderLevelChildrenFromContextMenuValidator_F(MenuCommand menuCommand) =>
        PrefabStageUtility.GetCurrentPrefabStage() != null;

    [MenuItem("CONTEXT/LevelMgr/Reorder Children")]
    private static void ReorderLevelChildrenFromContextMenu_F(MenuCommand menuCommand) => 
        ReorderLevelChildren_F((menuCommand.context as LevelMgr));

    [MenuItem("Spawn Level Prefab Tool/Reorder Level Children", true)]
    private static bool ReorderLevelChildrenFromTitleBarValidator_F()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        return prefabStage != null && prefabStage.prefabContentsRoot.GetComponent<LevelMgr>() != null;
    }

    [MenuItem("Spawn Level Prefab Tool/Reorder Level Children")]
    private static void ReorderLevelChildrenFromTitleBar_F() =>
        ReorderLevelChildren_F(PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.GetComponent<LevelMgr>());

    private static void ReorderLevelChildren_F(LevelMgr level)
    {
        Transform levelTrans = level.transform;
        for (int i = 0; i < levelTrans.childCount - 1; i++)
        {
            Transform min = levelTrans.GetChild(i);
            for (int j = i + 1; j < levelTrans.childCount; j++)
            {
                if (levelTrans.GetChild(j).transform.position.z < min.transform.position.z)
                    min = levelTrans.GetChild(j);
            }

            Undo.SetTransformParent(min.transform, levelTrans, "Reordered Level Children");
            min.SetSiblingIndex(i);
        }
    }
}
#endif
