using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponentRotation : LevelComponent
{
    #region Variables
    [SerializeField] private Vector3 RotateSpeed = new Vector3(0f, 0f, 20f);
    #endregion

    private void Update()
    {
        transform.Rotate(RotateSpeed * Time.deltaTime);
    }

    public override void Reset_F()
    {
        transform.localRotation = Quaternion.identity;
    }
}
