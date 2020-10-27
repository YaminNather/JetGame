using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LoopsMgr : MonoBehaviour
{
    #region Variables
    private LoopMgrBase m_Loop;
    //[SerializeField] private AnimationCurve LoopTransition_AC;

    private readonly Vector3 NullPos = new Vector3(-999f, -999f, -999f);
    #endregion

    /// <summary>
    /// Get All Loops from the Loops Database and assign it to m_Loop.
    /// </summary>
    public void LoopsFieldSetup_F()
    {
        int loopCur = GlobalMgr.INSTANCE.m_GlobalData.LoopCur;
        LoopsDatabase loopsDatabase = GlobalMgr.INSTANCE.m_LoopsDatabase;
        
        if (loopCur != -1)
            m_Loop = loopsDatabase.LoopGet_F(loopCur);
        else
            m_Loop = loopsDatabase.LoopGet_F(Random.Range(0, loopsDatabase.Loops.Count));
    }

    /// <summary>
    /// Spawn a loop.
    /// </summary>
    /// <param name="m_Loop"></param>
    /// <param name="spawnPos"></param>
    public void LoopSpawn_F(Vector3 spawnPos)
    {
        m_Loop.gameObject.SetActive(true);
        m_Loop.OnSpawn_F();
        m_Loop.transform.position = spawnPos;
        m_Loop.IsSpawned = true;
    }

    /// <summary>
    /// Despawn a loop.
    /// </summary>
    /// <param name="loop"></param>
    public void LoopDespawn_F()
    {
        m_Loop.OnDespawn_F();
        m_Loop.IsSpawned = false;
        m_Loop.gameObject.SetActive(false);
        //loop.EndHitbox.ListenerRemove_F(LoopEndHitboxOnEnter_EF);
    }

}
