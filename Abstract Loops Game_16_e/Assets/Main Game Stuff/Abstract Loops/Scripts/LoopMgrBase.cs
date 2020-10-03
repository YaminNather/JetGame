using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopMgrBase : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool m_IsSpawned;
    public bool IsSpawned { get => m_IsSpawned; set => m_IsSpawned = value; }

    private Hitbox[] m_LoopEndHitboxes;
    public Hitbox[] LoopEndHitboxes { get => m_LoopEndHitboxes; }
    #endregion

    protected virtual void Awake()
    {
        m_LoopEndHitboxes = GetComponentsInChildren<Hitbox>();
        m_LoopEndHitboxes[0].ListenerAdd_F(LoopPart0EndHitboxOnEnter_EF);
        m_LoopEndHitboxes[1].ListenerAdd_F(LoopPart1EndHitboxOnEnter_EF);
    }    

    /// <summary>
    /// Function to call when Loop is Spawned. Stuff like changing skybox material happens here.
    /// </summary>
    public virtual void OnSpawn_F()
    {
        Reset_F();
    }

    /// <summary>
    /// Function to call when Loop is Despawned.
    /// </summary>
    public virtual void OnDespawn_F()
    {

    }

    private void LoopPart0EndHitboxOnEnter_EF(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph))
        {
            LoopPartMove_F(m_LoopEndHitboxes[0], m_LoopEndHitboxes[1]);
        }
    }

    private void LoopPart1EndHitboxOnEnter_EF(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph))
        {
            LoopPartMove_F(m_LoopEndHitboxes[1], m_LoopEndHitboxes[0]);
        }
    }

    private void LoopPartMove_F(Hitbox loopPart0EndHitbox, Hitbox loopPart1EndHitbox)
    {
        loopPart0EndHitbox.transform.parent.position = loopPart1EndHitbox.transform.position;
    }

    protected virtual void Reset_F()
    {
        m_LoopEndHitboxes[0].transform.parent.position = Vector3.zero;
        m_LoopEndHitboxes[1].transform.parent.position = m_LoopEndHitboxes[0].transform.position;
    }
}
