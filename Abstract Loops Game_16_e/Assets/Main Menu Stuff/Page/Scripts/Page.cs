using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
public class Page : MonoBehaviour
{
    #region Variables
    private UIAnimationComponent[] m_UIAnimationComponents;
    protected Action OnOpen_E;
    protected Action OnClose_E;

    protected bool Clickable { get => GetComponent<GraphicRaycaster>().enabled; set => GetComponent<GraphicRaycaster>().enabled = value; }

    protected State_EN m_State = State_EN.Closed;
    public State_EN State { get => m_State; }
    #endregion

    public virtual void Open_F()
    {
        if (m_State != State_EN.Closed) return;

        m_State = State_EN.Opening;
        gameObject.SetActive(true);
        Clickable = false;

        m_UIAnimationComponents = GetComponentsInChildren<UIAnimationComponent>();
        Sequence Seq_0 = DOTween.Sequence();
        int aLength = m_UIAnimationComponents.Length;
        for(int i = 0; i < aLength; i++)
        {
            if (m_UIAnimationComponents[i].AnimationType == UIAnimationType_EN.Exit)
                continue;
            m_UIAnimationComponents[i].EntryInitialize_F();
            Seq_0.Insert(m_UIAnimationComponents[i].StartTime, m_UIAnimationComponents[i].EntryTweenMake_F());            
        }

        Seq_0.AppendCallback(() =>
        {
            Clickable = true;
            OnOpen_E?.Invoke();
            m_State = State_EN.Open; 
        });        
    }
    
    public virtual void Close_F(Page pageToOpen = null)
    {
        if (m_State != State_EN.Open) return;

        m_State = State_EN.Closing;
        GetComponent<GraphicRaycaster>().enabled = false;
        m_UIAnimationComponents = GetComponentsInChildren<UIAnimationComponent>();

        Sequence Seq_0 = DOTween.Sequence();
        foreach(UIAnimationComponent uiac in m_UIAnimationComponents)
        {
            if (uiac.AnimationType == UIAnimationType_EN.Entry) continue;
            uiac.ExitInitialize_F();
            Seq_0.Insert(uiac.StartTime, uiac.ExitTweenMake_F());
        }
        Seq_0.AppendCallback(() =>
        {
            GetComponent<GraphicRaycaster>().enabled = true;
            OnClose_E?.Invoke();
            gameObject.SetActive(false);
            pageToOpen?.Open_F();
            m_State = State_EN.Closed;
        });        
    }

    public enum State_EN { Closed, Opening, Open, Closing }
}
