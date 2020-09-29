using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
public class Page : MonoBehaviour
{
    #region Variables
    private AnimatedUIComponent[] m_AnimatedUIComponents;    

    [Header("Page Settings")]
    [SerializeField] private float m_OpenTime = 2;        
    #endregion   

    public virtual void Open_F()
    {
        gameObject.SetActive(true);
        GetComponent<GraphicRaycaster>().enabled = false;

        m_AnimatedUIComponents = GetComponentsInChildren<AnimatedUIComponent>();
        int orderCount = OrderMaxFind_F() + 1;
        float indiTime = m_OpenTime / orderCount;
        Debug.Log($"<color=yellow>indiTime = {indiTime}</color>");
        
        Sequence Seq_0 = DOTween.Sequence();

        int aLength = m_AnimatedUIComponents.Length;
        for(int i = 0; i < aLength; i++)
        {
            //Debug.Log($"<color=yellow>i = {i}\nAnimated Component Name = {m_AnimatedUIComponents[i].transform.name}\norder = {m_AnimatedUIComponents[i].m_Order}\nInsert Time = {m_AnimatedUIComponents[i].m_Order * indiTime}</color>");
            int i_1 = i;
            float animStartTime = m_AnimatedUIComponents[i_1].m_Order * indiTime;
            m_AnimatedUIComponents[i].EntryInitialize_F();
            Seq_0.InsertCallback(animStartTime, () =>
            {
                m_AnimatedUIComponents[i_1].Enter_F(m_OpenTime - animStartTime);
            });
        }

        Seq_0.InsertCallback(m_OpenTime, () => GetComponent<GraphicRaycaster>().enabled = true);
    }

    public virtual void Close_F(Page page = null)
    {
        GetComponent<GraphicRaycaster>().enabled = false;

        m_AnimatedUIComponents = GetComponentsInChildren<AnimatedUIComponent>();
        int orderCount = OrderMaxFind_F() + 1;
        float indiTime = m_OpenTime / orderCount;
        Debug.Log($"<color=yellow>indiTime = {indiTime}</color>");

        Sequence Seq_0 = DOTween.Sequence();

        int aLength = m_AnimatedUIComponents.Length;
        for (int i = 0; i < aLength; i++)
        {
            //Debug.Log($"<color=yellow>i = {i}\nAnimated Component Name = {m_AnimatedUIComponents[i].transform.name}\norder = {m_AnimatedUIComponents[i].m_Order}\nInsert Time = {m_AnimatedUIComponents[i].m_Order * indiTime}</color>");
            int i_1 = i;
            float animStartTime = m_AnimatedUIComponents[i_1].m_Order * indiTime;
            m_AnimatedUIComponents[i].ExitInitialize_F();
            Seq_0.InsertCallback(animStartTime, () =>
            {
                m_AnimatedUIComponents[i_1].Exit_F(m_OpenTime - animStartTime);
            });
        }

        Seq_0.InsertCallback(m_OpenTime, () =>
        {
            GetComponent<GraphicRaycaster>().enabled = true;
            gameObject.SetActive(false);
            page?.Open_F();
        });        
            
    }

    private int OrderMaxFind_F()
    {
        int r = 0;
        foreach(AnimatedUIComponent auic in m_AnimatedUIComponents)
        {
            if (r < auic.m_Order) r = auic.m_Order;
        }
        
        return r;
    }
}
