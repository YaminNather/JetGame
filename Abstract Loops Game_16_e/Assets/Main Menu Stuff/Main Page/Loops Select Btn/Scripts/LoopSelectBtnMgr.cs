using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class LoopSelectBtnMgr : Button
{
    [SerializeField] private int m_Id;
    public int Id => m_Id;

    private void Awake()
    {
        onClick.AddListener(OnClick_BEF);
    }

    private void OnClick_BEF()
    {
        GlobalData globalData = GlobalMgr.INSTANCE.m_GlobalData;
        if (globalData.LoopCur == m_Id) return;

        globalData.LoopCur = m_Id;

        globalData.Save_F();

        foreach (LoopSelectBtnMgr btn in transform.parent.GetComponentsInChildren<LoopSelectBtnMgr>())
            btn.Refresh_F();
    }

    public void Refresh_F()
    {
        image.color = m_Id == GlobalMgr.INSTANCE.m_GlobalData.LoopCur ? Color.green : Color.cyan;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LoopSelectBtnMgr))]
public class LoopSelectBtnMgrEditor : ButtonEditor
{
    public override VisualElement CreateInspectorGUI()
    {

        VisualElement r = new VisualElement();

        r.Add(new Label("LOCAL PROPERTIES")
        {
            style =
            { 
                fontSize = 20, unityFontStyleAndWeight = FontStyle.Bold, unityTextAlign = TextAnchor.MiddleCenter, 
                marginTop = 20, marginBottom = 20

            }
        });
        
        PropertyField propertyField = new PropertyField();
        propertyField.BindProperty(serializedObject.FindProperty("m_Id"));
        r.Add(propertyField);

        r.Add(new Label("BUTTON PROPERTIES")
        {
            style =
            {
                fontSize = 20, unityFontStyleAndWeight = FontStyle.Bold, unityTextAlign = TextAnchor.MiddleCenter,
                marginTop = 20, marginBottom = 20
            }
        });

        r.Add(new IMGUIContainer(() => base.OnInspectorGUI()));


        return r;
    }
}
#endif
