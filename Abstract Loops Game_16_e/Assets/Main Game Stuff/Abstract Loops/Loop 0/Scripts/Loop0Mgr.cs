using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This loop keeps rotating and changing the global hue.
/// </summary>
public class Loop0Mgr : LoopMgrBase
{
    #region Variables
    private Transform m_LoopPartsHolder;
    private float m_Hue;    
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_LoopPartsHolder = transform.GetChild(0);        
    }

    public override void OnSpawn_F()
    {
        base.OnSpawn_F();
        m_Hue = Random.Range(0f, 1f);
    }

    private void Update()
    {
        //m_Hue += 0.1f * Time.deltaTime;
        //if (m_Hue >= 1f) m_Hue = 0f;
        //MainGameReferences.INSTANCE.colorMgr.Hue0 = m_Hue;
        m_LoopPartsHolder.Rotate(0f, 0f, 90f * Time.deltaTime);
    }
    
    protected override void Reset_F()
    {
        base.Reset_F();
        m_LoopPartsHolder.rotation = Quaternion.identity;
    }
}
