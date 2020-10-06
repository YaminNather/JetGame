#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways][AddComponentMenu("Block Stuff/BlockEditorMgr")][DisallowMultipleComponent]
public class BlockEditorMgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool m_IsRotating;
    [SerializeField] private Transform m_RotatingTrans;
    #endregion

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (transform.position.x != 0f || transform.position.y != 0f)
        {
            transform.position = new Vector3(0f, 0f, transform.position.z);
            Undo.RecordObject(transform, $"Reset Position of block {transform.name} to zero in X and Y axis");
        }

        if (m_IsRotating == true && m_RotatingTrans != null)
        {
            using (SerializedObject so = new SerializedObject(transform))
            {
                SerializedProperty localRotationSP = so.FindProperty("m_LocalRotation");
                if(localRotationSP.quaternionValue != Quaternion.identity)
                {
                    using(SerializedObject so_1 = new SerializedObject(m_RotatingTrans))
                    {                        
                        SerializedProperty localRotationSP_1 = so_1.FindProperty("m_LocalRotation");
                        localRotationSP_1.quaternionValue *= localRotationSP.quaternionValue;
                        so_1.ApplyModifiedProperties();
                    }
                    localRotationSP.quaternionValue = Quaternion.identity;
                    so.ApplyModifiedProperties();
                    Debug.Log("Changed rotation");
                }
            }
        }
    }

    private void OnValidate()
    {
        using (SerializedObject so = new SerializedObject(this))
        {
            if(m_RotatingTrans != null && (m_RotatingTrans == transform || GetComponentsInChildren<Transform>().Contains(m_RotatingTrans) == false))
            {
                so.Update();
                so.FindProperty("m_Transform").objectReferenceValue = null;
                so.ApplyModifiedProperties();
            }

            if (m_IsRotating == true && m_RotatingTrans == null)
            {
                so.Update();
                so.FindProperty("m_IsRotating").boolValue = false;
                so.ApplyModifiedProperties();
            }

        }
    }
}

[CustomEditor(typeof(BlockEditorMgr))]
public class BlockEditorMgrEditor : Editor
{
    private PropertyField m_RotatingTransPropertyField;

    public override VisualElement CreateInspectorGUI()
    {
        BlockEditorMgr target = (BlockEditorMgr)this.target;
        VisualElement r = new VisualElement();

        r.Add(new IMGUIContainer(() => DrawDefaultInspector()));

        //m_RotatingTransPropertyField = new PropertyField();
        //Transform Trans_0 = serializedObject.FindProperty("m_RotatingTrans").objectReferenceValue as Transform;
        //if (Trans_0 != null)
        //{
        //    SerializedProperty SP_1 = new SerializedObject(Trans_0).FindProperty("m_LocalRotation");
        //    m_RotatingTransPropertyField.BindProperty(SP_1);
        //}
        //else
        //    m_RotatingTransPropertyField.SetEnabled(false);
        //r.Add(m_RotatingTransPropertyField);

        //r.schedule.Execute(CheckForRotation_F).Every(50);
        
        Foldout Foldout_0 = new Foldout()
        {
            text = "Rotating Objects Transform",
            style =
            {
                marginTop = 10,
                marginBottom = 10,                
            }            
        };
        IMGUIContainer IMGUIContainer_0 = new IMGUIContainer()
        {
            style =
            { 
                marginTop = 10,
                marginBottom = 10
            },
            onGUIHandler = () =>
            {
                Editor objectEditor = CreateEditor(serializedObject.FindProperty("m_RotatingTrans").objectReferenceValue as Transform);                
                objectEditor.OnInspectorGUI();
            }
        };
        Foldout_0.Add(IMGUIContainer_0);        
        r.Add(Foldout_0);

        return r;
    }

    private void CheckForRotation_F()
    {
        serializedObject.Update();        
        if (serializedObject.FindProperty("m_IsRotating").boolValue == true && serializedObject.FindProperty("m_RotatingTrans").objectReferenceValue == true)
        {
            if (m_RotatingTransPropertyField.enabledSelf == false) m_RotatingTransPropertyField.SetEnabled(true);
        }
        else
        {
            if(m_RotatingTransPropertyField.enabledSelf == true) m_RotatingTransPropertyField.SetEnabled(false);
        }
    }
}
#endif
