using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyMgr : LevelComponent
{
    public override void Reset_F()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            if (MainGameReferences.INSTANCE != null)
            {
                MainGameReferences.INSTANCE.scoreMgr.CurrencyAdd_F(1);
                MainGameReferences.INSTANCE.currencyPSMgr.DoBurst_F(transform.position);
            }
            gameObject.SetActive(false);
        }
    }
}
