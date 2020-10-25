using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

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
    private static bool ReorderLevelChildrenValidator_F(MenuCommand menuCommand)
    {
        return PrefabStageUtility.GetCurrentPrefabStage() != null;
    }

    [MenuItem("CONTEXT/LevelMgr/Reorder Children")]
    private static void ReorderLevelChildren_F(MenuCommand menuCommand)
    {
        Transform level = (menuCommand.context as LevelMgr).transform;

        for (int i = 0; i < level.childCount - 1; i++)
        {
            Transform min = level.GetChild(i);
            for (int j = i + 1; j < level.childCount; j++)
            {
                if (level.GetChild(j).transform.position.z < min.transform.position.z)
                    min = level.GetChild(j);
            }
            Undo.SetTransformParent(min.transform, level, "Reordered Level Children");
            min.SetSiblingIndex(i);
        }

    }
}
#endif
