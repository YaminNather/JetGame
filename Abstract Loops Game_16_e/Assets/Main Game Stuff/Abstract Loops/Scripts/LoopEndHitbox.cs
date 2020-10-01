using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopEndHitbox : Hitbox
{
    #region Variables
    private Loop0Mgr m_LoopMgr;

    private int m_ID;
    public int ID { get => m_ID; set => m_ID = value; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        m_LoopMgr = GetComponentInParent<Loop0Mgr>();
        ListenerAdd_F(LoopEndHitboxOnEnter_EF);
    }

    private void LoopEndHitboxOnEnter_EF(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            transform.position = m_LoopMgr.LoopEndHitboxes[(ID == 0) ? 1 : 0].transform.position + Vector3.forward * 5f;
        }
    }
}