using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop1Mgr : LoopMgrBase
{
    private float m_Hue;

    public override void OnSpawn_F()
    {
        base.OnSpawn_F();
        m_Hue = Random.Range(0f, 1f);
    }

    private void Update()
    {
        m_Hue += 0.1f * Time.deltaTime;
        if (m_Hue >= 1f) m_Hue = 0f;
        MainGameReferences.s_Instance.colorMgr.Hue0 = m_Hue;
    }
}
