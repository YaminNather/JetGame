#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways][AddComponentMenu("Block Stuff/BlockEditorMgr")][DisallowMultipleComponent]
public class BlockEditorMgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform m_RotationObjTrans;
    public Transform RotationObjTrans { get => m_RotationObjTrans; }
    #endregion

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (transform.position.x != 0f || transform.position.y != 0f)
        {
            transform.position = new Vector3(0f, 0f, transform.position.z);
            Undo.RecordObject(transform, $"Reset Position of block {transform.name } to zero in X and Y axis");
        }
    }
}
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(BlockEditorMgr))]
public class BlockEditorMgrEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        BlockEditorMgr target = this.target as BlockEditorMgr;
        VisualElement r = new VisualElement();

        r.Add(new IMGUIContainer(() => DrawDefaultInspector()));

        SerializedObject sObj = null;
        if(target.GetComponentInChildren<BlockComponentRotation>()) sObj = new SerializedObject(target.GetComponentInChildren<BlockComponentRotation>());
        else if(target.GetComponentInChildren<BlockComponentRotateOnTriggerEnter>()) sObj = new SerializedObject(target.GetComponentInChildren<BlockComponentRotateOnTriggerEnter>());
        if (sObj != null)
        {            
            PropertyField rotationDirField = new PropertyField();
            rotationDirField.BindProperty(sObj.FindProperty("m_RotationDir"));
            r.Add(rotationDirField);
        }

        return r;
    }
}
#endif