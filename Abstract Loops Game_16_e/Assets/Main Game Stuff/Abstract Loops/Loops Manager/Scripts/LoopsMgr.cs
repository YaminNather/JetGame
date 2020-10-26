﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LoopsMgr : MonoBehaviour
{
    #region Variables
    private LoopMgrBase[] m_Loops;
    //[SerializeField] private AnimationCurve LoopTransition_AC;

    private readonly Vector3 NullPos = new Vector3(-999f, -999f, -999f);
    #endregion

    /// <summary>
    /// Get All Loops from the Loops Database and assign it to m_Loops.
    /// </summary>
    public void LoopsFieldSetup_F()
    {
        m_Loops = GlobalMgr.INSTANCE.m_LoopsDatabase.Loops.ToArray();
    }

    /// <summary>
    /// Gets a random loop by going through all
    /// loops to find which isn't already spawned and calls LoopSpawn_F.
    /// </summary>
    public void RandomLoopSpawn_F(Vector3 spawnPos)
    {
        int index = 0;
        do
        {
            index = Random.Range(0, m_Loops.Length);
        } while (m_Loops[index].IsSpawned);
        
        LoopSpawn_F(m_Loops[index], spawnPos);
    }

    /// <summary>
    /// Spawn a loop.
    /// </summary>
    /// <param name="loop"></param>
    /// <param name="spawnPos"></param>
    public void LoopSpawn_F(LoopMgrBase loop, Vector3 spawnPos)
    {
        loop.gameObject.SetActive(true);
        loop.OnSpawn_F();
        loop.transform.position = spawnPos;
        loop.IsSpawned = true;
        //loop.EndHitbox.ListenerAdd_F(LoopEndHitboxOnEnter_EF);
    }

    /// <summary>
    /// Despawn a loop.
    /// </summary>
    /// <param name="loop"></param>
    public void LoopDespawn_F(LoopMgrBase loop)
    {
        loop.OnDespawn_F();
        loop.IsSpawned = false;
        loop.gameObject.SetActive(false);
        //loop.EndHitbox.ListenerRemove_F(LoopEndHitboxOnEnter_EF);
    }

    /// <summary>
    /// Despawns all spawned loops.
    /// </summary>
    public void LoopsAllDespawn_F()
    {
        foreach(LoopMgrBase loop in m_Loops)
        {
            if (loop.IsSpawned == true) LoopDespawn_F(loop);
        }
    }

    /// <summary>
    /// Adds this function to the loops End hitbox to tell it to white out the screen and spawn next loop.
    /// </summary>
    /// <param name="collider"></param>
    //private void LoopEndHitboxOnEnter_EF(Collider collider)
    //{
    //    if (collider.TryGetComponent(out PlayerHitbox ph))
    //    {
    //        Sequence Seq_00 = DOTween.Sequence();
    //        Seq_00.Append(DOTween.To(() => 0f, val =>
    //        {
    //            UnityEngine.UI.Image loopTransition = MainGameReferences.s_Instance.LoopTransition;
    //            loopTransition.color = loopTransition.color.With(a: val);
    //        }, 1f, 2f).SetEase(LoopTransition_AC));
    //        Seq_00.InsertCallback(0f, () => (MainGameReferences.s_Instance.playerController.PossessedPawn as JetPawn).InvincibilityStart_F());
    //        Seq_00.InsertCallback(1f, RandomLoopSpawn_F);
    //        Seq_00.InsertCallback(2f, () => (MainGameReferences.s_Instance.playerController.PossessedPawn as JetPawn).InvincibilityStop_F());
    //    }
    //}
}
