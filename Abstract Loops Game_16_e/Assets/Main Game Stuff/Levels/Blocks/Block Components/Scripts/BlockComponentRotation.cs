using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponentRotation : LevelComponent
{
    [SerializeField] private Vector3 RotateSpeed;

    private void Update()
    {
        transform.Rotate(RotateSpeed * Time.deltaTime);
    }

    public override void Reset_F()
    {
        transform.rotation = Quaternion.identity;
    }
}
