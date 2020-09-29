using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopEndHitbox : Hitbox
{
    private LoopMgrBase Loop;

    protected override void Awake()
    {
        base.Awake();

        Loop = GetComponentInParent<LoopMgrBase>();
        if(Loop) ListenerAdd_F(LoopEndHitboxOnEnter_EF);
    }

    private void LoopEndHitboxOnEnter_EF(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            if (MainGameReferences.s_Instance != null) MainGameReferences.s_Instance.loopsMgr.LoopDespawn_F(Loop);
        }
    }
}
