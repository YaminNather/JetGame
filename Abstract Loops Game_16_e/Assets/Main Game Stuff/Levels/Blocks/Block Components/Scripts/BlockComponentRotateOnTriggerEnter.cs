using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponentRotateOnTriggerEnter : LevelComponent
{
    #region Variables
    [SerializeField] private Hitbox m_Hitbox;
    
    private bool m_ToRotate;
    [SerializeField] private float m_RotationSpeed = 20f;
    #endregion

    private void Awake() => m_Hitbox.ListenerAdd_F(HitboxOnEnter_EF);    

    private void Update()
    {
        if (m_ToRotate) 
            transform.Rotate(new Vector3(0f, 0f, m_RotationSpeed) * Time.deltaTime);
    }

    public override void Reset_F()
    {
        m_ToRotate = false;
        transform.localRotation = Quaternion.identity;
    }
    
    private void HitboxOnEnter_EF(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph))
        {
            Debug.Log($"<color=yellow>Fan to start rotating</color>", this.gameObject);
            m_ToRotate = true;
        }
    }
}
