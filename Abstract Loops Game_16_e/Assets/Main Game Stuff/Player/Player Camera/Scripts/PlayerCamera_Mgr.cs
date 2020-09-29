using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera_Mgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private CinemachineVirtualCamera m_MinFOVCVC;
    [SerializeField] private CinemachineVirtualCamera m_MaxFOVCVC;

    private Pawn m_Player;
    private bool IsPossessed { get => m_Player != null; }

    private JetMovementComponent jmc;
    #endregion

    private void Awake()
    {
        m_MinFOVCVC.m_Priority = 1;
        m_MaxFOVCVC.m_Priority = 0;
    }

    public void OnPossess_F(Pawn player)
    {
        m_Player = player;
        jmc = player.GetComponent<JetMovementComponent>();
        m_MaxFOVCVC.m_Priority = 2;
        m_MinFOVCVC.m_Follow = m_MinFOVCVC.m_LookAt = m_MaxFOVCVC.m_Follow = m_MaxFOVCVC.m_LookAt = player.transform;
    }

    public void OnUnPossess_F()
    {
        Debug.Log("Camera OnUnPossess happened");
        m_Player = null;
        jmc = null;
        m_MaxFOVCVC.m_Priority = 0;
        m_MinFOVCVC.m_Follow = m_MinFOVCVC.m_LookAt = m_MaxFOVCVC.m_Follow = m_MaxFOVCVC.m_LookAt = null;
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

