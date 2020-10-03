using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Loop2PartMgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform Set0Color0_Trans;
    [SerializeField] private Transform Set0Color1_Trans;
    [SerializeField] private Transform Set1Color0_Trans;
    [SerializeField] private Transform Set1Color1_Trans;

    private Sequence Rotation_Seq;
    #endregion

    public void Reset_F()
    {
        Set0Color0_Trans.rotation = Quaternion.Euler(0f, 0f, 0f);
        Set0Color1_Trans.rotation = Quaternion.Euler(0f, 0f, 45f);
        Set1Color0_Trans.rotation = Quaternion.Euler(0f, 0f, -45f);
        Set1Color1_Trans.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (Rotation_Seq.IsActive()) Rotation_Seq.Kill();
        DoOneRotation_F();
    }

    private void DoOneRotation_F()
    {
        float rotateTime = 2f;
        Rotation_Seq = DOTween.Sequence();
        Rotation_Seq.Append(Set0Color0_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, 45f), rotateTime));
        Rotation_Seq.Insert(0f, Set0Color1_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), rotateTime));
        Rotation_Seq.Insert(0f, Set1Color0_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), rotateTime));
        Rotation_Seq.Insert(0f, Set1Color1_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, -45f), rotateTime));
        Rotation_Seq.AppendInterval(rotateTime);
        Rotation_Seq.Append(Set0Color0_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), rotateTime));
        Rotation_Seq.Insert(rotateTime * 2, Set0Color1_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, 45f), rotateTime));
        Rotation_Seq.Insert(rotateTime * 2, Set1Color0_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, -45f), rotateTime));
        Rotation_Seq.Insert(rotateTime * 2, Set1Color1_Trans.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), rotateTime));
        Rotation_Seq.AppendInterval(rotateTime);
        Rotation_Seq.AppendCallback(DoOneRotation_F);

    }
}
