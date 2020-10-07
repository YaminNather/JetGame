using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationComponentScale : UIAnimationComponent
{
    #region Variables
    [Space(20)]
    
    private Vector3 m_ScaleInit;    
    #endregion

    public override void OnSceneLoaded_F()
    {
        m_ScaleInit = transform.localScale;
    }

    public override void EntryInitialize_F()
    {
        transform.localScale = Vector3.zero;
    }

    public override Tweener EntryTweenMake_F()
    {
        Tweener r = transform.DOScale(m_ScaleInit, Duration);
        EntryTweenSetEase_F(r);
        return r;
    }

    public override void ExitInitialize_F()
    {
        
    }

    public override Tweener ExitTweenMake_F()
    {
        Tweener r = transform.DOScale(Vector3.zero, Duration);
        ExitTweenSetEase_F(r);
        return r;
    }
}
