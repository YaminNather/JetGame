using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
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
