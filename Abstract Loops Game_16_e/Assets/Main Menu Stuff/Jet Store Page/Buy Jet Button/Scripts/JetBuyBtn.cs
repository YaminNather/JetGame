using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UI;

public class JetBuyBtn : Button
{
    #region Variables
    [SerializeField] private Image m_JetIcon;
    [SerializeField] private Text m_CostLbl;

    private int m_JetID;
    public int JetID { get => m_JetID; set => m_JetID = value; }
    private JetData jetData { get => GlobalDatabaseInitializer.INSTANCE.m_JetsDatabase.m_JetDatas[m_JetID]; }
    #endregion

    /// <summary>
    /// To be called by JetStoreMgr once to initialize this button by giving it an id, refreshing it and adding the click event.
    /// </summary>
    /// <param name="jetID"></param>
    public void Init_F(int jetID)
    {
        m_JetID = jetID;
        Refresh_F();
        onClick.AddListener(OnClick_EF);
    }

    /// <summary>
    /// Refreshing the look of the button to mirror the status of the Jet linked to this button.
    /// </summary>
    public void Refresh_F()
    {
        JetData jetData = GlobalDatabaseInitializer.INSTANCE.m_JetsDatabase.m_JetDatas[m_JetID];            
        
        /* If owned, deactivate CostLbl and change the sprite to the sprite of the jet,
        if not owned then activate cost and set sprite to common unowned sprite from the JeStoreMgr.*/
        if(GlobalDatabaseInitializer.INSTANCE.m_GlobalData.JetCheckIfOwned_F(m_JetID))
        {
            m_JetIcon.sprite = jetData.Icon;
            m_JetIcon.color = Color.red;
            m_CostLbl.gameObject.SetActive(false);
        }
        else
        {
            m_JetIcon.sprite = MainMenuSceneReferences.INSTANCE.jetStoreMgr.JetNotOwnedSprite;
            m_JetIcon.color = Color.white;
            m_CostLbl.gameObject.SetActive(true);
            m_CostLbl.text = "" + jetData.Cost;
        }
    }    

    /// <summary>
    /// Event Function to do something when the button is clicked.
    /// </summary>
    private void OnClick_EF()
    {
        GlobalData m_GlobalData = GlobalDatabaseInitializer.INSTANCE.m_GlobalData;
        /* If jet is owned, equip it, 
         * else check if u have enough cash to buy it, if enough buy it and equip it. */
        if (m_GlobalData.JetCheckIfOwned_F(m_JetID))
        {
            m_GlobalData.JetCur = m_JetID;
            m_GlobalData.Save_F();
        }
        else
        {            
            if (m_GlobalData.Currency >= jetData.Cost)
            {
                m_GlobalData.JetsOwnedAddTo_F(m_JetID);
                m_GlobalData.CurrencyChange_F(-jetData.Cost);
                m_GlobalData.JetCur = m_JetID;
                m_GlobalData.Save_F();
            }
            else Debug.LogError("Not Enough Cash!!!");
        }

        //Makes the jet linked with this button the current jet and refresh the button.
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetCurSet_F(m_JetID);
        Refresh_F();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(JetBuyBtn))]
public class JetBuyBtnEditor : ButtonEditor
{
    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
        UnityEngine.UIElements.VisualElement r = new UnityEngine.UIElements.VisualElement();
        
        r.Add(new PropertyField(serializedObject.FindProperty("m_JetIcon"), "Jet Icon")
        { 
            style = { marginTop = 20 }
        });
        
        r.Add(new PropertyField(serializedObject.FindProperty("m_CostLbl"), "Cost Label"));
        r.Add(new UnityEngine.UIElements.VisualElement()
        { 
            style = { marginBottom = 20 }
        });
        
        r.Add(new UnityEngine.UIElements.IMGUIContainer(() => base.OnInspectorGUI()));
        
        return r;
    }
}
#endif