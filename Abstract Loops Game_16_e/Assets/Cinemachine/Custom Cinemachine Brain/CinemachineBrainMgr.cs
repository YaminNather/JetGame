using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(CinemachineBrain))]
public class CinemachineBrainMgr : MonoBehaviour
{
    #region Variables
    private CinemachineBrain m_CinemachineBrain;

    private CinemachineVirtualCamera m_CVCCur;
    public CinemachineVirtualCamera CVCCur => m_CVCCur;
    #endregion

    private void Awake()
    {
        m_CinemachineBrain = GetComponent<CinemachineBrain>();
        m_CVCCur = m_CinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        m_CVCCur.m_Priority = 10;
    }

    public void TransitionTo_F(CinemachineVirtualCamera cvc)
    {
        if (cvc == null) return;

        m_CVCCur.m_Priority = 0;
        cvc.m_Priority = 10;
        m_CVCCur = cvc;
    }
}
