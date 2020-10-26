using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelsMgr : MonoBehaviour
{
    #region Variables
    private LevelMgr[] m_EasyLevels;
    private LevelMgr[] m_NormalLevels;
    private LevelMgr[] m_HardLevels;
    private Queue<LevelMgr> m_LevelsSpawned;

    private readonly Vector3 k_NullVector = new Vector3(-999f, -999f, -999f);

#if UNITY_EDITOR
    [Header("Testing Stuff")]
    [SerializeField] private LevelMgr[] m_TestLevels;
#endif   
    #endregion

    private void Awake()
    {
        m_LevelsSpawned = new Queue<LevelMgr>(2);        
    }

    public void GetAllLevelsForGame_F()
    {
        LevelsDatabase ldb = GlobalMgr.INSTANCE.m_LevelsDatabase;
#if UNITY_EDITOR
        if (m_TestLevels == null || m_TestLevels.Length == 0)
        {
#endif
            m_EasyLevels = ldb.EasyLevels;
            m_NormalLevels = ldb.NormalLevels;
            m_HardLevels = ldb.HardLevels;
#if UNITY_EDITOR
        }
        else
        {
            if (m_TestLevels.Length == 1)
            {
                m_EasyLevels = m_NormalLevels = m_HardLevels = new LevelMgr[2];
                for (int i = 0; i < 2; i++) 
                    m_EasyLevels[i] = Instantiate(m_TestLevels[0].gameObject).GetComponent<LevelMgr>();
            }
            else
            {
                m_EasyLevels = m_NormalLevels = m_HardLevels = new LevelMgr[m_TestLevels.Length];
                for (int i = 0; i < m_TestLevels.Length; i++)
                    m_EasyLevels[i] = Instantiate(m_TestLevels[i].gameObject).GetComponent<LevelMgr>();
            }

            foreach (LevelMgr level in m_EasyLevels)
            {
                level.gameObject.SetActive(false);
                level.Init_F();
            }
        }
#endif
    }       
    
    public LevelMgr LevelCurGet_F()
    {
        if (m_LevelsSpawned.Count == 0)
            return null;

        return m_LevelsSpawned.ElementAt(0);
    }

    public void RandomLevelSpawn_F(Vector3 spawnPos)
    {
        if (m_LevelsSpawned.Count >= 2)
            return;

        int toSpawnCount = 2 - m_LevelsSpawned.Count;

        for (int i = 0; i < toSpawnCount; i++)
        {
            LevelMgr level = RandomLevelGet_F(MainGameReferences.INSTANCE.mainGameMgr.Difficulty);
            LevelSpawn_F(level, spawnPos);
            spawnPos = level.EndHitbox.transform.position;
        }

    }

    public LevelMgr RandomLevelGet_F(MainGameMgr.DifficultyEN difficulty)
    {
        LevelMgr[] levels = difficulty switch
        {
            MainGameMgr.DifficultyEN.Easy => m_EasyLevels,
            MainGameMgr.DifficultyEN.Normal => m_NormalLevels,
            MainGameMgr.DifficultyEN.Hard => m_HardLevels,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };

        int index = 0, levelLength = levels.Length;
        do
        {
            index = Random.Range(0, levelLength);
        } while (levels[index].IsSpawned);

        return levels[index];
    }

    public void LevelSpawn_F(LevelMgr level, Vector3 spawnPos)
    {
        level.transform.position = spawnPos;
        level.gameObject.SetActive(true);
        level.OnSpawn_F();
        level.IsSpawned = true;
        Hitbox endHitbox = level.EndHitbox;
        endHitbox.ListenerAdd_F(LevelHitboxOnEnterSpawnNextLevel_EF);

        m_LevelsSpawned.Enqueue(level);        
    }

    //public Vector3 LevelSpawnPosGet_F()
    //{
    //    switch (m_LevelsSpawned.Count)
    //    {
    //        case 0:
    //            if (m_PlayerJustRevived)
    //            {
    //                m_PlayerJustRevived = false;
    //                return new Vector3(0f, 0f, MainGameReferences.INSTANCE.player.transform.position.z);
    //            }
    //            else
    //                return Vector3.zero;

    //        default:
    //            return m_LevelsSpawned.Last().EndHitbox.transform.position;
    //    }
    //}

    private void LevelHitboxOnEnterSpawnNextLevel_EF(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph)) RandomLevelSpawn_F(m_LevelsSpawned.Last().EndHitbox.transform.position);
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
