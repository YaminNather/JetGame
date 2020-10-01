using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This loop keeps rotating and changing the global hue.
/// </summary>
public class Loop0Mgr : LoopMgrBase
{
    #region Variables
    [SerializeField] private Material m_Skybox_Mat;

    private Transform m_LoopPartsHolder;
    private float m_Hue;

    private Hitbox[] m_LoopEndHitboxes;
    public Hitbox[] LoopEndHitboxes { get => m_LoopEndHitboxes; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_LoopPartsHolder = transform.GetChild(0);

        m_LoopEndHitboxes = GetComponentsInChildren<Hitbox>();
        m_LoopEndHitboxes[0].ListenerAdd_F(LoopPart0EndHitboxOnEnter_EF);
        m_LoopEndHitboxes[1].ListenerAdd_F(LoopPart1EndHitboxOnEnter_EF);
    }

    public override void OnSpawn_F()
    {
        m_Hue = Random.Range(0f, 1f);
        RenderSettings.skybox = m_Skybox_Mat;
    }

    private void Update()
    {
        m_Hue += 0.1f * Time.deltaTime;
        if (m_Hue >= 1f) m_Hue = 0f;
        MainGameReferences.s_Instance.colorMgr.Hue0 = m_Hue;
        m_LoopPartsHolder.Rotate(0f, 0f, 90f * Time.deltaTime);
    }    
    
    private void LoopPart0EndHitboxOnEnter_EF(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            LoopPartMove_F(m_LoopEndHitboxes[0], m_LoopEndHitboxes[1]);
        }
    }

    private void LoopPart1EndHitboxOnEnter_EF(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph))
        {
            LoopPartMove_F(m_LoopEndHitboxes[1], m_LoopEndHitboxes[0]);
        }
    }

    private void LoopPartMove_F(Hitbox loopPart0EndHitbox, Hitbox loopPart1EndHitbox)
    {
        loopPart0EndHitbox.transform.parent.position = loopPart1EndHitbox.transform.position;
    }
}
