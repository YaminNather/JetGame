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
            if (MainGameReferences.s_Instance != null)
            {
                MainGameReferences.s_Instance.scoreMgr.CurrencyAdd_F(1);
                MainGameReferences.s_Instance.currencyPSMgr.DoBurst_F(transform.position);
            }
            gameObject.SetActive(false);
        }
    }
}
