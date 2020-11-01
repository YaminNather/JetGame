using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI Animation/Scale")]
public class UIAnimationComponentScale : UIAnimationComponent
{
    #region Variables
    private Vector3 m_ScaleInit;
    #endregion

    public override void OnSceneLoaded_F() => m_ScaleInit = transform.localScale;   

    public override void EntryInitialize_F() => GetComponent<RectTransform>().localScale = Vector3.zero;

    public override Tweener EntryTween_F()
    {
        Tweener r = GetComponent<RectTransform>().DOScale(m_ScaleInit, Duration);
        EntryTweenSetEase_F(r);
        return r;
    }

    public override void ExitInitialize_F() { }    

    public override Tweener ExitTween_F()
    {
        Tweener r = GetComponent<RectTransform>().DOScale(Vector3.zero, Duration);
        ExitTweenSetEase_F(r);
        return r;
    }   
}
