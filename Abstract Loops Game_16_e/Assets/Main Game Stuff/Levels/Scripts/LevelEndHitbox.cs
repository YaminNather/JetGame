using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndHitbox : Hitbox
{
    private LevelMgr level;

    protected override void Awake()
    {
        base.Awake();

        level = GetComponentInParent<LevelMgr>();
        if (level != null)
            ListenerAdd_F(HitboxOnEnterLevelDespawn_EF);
        else
            Destroy(gameObject);
    }

    private void HitboxOnEnterLevelDespawn_EF(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            if (MainGameReferences.INSTANCE != null) MainGameReferences.INSTANCE.levelsMgr.LevelDespawn_F(level);
            else FindObjectOfType<LevelsMgr>()?.LevelDespawn_F(level);
        }
    }
}
