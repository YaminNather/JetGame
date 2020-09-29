using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JetStoreMgr : Page
{
    #region Variables
    [Space(50)]
    [SerializeField] private Text CurrencyValue_Lbl;
    [SerializeField] private Text JetName_Lbl;
    [SerializeField] private Text BuyAmount_Lbl;
    [SerializeField] private GameObject Equip_Btn;
    [SerializeField] private GameObject Equipped_Icon;

    private GlobalData gd;

    private JetDisplayMgr jetDisplayMgr;
    private List<JetData> m_JetsDatas;
    private JetData JetCurData { get => m_JetsDatas[m_Index]; }
    private int m_Index;
    #endregion

    private void OnEnable()
    {
        Init_F();
        RefreshCurrencyValueLbl_F();
    }

    private void Awake()
    {
        gd = GlobalDatabaseInitializer.s_Instance.globalData;
        jetDisplayMgr = MainMenuSceneReferences.s_Instance.jetDisplayMgr;
        m_JetsDatas = new List<JetData>();
    }

    private void Init_F()
    {
        m_JetsDatas.Clear();
        int index = 0;
        foreach (KeyValuePair<string, JetData> kvp in jetDisplayMgr.Jets)
        {
            m_JetsDatas.Add(kvp.Value);
            if (kvp.Key == jetDisplayMgr.JetCur.PlayerName)
            {
                m_Index = index;
            }
            index++;
        }

        Refresh_F();
    }    

    private void Refresh_F()
    {
        JetData jetCurData = m_JetsDatas[m_Index];
        jetDisplayMgr.JetChange_F(jetCurData.PlayerName);
        JetName_Lbl.text = jetCurData.PlayerName;
        RefreshBuyButtons_F();
    }

    private void RefreshBuyButtons_F()
    {
        JetData jetCurData = m_JetsDatas[m_Index];
        if (gd.JetCheckIfOwned_F(jetCurData.PlayerName))
        {
            BuyAmount_Lbl.transform.parent.gameObject.SetActive(false);
            bool IsEquipped = jetCurData.PlayerName == gd.JetCur;
            Equip_Btn.SetActive(!IsEquipped);
            Equipped_Icon.SetActive(IsEquipped);
        }
        else
        {
            BuyAmount_Lbl.transform.parent.gameObject.SetActive(true);
            Equip_Btn.SetActive(false);
            Equipped_Icon.SetActive(false);
            BuyAmount_Lbl.text = "" + jetCurData.Cost;
        }
    }

    private void RefreshCurrencyValueLbl_F()
    {
        CurrencyValue_Lbl.text = gd.Currency + "";
    }

    #region Button Functions
    public void LeftBtn_BEF()
    {
        m_Index--;
        if (m_Index < 0) m_Index = m_JetsDatas.Count - 1;
        Refresh_F();
    }

    public void RightBtn_BEF()
    {
        m_Index++;
        if (m_Index >= m_JetsDatas.Count) m_Index = 0;
        Refresh_F();
    }

    public void BuyBtn_BEF()
    {
        if (JetCurData.Cost > gd.Currency) return;

        gd.JetsOwnedAddTo_F(JetCurData.PlayerName);
        jetDisplayMgr.JetPurchased_F();        
        RefreshBuyButtons_F();
        gd.CurrencyChange_F(-JetCurData.Cost);
        gd.JetCur = JetCurData.PlayerName;
        RefreshCurrencyValueLbl_F();
    }

    public void EquipBtn_BEF()
    {
        gd.JetCur = JetCurData.PlayerName;
        gd.Save_F();
        RefreshBuyButtons_F();        
    }

    public void CloseBtn_BEF()
    {
        Close_F(MainMenuSceneReferences.s_Instance.mainMenuMgr);        
    }
    #endregion
}
