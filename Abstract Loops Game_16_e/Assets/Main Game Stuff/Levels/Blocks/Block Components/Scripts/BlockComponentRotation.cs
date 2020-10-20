using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponentRotation : LevelComponent
{
    #region Variables
    [SerializeField] private float m_RotationSpeed = 90f;
    [SerializeField] private int m_RotationDir = 1;
    #endregion

    private void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, m_RotationSpeed * m_RotationDir) * Time.deltaTime);
    }

    public override void Reset_F()
    {
        transform.localRotation = Quaternion.identity;
    }
}
