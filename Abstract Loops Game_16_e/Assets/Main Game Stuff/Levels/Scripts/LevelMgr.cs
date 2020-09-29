using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr : MonoBehaviour
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
