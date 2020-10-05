using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public partial class Hitbox : MonoBehaviour
{
    #region Variables
    [SerializeField] protected UnityEvent<Collider> m_Trigger_E; // The Trigger Event which takes place on collision.
    #endregion

    protected virtual void Awake()
    {
        if(TryGetComponent(out Collider col)) col.isTrigger = true;
    }

    /// <summary>
    /// Adds listeners to the trigger Event. 
    /// </summary>
    /// <param name="action"></param>
    public void ListenerAdd_F(UnityAction<Collider> action)
    {
        if (action != null)
            m_Trigger_E.AddListener(action);
    }

    /// <summary>
    /// Removes listeners from the trigger Event. 
    /// </summary>
    /// <param name="action"></param>
    public void ListenerRemove_F(UnityAction<Collider> action)
    {
        if (action != null)
            m_Trigger_E.RemoveListener(action);
    }

    /// <summary>
    /// Removes all listeners from the trigger Event. 
    /// </summary>
    /// <param name="action"></param>
    public void RemoveAllListeners_F() => m_Trigger_E.RemoveAllListeners();

    private void OnTriggerEnter(Collider collider)
    {
        m_Trigger_E?.Invoke(collider);
    }
}

#if UNITY_EDITOR
public partial class Hitbox : MonoBehaviour
{
    [MenuItem("GameObject/Hitboxes/Box Hitbox", false, priority = 10)]
    public static void BoxHitboxCreateMenuItem_F(MenuCommand menuCommand)
    {
        GameObject createdGObj = new GameObject("Box Hitbox", new System.Type[] { typeof(BoxCollider), typeof(Hitbox) });
        BoxCollider boxCollider = createdGObj.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        GameObjectUtility.SetParentAndAlign(createdGObj, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(createdGObj, "Created a box Hitbox");
        Selection.activeGameObject = createdGObj;
        Debug.Log($"menuCommand.context = {menuCommand.context}", menuCommand.context);
    }
}

public class EditorWindowUndoTest : EditorWindow
{ 

    [MenuItem("Editor Window Undo Test/Create Window")]
    public static void CreateEditorWindow_F()
    {
        CreateWindow<EditorWindowUndoTest>();
    }
}
#endif
