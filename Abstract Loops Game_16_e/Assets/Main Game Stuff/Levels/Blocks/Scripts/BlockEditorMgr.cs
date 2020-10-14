#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
