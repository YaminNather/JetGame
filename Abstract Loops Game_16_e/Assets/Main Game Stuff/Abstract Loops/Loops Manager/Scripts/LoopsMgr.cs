using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LoopsMgr : MonoBehaviour
{
    #region Variables
    private LoopMgrBase[] m_Loops;
    [SerializeField] private AnimationCurve LoopTransition_AC;

    private readonly Vector3 NullPos = new Vector3(-999f, -999f, -999f);
    #endregion

    /// <summary>
    /// Get All Loops from the Loops Database and assign it to m_Loops.
    /// </summary>
    public void LoopsAssignAll_F()
    {
        m_Loops = GlobalDatabaseInitializer.s_Instance.loopsDatabase.Loops.ToArray();
    }

    /// <summary>
    /// Gets a random loop by going through all
    /// loops to find which isnt already spawned and calls LoopSpawn_F.
    /// </summary>
    public void RandomLoopSpawn_F()
    {
        int index = 0;
        do
        {
        index = Random.Range(0, m_Loops.Length);
        } while (m_Loops[index].IsSpawned);
        
        LoopSpawn_F(m_Loops[index]);
    }

    /// <summary>
    /// Spawn a loop.
    /// </summary>
    /// <param name="loop"></param>
    public void LoopSpawn_F(LoopMgrBase loop)
    {
        Pawn player = MainGameReferences.s_Instance.playerController.PossessedPawn;

        loop.gameObject.SetActive(true);
        loop.OnSpawn_F();
        loop.transform.position = new Vector3(0f, 0f, player.transform.position.z - 5f);
        loop.IsSpawned = true;
        loop.EndHitbox.ListenerAdd_F(LoopEndHitboxOnEnter_EF);
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
        loop.EndHitbox.ListenerRemove_F(LoopEndHitboxOnEnter_EF);
    }

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
    private void LoopEndHitboxOnEnter_EF(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerHitbox ph))
        {
            Sequence Seq_00 = DOTween.Sequence();
            Seq_00.Append(DOTween.To(() => 0f, val =>
            {
                UnityEngine.UI.Image loopTransition = MainGameReferences.s_Instance.LoopTransition;
                loopTransition.color = loopTransition.color.With(a: val);
            }, 1f, 2f).SetEase(LoopTransition_AC));
            Seq_00.InsertCallback(0f, () => (MainGameReferences.s_Instance.playerController.PossessedPawn as JetPawn).InvincibilityStart_F());
            Seq_00.InsertCallback(1f, RandomLoopSpawn_F);
            Seq_00.InsertCallback(2f, () => (MainGameReferences.s_Instance.playerController.PossessedPawn as JetPawn).InvincibilityStop_F());
        }
    }
}
