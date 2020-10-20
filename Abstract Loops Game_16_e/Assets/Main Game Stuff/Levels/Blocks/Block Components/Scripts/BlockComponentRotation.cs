using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponentRotation : LevelComponent
{
    #region Variables
    [SerializeField] private Vector3 RotateSpeed = new Vector3(0f, 0f, 20f);
    [SerializeField] private int m_RotationDir = 1;
    #endregion

    private void Update()
    {
        transform.Rotate(RotateSpeed * m_RotationDir * Time.deltaTime);
    }

    public override void Reset_F()
    {
        transform.localRotation = Quaternion.identity;
    }
}
