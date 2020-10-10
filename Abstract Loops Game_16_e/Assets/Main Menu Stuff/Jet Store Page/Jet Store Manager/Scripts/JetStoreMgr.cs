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
    public Sprite JetNotOwnedSprite { get => m_JetNotOwnedSprite; }
    #endregion

    private void Awake()
    {
        m_MainMenuJetMgr = MainMenuSceneReferences.INSTANCE.mainMenuJetMgr;
        BuyBtnsSetup_F();
    }

    private void OnEnable()
    {
        m_VCamera.Priority = 20;
        GlobalData globalData = GlobalDatabaseInitializer.INSTANCE.m_GlobalData;
        m_JetSelected = globalData.JetCur;
        m_CurrencyValueLbl.text = "" + globalData.Currency;
    }

    private void BuyBtnsSetup_F()
    {
        GameObject firstBtn = m_BuyBtnsHolderTrans.transform.GetChild(0).gameObject;
        m_JetNotOwnedSprite = firstBtn.transform.GetChild(0).GetComponent<Image>().sprite;

        int aLength = m_MainMenuJetMgr.JetMeshes.Count - 1;
        foreach(KeyValuePair<int, JetData> kvp in GlobalDatabaseInitializer.INSTANCE.m_JetsDatabase.m_JetDatas)
        {
            GameObject gObj = Instantiate(firstBtn, m_BuyBtnsHolderTrans);
            gObj.GetComponent<JetBuyBtn>().Init_F(kvp.Key);
        }
        
        Destroy(firstBtn);

        m_JetSelected = GlobalDatabaseInitializer.INSTANCE.m_GlobalData.JetCur;
    }

    public void BackBtn_BEF()
    {        
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetCurSet_F(GlobalDatabaseInitializer.INSTANCE.m_GlobalData.JetCur);
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.Pages_EN.Main);
        m_VCamera.Priority = 0;
    }
}
