using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopMgrBase : MonoBehaviour
{
    #region Variables
    [HideInInspector] public Hitbox EndHitbox;

    [SerializeField] private bool m_IsSpawned;
    public bool IsSpawned { get => m_IsSpawned; set => m_IsSpawned = value; }
    #endregion

    protected virtual void Awake()
    {
        EndHitbox = GetComponentInChildren<Hitbox>();        
    }    

    /// <summary>
    /// Function to call when Loop is Spawned. Stuff like changing skybox material happens here.
    /// </summary>
    public virtual void OnSpawn_F()
    {

    }

    /// <summary>
    /// Function to call when Loop is Despawned.
    /// </summary>
    public virtual void OnDespawn_F()
    {

    }
}
