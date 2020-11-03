using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JetStoreMgr : Page
{
    #region Variables
    private MainMenuJetMgr m_MainMenuJetMgr;
    [SerializeField] private CinemachineVirtualCamera m_VCamera;
    [SerializeField] private Transform m_BuyBtnsHolderTrans;
    [SerializeField] private Text m_CurrencyValueLbl;
    private int m_JetSelected;
    private Sprite m_JetNotOwnedSprite;
    public Sprite JetNotOwnedSprite => m_JetNotOwnedSprite;

    [SerializeField] private TutorialMgr m_Tutorial;
    #endregion

    private void Awake()
    {
        m_MainMenuJetMgr = MainMenuSceneReferences.INSTANCE.mainMenuJetMgr;
        BuyBtnsSetup_F();

        OnOpen_E += TutorialCheckAndDisplay_F;
    }

    private void OnEnable()
    {
        m_VCamera.Priority = 20;
        GlobalData globalData = GlobalMgr.s_Instance.m_GlobalData;
        m_JetSelected = globalData.JetCur;
        m_CurrencyValueLbl.text = "" + globalData.Currency;
    }

    public void TutorialCheckAndDisplay_F()
    {
        GlobalData globalData = GlobalMgr.s_Instance.m_GlobalData;

        if (!globalData.StoreTutorialDisplayed)
        {
            m_Tutorial.gameObject.SetActive(true);
            globalData.StoreTutorialDisplayed = true;
            globalData.Save_F();
        }
    }
    
    private void BuyBtnsSetup_F()
    {
        GameObject firstBtn = m_BuyBtnsHolderTrans.transform.GetChild(0).gameObject;
        m_JetNotOwnedSprite = firstBtn.transform.GetChild(0).GetComponent<Image>().sprite;

        int aLength = m_MainMenuJetMgr.JetMeshes.Count - 1;
        foreach(KeyValuePair<int, JetData> kvp in GlobalMgr.s_Instance.m_JetsDatabase.m_JetDatas)
        {
            GameObject gObj = Instantiate(firstBtn, m_BuyBtnsHolderTrans);
            gObj.GetComponent<JetBuyBtn>().Init_F(kvp.Key);
        }
        
        Destroy(firstBtn);

        m_JetSelected = GlobalMgr.s_Instance.m_GlobalData.JetCur;
    }

    public void AllBuyButtonsRefresh_F()
    {
        foreach (JetBuyBtn btn in GetComponentsInChildren<JetBuyBtn>())
        {
            btn.Refresh_F();
        }
    }

    #region Button Functions
    public void BackBtn_BEF()
    {        
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetCurSet_F(GlobalMgr.s_Instance.m_GlobalData.JetCur);
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.Main);
        m_VCamera.Priority = 0;
    }
    #endregion
}
