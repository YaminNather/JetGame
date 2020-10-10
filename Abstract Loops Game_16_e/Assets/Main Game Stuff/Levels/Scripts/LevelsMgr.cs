using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelsMgr : MonoBehaviour
{
    #region Variables
    private LevelMgr[] m_Levels;
    private Queue<LevelMgr> m_LevelsSpawned;
    private bool m_PlayerJustRevived;
    public bool PlayerJustRevived { get => m_PlayerJustRevived; set => m_PlayerJustRevived = value; }    

    private readonly Vector3 k_NullVector = new Vector3(-999f, -999f, -999f);
    #endregion

    private void Awake()
    {
        m_LevelsSpawned = new Queue<LevelMgr>(2);        
    }

    public LevelMgr LevelCurGet_F()
    {
        if (m_LevelsSpawned.Count == 0)
            return null;

        return m_LevelsSpawned.ElementAt(0);
    }

    public void LevelsAssignAll_F()
    {
        m_Levels = GlobalDatabaseInitializer.INSTANCE.m_LevelsDatabase.Levels.ToArray();
    }       

    public void RandomLevelSpawn_F()
    {
        if (m_LevelsSpawned.Count >= 2)
            return;

        int toSpawnCount = 2 - m_LevelsSpawned.Count;

        for (int i = 0; i < toSpawnCount; i++)
        {
            int index = 0, levelLength = m_Levels.Length;
            do
            {
                index = Random.Range(0, levelLength);
            } while (m_Levels[index].IsSpawned);
            
            LevelSpawn_F(m_Levels[index]);
        }

    }

    public void LevelSpawn_F(LevelMgr level)
    {
        Vector3 spawnPos = LevelSpawnPosGet_F();        
        level.transform.position = spawnPos;
        level.gameObject.SetActive(true);
        level.OnSpawn_F();
        level.IsSpawned = true;
        Hitbox endHitbox = level.EndHitbox;
        endHitbox.ListenerAdd_F(LevelHitboxOnEnterSpawnNextLevel_EF);

        m_LevelsSpawned.Enqueue(level);        
    }

    public Vector3 LevelSpawnPosGet_F()
    {
        switch (m_LevelsSpawned.Count)
        {
            case 0:
                if (m_PlayerJustRevived)
                {
                    m_PlayerJustRevived = false;
                    return new Vector3(0f, 0f, MainGameReferences.INSTANCE.player.transform.position.z);
                }
                else
                    return Vector3.zero;

            default:
                return m_LevelsSpawned.Last().EndHitbox.transform.position;
        }
    }

    private void LevelHitboxOnEnterSpawnNextLevel_EF(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph)) RandomLevelSpawn_F();
    }

    public void LevelDespawn_F(LevelMgr level)
    {
        if (!level.IsSpawned) return;

        level.IsSpawned = false;
        level.EndHitbox.ListenerRemove_F(LevelHitboxOnEnterSpawnNextLevel_EF);
        level.OnDespawn_F();
        level.gameObject.SetActive(false);

        m_LevelsSpawned.Dequeue();
    }

    public void LevelsDespawnAll_F()
    {
        LevelMgr[] levels = m_LevelsSpawned.ToArray();
        for (int i = 0; i < levels.Length; i++)
        {            
            LevelDespawn_F(levels[i]);
        }
    }
}
