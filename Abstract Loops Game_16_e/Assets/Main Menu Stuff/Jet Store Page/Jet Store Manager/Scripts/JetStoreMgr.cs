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
        m_JetSelected = GlobalDatabaseInitializer.INSTANCE.m_GlobalData.JetCur;
    }

    private void BuyBtnsSetup_F()
    {
        GameObject firstBtn = m_BuyBtnsHolderTrans.transform.GetChild(0).gameObject;
        m_JetNotOwnedSprite = firstBtn.transform.GetChild(0).GetComponent<Image>().sprite;

        int aLength = m_MainMenuJetMgr.JetDatasAndGObjs.Count - 1;
        foreach(KeyValuePair<int, JetData> kvp in GlobalDatabaseInitializer.INSTANCE.m_JetsDatabase.m_JetDatas)
        {
            GameObject gObj = Instantiate(firstBtn, m_BuyBtnsHolderTrans);
            gObj.GetComponent<JetBuyBtn>().Init_F(kvp.Key);
        }
        
        Destroy(firstBtn);

        m_JetSelected = GlobalDatabaseInitializer.INSTANCE.m_GlobalData.JetCur;
    }

    public void JetSelectedSet_F(int id)
    {
        if (m_JetSelected == id) return;

        m_JetSelected = id;
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetCurSet_F(id);
    }
}
