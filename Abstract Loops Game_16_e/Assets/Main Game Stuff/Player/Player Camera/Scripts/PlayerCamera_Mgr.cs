using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera_Mgr : MonoBehaviour
{
    #region Variables

    private CinemachineBrainMgr m_CinemachineBrainMgr;

    [SerializeField, FormerlySerializedAs("m_MinFOVCVC")] private CinemachineVirtualCamera m_SpectatorCVC;
    [SerializeField, FormerlySerializedAs("m_MaxFOVCVC")] private CinemachineVirtualCamera m_PlayerCVC;

    private Pawn m_Player;
    private bool IsPossessed => m_Player != null;
    #endregion

    private void Awake()
    {
        m_CinemachineBrainMgr = GetComponent<CinemachineBrainMgr>();
    }

    public void OnPossess_F(Pawn player)
    {
        m_Player = player;
        //m_MinFOVCVC.m_Follow = m_MinFOVCVC.m_LookAt = 

        m_CinemachineBrainMgr.TransitionTo_F(m_PlayerCVC);
        m_PlayerCVC.m_Follow = m_PlayerCVC.m_LookAt = player.transform;
    }

    public void OnUnPossess_F()
    {
        //Debug.Log("Camera OnUnPossess happened");
        m_Player = null;
        m_SpectatorCVC.transform.position = new Vector3(0.0f, 0.0f, MainGameReferences.INSTANCE.player.transform.position.z - 20f);
        m_CinemachineBrainMgr.TransitionTo_F(m_SpectatorCVC);
        m_SpectatorCVC.m_Follow = m_SpectatorCVC.m_LookAt = m_PlayerCVC.m_Follow = m_PlayerCVC.m_LookAt = null;
    }
}









//public class PlayerCamera_Mgr : MonoBehaviour
//{
//    #region Variables
//    private Camera m_Camera;
    
//    private Pawn m_Player;

//    private Vector3 m_OffsetDef;
//    private Vector3 m_OffsetCur;
//    #endregion

//    private void Awake()
//    {
//        m_Camera = GetComponent<Camera>();
//    }

//    public void OnPossess_F(Pawn player)
//    {
//        m_Player = player;

//        m_OffsetDef = transform.position - m_Player.transform.position;
//        m_OffsetCur = m_OffsetDef;

//        float fovOriginal = m_Camera.fieldOfView;
//        m_Camera.fieldOfView -= 20f;
//        GetComponent<Camera>().DOFieldOfView(fovOriginal, 3f).SetEase(Ease.InCirc);
//    }

//    public void OnUnPossess_F()
//    {
//        m_Player = null;
//    }

//    private void LateUpdate()
//    {
//        if(m_Player != null)
//            transform.position = new Vector3(transform.position.x, transform.position.y, m_Player.transform.position.z + m_OffsetCur.z);
//    }
//}

