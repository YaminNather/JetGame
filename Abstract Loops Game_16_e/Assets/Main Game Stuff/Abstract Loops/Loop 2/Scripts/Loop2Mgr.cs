using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop2Mgr : LoopMgrBase
{
    #region Variables
    private Transform m_Part0_Trans;
    private Transform m_Part1_Trans;

    private float m_Hue0;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_Part0_Trans = transform.GetChild(0);
        m_Part1_Trans = transform.GetChild(1);
    }

    private void Update()
    {
        m_Hue0 += Time.deltaTime;
        if (m_Hue0 >= 1f) m_Hue0 = 0f;
        MainGameReferences.s_Instance.colorMgr.Hue0 = m_Hue0;

        m_Part0_Trans.Rotate(new Vector3(0f, 0f, 60f) * Time.deltaTime);
        m_Part1_Trans.rotation = m_Part0_Trans.rotation;
    }

    protected override void Reset_F()
    {
        base.Reset_F();
        m_Part0_Trans.rotation = Quaternion.identity;
        m_Part1_Trans.rotation = Quaternion.identity;
        m_Hue0 = 0;
    }
}
