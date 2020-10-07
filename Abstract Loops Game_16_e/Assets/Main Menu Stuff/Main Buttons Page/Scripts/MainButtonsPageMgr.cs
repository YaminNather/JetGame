using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonsPageMgr : Page
{
    #region Variables
    [SerializeField] private RectTransform m_PlayBtn_RectTrans;
    [SerializeField] private RectTransform m_SettingsBtn_RectTrans;
    [SerializeField] private RectTransform m_JetStoreBtn_RectTrans;

    private Vector3 m_CenterPos;
    private Vector3 m_LeftPos;
    private Vector3 m_RightPos;
    private Vector3 m_BackPos;
    private readonly Vector3 CENTERSIZE = new Vector3(1.3f, 1.3f, 1.3f);
    #endregion


    private void Awake()
    {
        m_CenterPos = m_PlayBtn_RectTrans.anchoredPosition;
        m_LeftPos = m_JetStoreBtn_RectTrans.anchoredPosition;
        m_RightPos = m_SettingsBtn_RectTrans.anchoredPosition;        
    }

    public void ChangeBtnWithoutAnim_F(MainMenuPages_EN pageEnum)
    {
        switch(pageEnum)
        {
            case MainMenuPages_EN.Main:
                m_PlayBtn_RectTrans.anchoredPosition = m_CenterPos;
                m_PlayBtn_RectTrans.localScale = CENTERSIZE;
                m_JetStoreBtn_RectTrans.anchoredPosition = m_LeftPos;
                m_SettingsBtn_RectTrans.anchoredPosition = m_RightPos;
                m_JetStoreBtn_RectTrans.localScale = m_SettingsBtn_RectTrans.localScale = Vector3.one;
                break;
            
            case MainMenuPages_EN.JetStore:
                m_PlayBtn_RectTrans.anchoredPosition = m_CenterPos;
                m_JetStoreBtn_RectTrans.anchoredPosition = m_CenterPos;
                m_JetStoreBtn_RectTrans.localScale = CENTERSIZE;
                m_SettingsBtn_RectTrans.anchoredPosition = m_LeftPos;                
                m_PlayBtn_RectTrans.localScale = m_SettingsBtn_RectTrans.localScale = Vector3.zero;
                break;
            
            case MainMenuPages_EN.Settings:
                m_PlayBtn_RectTrans.anchoredPosition = m_CenterPos;
                m_SettingsBtn_RectTrans.anchoredPosition = m_CenterPos;
                m_SettingsBtn_RectTrans.localScale = CENTERSIZE;
                m_JetStoreBtn_RectTrans.anchoredPosition = m_RightPos;
                m_PlayBtn_RectTrans.localScale = m_JetStoreBtn_RectTrans.localScale = Vector3.zero;
                break;
        }
    }

    public void ChangeBtnWithAnim_F()
    { 
            
    }
}
