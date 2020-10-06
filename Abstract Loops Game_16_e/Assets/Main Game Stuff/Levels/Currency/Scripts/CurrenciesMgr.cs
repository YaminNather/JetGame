using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrenciesMgr : MonoBehaviour
{
    #region Variables
    private CurrenciesPSMgr m_PSMgr;
    private CurrenciesAddAnimMgr m_AddAnimMgr;
    #endregion

    private void Awake()
    {
        m_PSMgr = GetComponentInChildren<CurrenciesPSMgr>();
        m_AddAnimMgr = GetComponentInChildren<CurrenciesAddAnimMgr>();
    }

    public void DoCollectedAnim_F(Vector3 pos)
    {
        m_PSMgr.BurstDo_F(pos);
        m_AddAnimMgr.AnimDo_F(pos);
    }   
}
