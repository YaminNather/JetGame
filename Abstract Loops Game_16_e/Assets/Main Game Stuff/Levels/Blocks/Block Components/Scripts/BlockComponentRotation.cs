using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponentRotation : BlockComponentRotationBase
{
    private void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, m_RotationSpeed * m_RotationDir) * Time.deltaTime);
    }

    public override void Reset_F()
    {
        transform.localRotation = Quaternion.identity;
    }
}

public class BlockComponentRotationBase : LevelComponent
{
    #region Variables
    [SerializeField] protected float m_RotationSpeed = 90f;
    [SerializeField] protected int m_RotationDir = 1;
    #endregion
}
